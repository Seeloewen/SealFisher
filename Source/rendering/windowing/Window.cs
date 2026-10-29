using SealFisher.Rendering.Graphics;
using SealFisher.Rendering.Graphics.Abstraction.Buffer;
using SealFisher.Rendering.Graphics.Abstraction.Geometry;
using SealFisher.Rendering.Gui.Components;
using SealFisher.Rendering.Gui.Events;
using SealFisher.Source.Rendering.Graphics;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using Silk.NET.GLFW;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static Silk.NET.GLFW.GlfwCallbacks;

namespace SealFisher.Rendering.Windowing
{
    public unsafe class Window
    {
        public readonly string title;
        public readonly Resolution resolution;

        public WindowHandle* instance;
        public ComPtr<IDXGISwapChain1> swapChain;
        public ComPtr<ID3D11RenderTargetView> renderTarget;
        public Viewport viewport;

        public VertexBuffer primitiveBuffer;
        public VertexBuffer textureBuffer;

        //Holds the delegate so the callback isn't consumed by gargabe collection
        private ScrollCallback scrollCallback;

        public bool isVisible = false;
        private List<GuiComponent> children = new List<GuiComponent>();

        public Window(int width, int height, string title)
        {
            resolution = new Resolution(width, height);
            this.title = title;

            Renderer.glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi); //NoApi means DX Context can be used
            Renderer.glfw.WindowHint(WindowHintBool.Resizable, false);

            viewport = new Viewport() { Width = width, Height = height };
            primitiveBuffer = new VertexBuffer(PrimitiveRenderer.layout, () => PrimitiveRenderer.Render(this));
            textureBuffer = new VertexBuffer(TextureRenderer.layout, () => TextureRenderer.Render(this));
            scrollCallback = ScrollCallback;

            Renderer.wnds.Add(this);
        }

        private void ScrollCallback(WindowHandle* window, double xOffset, double yOffset)
        {
            Debug.WriteLine($"Scroll: {yOffset}");
            ScrollEvent ev = new ScrollEvent((int)yOffset);
            Scroll(ev);
            
        }

        public void Scroll(ScrollEvent ev)
        {
            foreach(GuiComponent c in children)
            {
                c.Scroll(ev);
                if (ev.consumed) break;
            }
        }

        public void Click(ClickEvent ev)
        {
            foreach (GuiComponent c in children)
            {
                c.Click(ev);
                if (ev.consumed) break;
            }
        }

        public void Show()
        {
            if (isVisible) return;

            instance = Renderer.glfw.CreateWindow(GetWidth(), GetHeight(), title, null, null);
            InitSwapChain();
            InitRenderTargetView(); //Create RTV (basically a reference to the buffer, making it accessible for drawing)
            Renderer.glfw.SetScrollCallback(instance, scrollCallback);
            isVisible = true;
        }

        public void Hide()
        {
            if (!isVisible) return;

            renderTarget.Dispose();
            swapChain.Dispose();
            renderTarget = default;
            swapChain = default;

            Renderer.glfw.DestroyWindow(instance);
            isVisible = false;
            instance = null;
        }

        public void AddChild(GuiComponent gui)
        {
            gui.SetParentWindow(this);
            children.Add(gui);
        }

        private void InitSwapChain()
        {
            //Create double buffered chain
            SwapChainDesc1 desc = new SwapChainDesc1()
            {
                Width = (uint)GetWidth(),
                Height = (uint)GetHeight(),
                Format = Format.FormatR8G8B8A8Unorm,
                SampleDesc = new SampleDesc() { Count = 1, Quality = 0 },
                BufferUsage = DXGI.UsageRenderTargetOutput,
                BufferCount = 2,
                SwapEffect = SwapEffect.FlipDiscard,
                Scaling = Scaling.Stretch,
            };

            int i = Renderer.factory.CreateSwapChainForHwnd(Renderer.device, GetNativePtr(), in desc, null, ref Unsafe.NullRef<IDXGIOutput>(), ref swapChain);
        }

        private void InitRenderTargetView()
        {
            //Set Backbuffer as RTV
            ComPtr<ID3D11Texture2D> backBuffer = swapChain.GetBuffer<ID3D11Texture2D>(0);
            Renderer.device.CreateRenderTargetView(backBuffer, null, ref renderTarget);
            backBuffer.Dispose();
        }

        public void SetPos(int x, int y)
        {
            Renderer.glfw.SetWindowPos(instance, x, y);
        }

        public IntPtr GetNativePtr() => new GlfwNativeWindow(Renderer.glfw, instance).Win32.Value.Hwnd;

        public List<GuiComponent> GetChildren() => children;

        public int GetWidth() => resolution.width;

        public int GetHeight() => resolution.height;
    }
}
