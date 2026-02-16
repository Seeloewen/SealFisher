using SealFisher.Rendering.Graphics;
using SealFisher.rendering.windowing;
using System.Linq;
using SealFisher.Rendering.Graphics.Abstraction.Geometry;
using SealFisher.Rendering.Graphics.Abstraction;

namespace SealFisher
{
    internal static class Game
    {
        public unsafe static void Run()
        {
            Renderer.InitGLFW();
            Renderer.InitD3D11();

            Window wndMain = new Window(500, 500, "wndMain");

            int i = 0;
            Window wndSecond = null;

            while (!Renderer.glfw.WindowShouldClose(wndMain.instance))
            {
                wndMain.primitiveBuffer.Put(0f, 0.8f, 0f, 1f, 0f, 0.5f);
                wndMain.primitiveBuffer.Put(0.8f, -0.8f, 1f, 0f, 0f, 0.5f);
                wndMain.primitiveBuffer.Put(-0.8f, -0.8f, 0f, 0f, 1f, 0.5f);


                if (i == 0)
                {
                    wndSecond = new Window(500, 500, "wndSecond");
                }
                i++;

                PrimitiveRenderer.DrawRect(wndSecond, new Rect(200, 200, 500, 500), new Color(0, 255, 0));
                PrimitiveRenderer.DrawTriangle(wndSecond, new Triangle(900, 200, 1100, 500, 700, 500), new Color(0, 0, 255));
                PrimitiveRenderer.DrawQuad(wndSecond, new Quad(1500, 200, 1900, 100, 2000, 500, 1400, 600), new Color(255, 0, 0));

                Renderer.glfw.PollEvents();
                Renderer.Render();


                //Check for each window if it should be closed
                foreach (Window wnd in Renderer.wnds.ToList())
                {
                    if (Renderer.glfw.WindowShouldClose(wnd.instance))
                    {
                        Renderer.glfw.DestroyWindow(wnd.instance);
                        Renderer.wnds.Remove(wnd);
                    }
                }
            }
        }
    }
}
