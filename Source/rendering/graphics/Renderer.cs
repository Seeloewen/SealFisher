using SealFisher.Rendering.Graphics.Abstraction;
using SealFisher.Rendering.Graphics.Abstraction.Geometry;
using SealFisher.Rendering.Gui.Components;
using SealFisher.Rendering.Windowing;
using SealFisher.Source.Rendering.Graphics;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D.Compilers;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using Silk.NET.GLFW;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using WinRT;

namespace SealFisher.Rendering.Graphics
{
    public enum Batch { None, Primitive, Texture }

    public static class Renderer
    {
        public static Glfw glfw;
        public static D3D11 d3d11;
        public static DXGI dxgi;
        public static D3DCompiler d3dCompiler;

        public static ComPtr<ID3D11Device> device;
        public static ComPtr<ID3D11DeviceContext> deviceContext;
        public static ComPtr<IDXGIFactory2> factory;
        public static RasterizerState defaultState;
        public static RasterizerState scissorState;

        public static ComPtr<ID3D11BlendState> blendState;

        private static Batch currentBatch = Batch.None;

        public static List<Window> wnds = new List<Window>();

        public static void InitGLFW()
        {
            glfw = Glfw.GetApi();
            glfw.Init();
            Screen.Init();
        }

        public unsafe static void InitD3D11()
        {
            d3d11 = D3D11.GetApi();
            dxgi = DXGI.GetApi();
            d3dCompiler = D3DCompiler.GetApi();

            factory = dxgi.CreateDXGIFactory1<IDXGIFactory2>();

            int i = d3d11.CreateDevice(default(ComPtr<IDXGIAdapter>), D3DDriverType.Hardware, default, 0, null, 0, D3D11.SdkVersion, ref device, null, ref deviceContext);
            if (i != 0)
            {
                throw new Exception("Could not create D3D11Device");
            }

            PrimitiveRenderer.Init();
            TextureRenderer.Init();

            defaultState = new RasterizerState(false);
            scissorState = new RasterizerState(true);

            defaultState.Use();

            //Enable alpha blending, so transparent pixels and colors work
            BlendDesc blendDesc = new BlendDesc();
            blendDesc.RenderTarget[0] = new RenderTargetBlendDesc
            {
                BlendEnable = true,
                SrcBlend = Blend.SrcAlpha,
                DestBlend = Blend.InvSrcAlpha,
                BlendOp = BlendOp.Add,
                SrcBlendAlpha = Blend.One,
                DestBlendAlpha = Blend.InvSrcAlpha,
                BlendOpAlpha = BlendOp.Add,
                RenderTargetWriteMask = (byte)ColorWriteEnable.All
            };
            device.CreateBlendState(ref blendDesc, ref blendState);
            deviceContext.OMSetBlendState(blendState, null, 0xFFFFFFFF);
        }


        public static void Render()
        {
            float[] clearColor = { 0.243f, 0.898f, 0.941f, 1.0f };

            foreach (Window wnd in wnds)
            {
                if (!wnd.isVisible) continue;

                deviceContext.ClearRenderTargetView(wnd.renderTarget, ref clearColor[0]);
                deviceContext.IASetPrimitiveTopology(D3DPrimitiveTopology.D3D11PrimitiveTopologyTrianglelist);
                deviceContext.RSSetViewports(1, ref wnd.viewport);
                deviceContext.OMSetRenderTargets(1, ref wnd.renderTarget, ref Unsafe.NullRef<ID3D11DepthStencilView>());

                wnd.GetChildren().ForEach(c => c.Render()); //Render all children of the window

                FlushBatch(wnd);
                wnd.swapChain.Present(0, 0);
            }
        }

        public static void SetScissor(Window wnd, Rect2 r)
        {
            FlushBatch(wnd);

            Box2D<int> box = new Box2D<int>(r.x, r.y, r.x + r.w, r.y + r.h);
            scissorState.Use();
            deviceContext.RSSetScissorRects(1, ref box);
        }

        public static void RemoveScissor(Window wnd)
        {
            FlushBatch(wnd);
            defaultState.Use();
        }

        public static void SwitchBatch(Window wnd, Batch batch)
        {
            if (currentBatch == batch) return;
            FlushBatch(wnd);
            currentBatch = batch;
        }

        public static void FlushBatch(Window wnd)
        {
            PrimitiveRenderer.Render(wnd);
            TextureRenderer.Render(wnd);
            currentBatch = Batch.None;
        }
    }
}