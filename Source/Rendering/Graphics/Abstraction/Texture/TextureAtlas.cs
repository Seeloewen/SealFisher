using Newtonsoft.Json.Linq;
using SealFisher.Rendering.Graphics.Abstraction.Geometry;
using SealFisher.Util;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;

namespace SealFisher.Rendering.Graphics.Abstraction.Texture
{
    public static class TextureAtlas
    {
        private static Dictionary<string, Rect2> boundsMap = new Dictionary<string, Rect2>();
        private static Dictionary<string, Image<Rgba32>> imageMap = new Dictionary<string, Image<Rgba32>>();
        private static ComPtr<ID3D11Texture2D> mapTexture;
        private static Image<Rgba32> mapImage;
        public static ComPtr<ID3D11ShaderResourceView> mapResource;

        public static void LoadMappings(string fileName)
        {
            JObject obj = JObject.Parse(FileUtil.StrFromResource("texture_map.json"));
            foreach (JProperty prop in obj.Properties())
            {
                //Load all textures as RGBA32 Image
                Image<Rgba32> img = FileUtil.ImgFromResource((string)prop.Value);
                imageMap.Add(prop.Name, img);
            }

            //Construct texture map from images
            List<Rect2> rects = new List<Rect2>() { new Rect2(0, 0, 10000, 10000) };
            foreach (var img in imageMap)
            {
                int w = img.Value.Width + 2;
                int h = img.Value.Height + 2;

                (int i, Rect2 r) bestRect = (int.MaxValue, default);
                foreach (Rect2 rect in rects)
                {
                    //Skip the rectangle if the image doesn't fit
                    if (rect.w < w || rect.h < h) continue;

                    //If the remaining free space in this rectangle after the image would be added is lower, take it (waste as little space as possible)
                    int free = rect.w * rect.h - w * h;
                    if (free < bestRect.i) bestRect = (free, rect);
                }

                //Calculate how the remaining space should be split
                Rect2 r = bestRect.r;
                Rect2 r1 = new Rect2(r.x + w, r.y, r.w - w, r.h);
                Rect2 r2 = new Rect2(r.x, r.y + h, w, r.h - h);
                Rect2 r3 = new Rect2(r.x, r.y + h, r.w, r.h - h);
                Rect2 r4 = new Rect2(r.x + w, r.y, r.w - w, h);

                rects.Remove(r);
                if (Math.Max(r1.GetArea(), r2.GetArea()) > Math.Max(r3.GetArea(), r4.GetArea()))
                {
                    rects.Add(r1);
                    rects.Add(r2);
                }
                else
                {
                    rects.Add(r3);
                    rects.Add(r4);
                }

                boundsMap.Add(img.Key, new Rect2(r.x, r.y, w - 2, h - 2));
            }
        }

        public static void Create()
        {
            Image<Rgba32> mapImg = new Image<Rgba32>(10000, 10000);
            foreach(var img in imageMap)
            {
                Rect2 rect = boundsMap[img.Key];
                mapImg.Mutate(o => o.DrawImage(img.Value, new Point(rect.x, rect.y), 1f));
            }

            mapImage = mapImg;

            BuildTexture();
        }

        private unsafe static void BuildTexture()
        {
            //Load image as value array
            var image = mapImage;
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

                Renderer.device.CreateTexture2D(ref desc, ref sub, ref mapTexture);
            }

            Renderer.device.CreateShaderResourceView(mapTexture, null, ref mapResource);
        }

        public static Rect GetTextureBounds(string textureId)
        {
            if (!boundsMap.ContainsKey(textureId)) return new Rect(0, 0, 1, 1);

            Rect2 mapBounds = boundsMap[textureId];

            return new Rect(mapBounds.x / 10000f,
                            mapBounds.y / 10000f,
                            (mapBounds.x + mapBounds.w) / 10000f,
                            (mapBounds.y + mapBounds.h) / 10000f);
        }
    }
}
