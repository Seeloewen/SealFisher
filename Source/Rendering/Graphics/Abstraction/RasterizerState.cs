using Silk.NET.Core.Native;
using Silk.NET.Direct3D.Compilers;
using Silk.NET.Direct3D11;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealFisher.Rendering.Graphics.Abstraction
{
    public class RasterizerState
    {
        private ComPtr<ID3D11RasterizerState> instance;

        public RasterizerState(bool enableScissor)
        {
            RasterizerDesc desc = new RasterizerDesc()
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.None,
                ScissorEnable = enableScissor,
                DepthClipEnable = true
            };

            Renderer.device.CreateRasterizerState(ref desc, ref instance);
        }

        public void Use()
        {
            Renderer.deviceContext.RSSetState(instance);
        }
    }
}
