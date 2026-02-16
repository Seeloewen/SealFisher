using SealFisher.Util;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D.Compilers;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace SealFisher.Rendering.Graphics.Abstraction.Shader
{
    /* Not really how interfaces should be used, but who cares. This doesn't really serve any purpose of abstraction,
     * but is rather used as a blueprint for what shaders definitely need to implement.
     */
    public interface IShader
    {
        public ComPtr<ID3D10Blob> shaderBlob { get; set; }

        public void Create();

        public void Use();

        protected static unsafe ComPtr<ID3D10Blob> Compile(string fileName, string profile)
        {
            if (Renderer.device.Handle == null)
            {
                MessageBox.Show("Could not begin shader compilation. Make sure the renderer was initialized with a D3D11Device");
                return null;
            }

            ComPtr<ID3D10Blob> shaderBlob = default;
            ComPtr<ID3D10Blob> errorBlob = default;

            string rawContent = FileUtil.StrFromResource(fileName);
            var shaderBytes = Encoding.ASCII.GetBytes(rawContent);
            int i = Renderer.d3dCompiler.Compile(in shaderBytes[0], (nuint)shaderBytes.Length, nameof(rawContent), null, ref Unsafe.NullRef<ID3DInclude>(), "Main", profile, 0, 0, ref shaderBlob, ref errorBlob);

            if (i != 0)
            {
                MessageBox.Show("Shader could not be compiled: " + SilkMarshal.PtrToString((nint)errorBlob.GetBufferPointer()));
                return null;
            }

            return shaderBlob;
        }
    }
}
