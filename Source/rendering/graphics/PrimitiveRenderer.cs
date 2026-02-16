using SealFisher.Rendering.Graphics.Abstraction.Buffer;
using SealFisher.Rendering.Graphics.Abstraction.Shader;
using SealFisher.rendering.windowing;
using Silk.NET.DXGI;

namespace SealFisher.Rendering.Graphics
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
