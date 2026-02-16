using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SealFisher.rendering.graphics.abstraction.shader
{
    public class VertexShader : IShader
    {
        public ComPtr<ID3D11VertexShader> instance;
        public ComPtr<ID3D10Blob> byteCode;

        public unsafe void Create()
        {
            byteCode = IShader.Compile(Shaders.vertexShader, "vs_5_0");
            if (byteCode.Handle == null)
            {
                MessageBox.Show("Cannot create Vertex Shader. Compiled ShaderBlob is null.");
                return;
            }

            int i = Renderer.device.CreateVertexShader(byteCode.GetBufferPointer(), byteCode.GetBufferSize(), ref Unsafe.NullRef<ID3D11ClassLinkage>(), ref instance);

            if (i != 0)
            {
                MessageBox.Show("Failed to create Vertex Shader. Unknown Error");
                return;
            }
        }

        public void Use()
        {
            Renderer.deviceContext.VSSetShader(instance, ref SilkMarshal.NullRef<ComPtr<ID3D11ClassInstance>>(), 0);
        }
    }
}