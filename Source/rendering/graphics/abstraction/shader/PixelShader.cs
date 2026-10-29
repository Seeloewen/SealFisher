using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SealFisher.Rendering.Graphics.Abstraction.Shader
{
    public class PixelShader : IShader
    {
        public ComPtr<ID3D10Blob> shaderBlob { get; set; }
        public ComPtr<ID3D11PixelShader> instance;

        public unsafe void Create(string fileName)
        {
            shaderBlob = IShader.Compile(fileName, "ps_5_0");
            if (shaderBlob.Handle == null)
            {
                MessageBox.Show("Cannot create Pixel Shader. Compiled ShaderBlob is null.");
                return;
            }

            int i = Renderer.device.CreatePixelShader(shaderBlob.GetBufferPointer(), shaderBlob.GetBufferSize(), ref Unsafe.NullRef<ID3D11ClassLinkage>(), ref instance);

            if (i != 0)
            {
                MessageBox.Show("Failed to create Pixel Shader. Unknown Error");
                return;
            }
        }

        public void Use()
        {
            Renderer.deviceContext.PSSetShader(instance, ref SilkMarshal.NullRef<ComPtr<ID3D11ClassInstance>>(), 0);
        }
    }
}
