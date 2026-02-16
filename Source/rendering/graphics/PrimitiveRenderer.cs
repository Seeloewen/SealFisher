using SealFisher.rendering.graphics.abstraction.buffer;
using SealFisher.rendering.graphics.abstraction.shader;
using SealFisher.rendering.windowing;
using Silk.NET.DXGI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealFisher.rendering.graphics
{
    public class PrimitiveRenderer
    {
        private static VertexShader vertexShader;
        private static PixelShader pixelShader;
        public static InputLayout layout;

        public static void Init()
        {
            vertexShader = new VertexShader();
            vertexShader.Create();

            pixelShader = new PixelShader();
            pixelShader.Create();

            layout = new InputLayout();
            layout.AddInfoElement("POSITION", Format.FormatR32G32B32Float, 12);
            layout.AddInfoElement("COLOR", Format.FormatR32G32B32A32Float, 16);
            layout.Create(vertexShader);
        }

        public static void Render(Window wnd)
        {
            wnd.primitiveBuffer.Use();
            vertexShader.Use();
            pixelShader.Use();
            wnd.primitiveBuffer.Flush();

        }
    }
}
