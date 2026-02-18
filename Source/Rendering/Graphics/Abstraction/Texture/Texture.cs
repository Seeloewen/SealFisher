using SealFisher.Util;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Protection.PlayReady;

namespace SealFisher.Rendering.Graphics.Abstraction.Texture
{
    public class Texture
    {
        //Warning: All of this should not be here, it should be part of a texture atlas system!!
        public static ComPtr<ID3D11Texture2D> instance;
        public static ComPtr<ID3D11ShaderResourceView> srv;

        public unsafe Texture(string fileName)
        {
            //Load image as value array
            var image = FileUtil.ImgFromResource(fileName);
            Rgba32[] pixels = new Rgba32[image.Width * image.Height];
            image.CopyPixelDataTo(pixels);

            //Create description for 2d texture
            Texture2DDesc desc = new Texture2DDesc
            {
                Width = (uint)image.Width,
                Height = (uint)image.Height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.FormatR8G8B8A8Unorm,
                SampleDesc = new SampleDesc(1, 0),
                Usage = Usage.Default,
                BindFlags = (uint)BindFlag.ShaderResource,
                CPUAccessFlags = 0,
                MiscFlags = 0
            };

            //Create texture from description and pixel data
            fixed (void* p = pixels)
            {
                SubresourceData sub = new SubresourceData
                {
                    PSysMem = p,
                    SysMemPitch = (uint)(image.Width * sizeof(Rgba32))
                };

                instance.Dispose();
                srv.Dispose();
                Renderer.device.CreateTexture2D(ref desc, ref sub, ref instance);
            }

            Renderer.device.CreateShaderResourceView(instance, null, ref srv);
        }
    }
}
