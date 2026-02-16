using SealFisher.Rendering.Graphics;
using SealFisher.Rendering.Graphics.Abstraction.Buffer;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using Silk.NET.GLFW;
using System;
using System.Runtime.CompilerServices;

namespace SealFisher.rendering.windowing
{
    public unsafe class Window
    {
        private readonly uint width;
        private readonly uint height;

        public WindowHandle* instance;
        public ComPtr<IDXGISwapChain1> swapChain;
        public ComPtr<ID3D11RenderTargetView> renderTarget;
        public Viewport viewport;

        public VertexBuffer primitiveBuffer;

        public Window(int width, int height, string title)
        {
            this.width = (uint)width;
            this.height = (uint)height;

            Renderer.glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi); //NoApi means DX Context can be used
            instance = Renderer.glfw.CreateWindow(width, height, title, null, null);

            if (instance == null) throw new Exception("Could not create window");

            viewport = new Viewport() { Width = width, Height = height };
            InitSwapChain();
            InitRenderTargetView(); //Create RTV (basically a reference to the buffer, making it accessible for drawing)
            primitiveBuffer = new VertexBuffer(PrimitiveRenderer.layout);

            Renderer.wnds.Add(this);
        }

        private void InitSwapChain()
        {
            //Create double buffered chain
            SwapChainDesc1 desc = new SwapChainDesc1()
            {
                Width = width,
                Height = height,
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
        }

        public void SetPos(int x, int y)
        {
            Renderer.glfw.SetWindowPos(instance, x, y);
        }

        public void Draw(float x, float y, float r, float g, float b, float a)
        {
            primitiveBuffer.Put(x, y, r, g, b, a);
        }

        public IntPtr GetNativePtr() => new GlfwNativeWindow(Renderer.glfw, instance).Win32.Value.Hwnd;
    }
}
