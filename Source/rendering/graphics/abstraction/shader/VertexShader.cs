using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SealFisher.Rendering.Graphics.Abstraction.Shader
{
    public class VertexShader : IShader
    {
        public ComPtr<ID3D10Blob> shaderBlob { get; set; }
        public ComPtr<ID3D11VertexShader> instance;

        public unsafe void Create(string fileName)
        {         
            shaderBlob = IShader.Compile(fileName, "vs_5_0");
            if (shaderBlob.Handle == null)
            {
                MessageBox.Show("Cannot create Vertex Shader. Compiled ShaderBlob is null.");
                return;
            }

            int i = Renderer.device.CreateVertexShader(shaderBlob.GetBufferPointer(), shaderBlob.GetBufferSize(), ref Unsafe.NullRef<ID3D11ClassLinkage>(), ref instance);

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