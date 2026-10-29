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

            while (wndMain.isVisible)
            {
                Update();

                Test2(); //Only for debugging

                Renderer.glfw.PollEvents();
                Renderer.Render();

                //Check for each window if it should be closed
                foreach (Window wnd in Renderer.wnds)
                {
                    if (wnd.isVisible && Renderer.glfw.WindowShouldClose(wnd.instance))
                    {
                        wnd.Hide();
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
            ScrollViewer sv = new ScrollViewer(0, 0, 200, 300);
            sv.AddChild(new Rectangle(0, 0, 200, 200, new Color(0f, 1f, 0f)));
            sv.AddChild(new Rectangle(0, 200, 200, 200, new Color(1f, 0f, 0f)));
            wndSecond.AddChild(sv);
        }

        private static void Test2()
        {
            wndMain.primitiveBuffer.Put(0f, 0.8f, 0f, 1f, 0f, 1f);
            wndMain.primitiveBuffer.Put(0.8f, -0.8f, 1f, 0f, 0f, 1f);
            wndMain.primitiveBuffer.Put(-0.8f, -0.8f, 0f, 0f, 1f, 1f);

            //TextureRenderer.Draw(wndSecond, new Rect(wndSecond.resolution, 0, 0, 720, 480), "background_2_idle");

        }
    }
}
