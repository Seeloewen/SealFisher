using SealFisher.Rendering.Graphics;
using System.Linq;
using SealFisher.Rendering.Graphics.Abstraction.Geometry;
using SealFisher.Rendering.Graphics.Abstraction;
using SealFisher.Rendering.Windowing;
using SealFisher.Rendering.Gui.Components;
using SealFisher.Source.Rendering.Graphics;

namespace SealFisher
{
    internal static class Game
    {
        public static Window wndMain;
        public static Window wndSecond;

        public static void Init()
        {
            Renderer.InitGLFW();
            Renderer.InitD3D11();
        }

        public unsafe static void Run()
        {
            wndMain = new Window(500, 500, "wndMain");
            wndMain.Show();

            wndSecond = new Window(720, 480, "wndSecond");
            wndSecond.Show();

            Test1(); //Only for debugging

            while (!Renderer.glfw.WindowShouldClose(wndMain.instance))
            {
                Update();

                Test2(); //Only for debugging

                Renderer.glfw.PollEvents();
                Renderer.Render();

                //Check for each window if it should be closed
                foreach (Window wnd in Renderer.wnds.ToList())
                {
                    if (wnd.instance != null && Renderer.glfw.WindowShouldClose(wnd.instance))
                    {
                        Renderer.glfw.DestroyWindow(wnd.instance);
                        Renderer.wnds.Remove(wnd);
                    }
                }
            }
        }

        //Game logic loop, run once per frame update
        private static void Update()
        {
            
        }

        private static void Test1()
        {
            /*Rectangle r = new Rectangle(200, 200, 1000, 1000, new Color(0.5f));
            r.AddChild(new Rectangle(30, 30, 200, 200, new Color(0.9f, 0f, 0f)));
            wndSecond.AddChild(r);
            wndSecond.AddChild(new Rectangle(0, 0, 250, 250, new Color(0f, 1f, 0f)));
            wndSecond.AddChild(new Rectangle(250, 250, 250, 250, new Color(1f, 0f, 0f)));*/

        }

        private static void Test2()
        {
            wndMain.primitiveBuffer.Put(0f, 0.8f, 0f, 1f, 0f, 0.5f);
            wndMain.primitiveBuffer.Put(0.8f, -0.8f, 1f, 0f, 0f, 0.5f);
            wndMain.primitiveBuffer.Put(-0.8f, -0.8f, 0f, 0f, 1f, 0.5f);

            TextureRenderer.Draw(wndSecond, new Rect(wndSecond.resolution, 0, 0, 500, 500), "imgBackground.png");

        }
    }
}
