using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WinRT;

namespace SealFisher.rendering.graphics.abstraction.shader
{
    public class PixelShader : IShader
    {
        public ComPtr<ID3D11PixelShader> instance;
        public ComPtr<ID3D10Blob> byteCode;

        public unsafe void Create()
        {
            byteCode = IShader.Compile(Shaders.pixelShader, "ps_5_0");
            if(byteCode.Handle == null)
            {
                MessageBox.Show("Cannot create Pixel Shader. Compiled ShaderBlob is null.");
                return;
            }

            int i = Renderer.device.CreatePixelShader(byteCode.GetBufferPointer(), byteCode.GetBufferSize(), ref Unsafe.NullRef<ID3D11ClassLinkage>(), ref instance);
        
            if(i != 0)
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
