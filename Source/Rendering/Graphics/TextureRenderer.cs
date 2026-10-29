using SealFisher.Rendering.Graphics;
using SealFisher.Rendering.Graphics.Abstraction.Buffer;
using SealFisher.Rendering.Graphics.Abstraction.Geometry;
using SealFisher.Rendering.Graphics.Abstraction.Shader;
using SealFisher.Rendering.Graphics.Abstraction.Texture;
using SealFisher.Rendering.Windowing;
using SealFisher.Util;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SealFisher.Source.Rendering.Graphics
{
    public class TextureRenderer
    {
        private static VertexShader vertexShader;
        private static PixelShader pixelShader;
        public static InputLayout layout;

        private static ComPtr<ID3D11SamplerState> sampler;

        public static void Init()
        {
            vertexShader = new VertexShader();
            vertexShader.Create("texture_vertex.hlsl");

            pixelShader = new PixelShader();
            pixelShader.Create("texture_pixel.hlsl");

            layout = new InputLayout();
            layout.AddInfoElement("POSITION", Format.FormatR32G32B32Float, 12);
            layout.AddInfoElement("TEXCOORD", Format.FormatR32G32Float, 8);
            layout.Create(vertexShader);

            SamplerDesc sampDesc = new SamplerDesc
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                MinLOD = 0,
                MaxLOD = float.MaxValue
            };
            Renderer.device.CreateSamplerState(ref sampDesc, ref sampler);

            TextureAtlas.LoadMappings("texture_map.json");
            TextureAtlas.Create();
        }

        public static void Draw(Window wnd, Rect rect, string textureId)
        {
            Renderer.SwitchBatch(wnd, Batch.Texture);

            Rect textureBounds = TextureAtlas.GetTextureBounds(textureId);

            wnd.textureBuffer.Put(rect.x1, rect.y1, textureBounds.x1, textureBounds.y1);
            wnd.textureBuffer.Put(rect.x2, rect.y2, textureBounds.x2, textureBounds.y2);
            wnd.textureBuffer.Put(rect.x1, rect.y2, textureBounds.x1, textureBounds.y2);
            wnd.textureBuffer.Put(rect.x1, rect.y1, textureBounds.x1, textureBounds.y1);
            wnd.textureBuffer.Put(rect.x2, rect.y1, textureBounds.x2, textureBounds.y1);
            wnd.textureBuffer.Put(rect.x2, rect.y2, textureBounds.x2, textureBounds.y2);

        }

        public static void Render(Window wnd)
        {
            if (wnd.textureBuffer.GetVertexCount() == 0) return;

            wnd.textureBuffer.Use();
            vertexShader.Use();
            pixelShader.Use();
            Renderer.deviceContext.PSSetShaderResources(0, 1, ref TextureAtlas.mapResource);
            Renderer.deviceContext.PSSetSamplers(0, 1, ref sampler);
            wnd.textureBuffer.Flush();
        }
    }
}
