using SealFisher.Rendering.Graphics.Abstraction.Buffer;
using SealFisher.Rendering.Graphics.Abstraction.Shader;
using Silk.NET.DXGI;
using SealFisher.Rendering.Graphics.Abstraction.Geometry;
using SealFisher.Rendering.Graphics.Abstraction;
using SealFisher.Rendering.Windowing;

namespace SealFisher.Rendering.Graphics
{
    public class PrimitiveRenderer
    {
        private static VertexShader vertexShader;
        private static PixelShader pixelShader;
        public static InputLayout layout;

        public static void Init()
        {
            //Create shaders and input layout for buffer
            vertexShader = new VertexShader();
            vertexShader.Create("primitive_vertex.hlsl");

            pixelShader = new PixelShader();
            pixelShader.Create("primitive_pixel.hlsl");

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

        public static void DrawRect(Window wnd, Rect rect, Color c)
        {
            wnd.primitiveBuffer.Put(rect.x1, rect.y1, c.r, c.g, c.b, c.a);
            wnd.primitiveBuffer.Put(rect.x2, rect.y2, c.r, c.g, c.b, c.a);
            wnd.primitiveBuffer.Put(rect.x1, rect.y2, c.r, c.g, c.b, c.a);
            wnd.primitiveBuffer.Put(rect.x1, rect.y1, c.r, c.g, c.b, c.a);
            wnd.primitiveBuffer.Put(rect.x2, rect.y1, c.r, c.g, c.b, c.a);
            wnd.primitiveBuffer.Put(rect.x2, rect.y2, c.r, c.g, c.b, c.a);
        }

        public static void DrawTriangle(Window wnd, Triangle triangle, Color c)
        {
            wnd.primitiveBuffer.Put(triangle.x1, triangle.y1, c.r, c.g, c.b, c.a);
            wnd.primitiveBuffer.Put(triangle.x2, triangle.y2, c.r, c.g, c.b, c.a);
            wnd.primitiveBuffer.Put(triangle.x3, triangle.y3, c.r, c.g, c.b, c.a);
        }

        public static void DrawQuad(Window wnd, Quad quad, Color c)
        {
            wnd.primitiveBuffer.Put(quad.x1, quad.y1, c.r, c.g, c.b, c.a);
            wnd.primitiveBuffer.Put(quad.x3, quad.y3, c.r, c.g, c.b, c.a);
            wnd.primitiveBuffer.Put(quad.x4, quad.y4, c.r, c.g, c.b, c.a);
            wnd.primitiveBuffer.Put(quad.x1, quad.y1, c.r, c.g, c.b, c.a);
            wnd.primitiveBuffer.Put(quad.x2, quad.y2, c.r, c.g, c.b, c.a);
            wnd.primitiveBuffer.Put(quad.x3, quad.y3, c.r, c.g, c.b, c.a);
        }
    }
}
