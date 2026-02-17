using SealFisher.Rendering.Graphics;
using System.Linq;
using SealFisher.Rendering.Graphics.Abstraction.Geometry;
using SealFisher.Rendering.Graphics.Abstraction;
using SealFisher.Rendering.Windowing;
using SealFisher.Rendering.Gui.Components;

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

                    Rectangle r = new Rectangle(200, 200, 1000, 1000, new Color(0.5f));
                    r.AddChild(new Rectangle(30, 30, 200, 200, new Color(0.9f, 0f, 0f)));
                    wndSecond.AddChild(r);
                    wndSecond.AddChild(new Rectangle(200, 1200, 500, 200, new Color(0.5f)));
                    wndSecond.AddChild(new Rectangle(0, 0, 200, 200, new Color(0.5f, 0.0f, 0.0f)));
                    wndSecond.AddChild(new Rectangle(2460, 1340, 100, 100, new Color(0.5f, 0.0f, 0.0f)));

                }
                i++;

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
