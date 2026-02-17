using SealFisher.Rendering.Graphics;
using Silk.NET.GLFW;

namespace SealFisher.Rendering
{
    static public unsafe class Screen
    {
        public static Monitor* monitor; //Primary monitor
        private static VideoMode* videoMode;

        public static void Init()
        {
            monitor = Renderer.glfw.GetPrimaryMonitor();
            videoMode = Renderer.glfw.GetVideoMode(monitor);
        }

        public static int GetWidth() => videoMode->Width;

        public static int GetHeight() => videoMode->Height;

        public static float xToScreen(Resolution r, int x) => (x * 2f) / r.width - 1f;

        public static float yToScreen(Resolution r, int y) => ((r.height - y) * 2f) / r.height - 1f;

        public static int xToInt(Resolution r, float x) => (int)(((x + 1) / 2) * r.width);

        public static int yToInt(Resolution r, float y) => (int)((1f - (y + 1f) * 0.5f) * r.height);
    }
}
