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

        public static float xToScreen(int x) => (x * 2f) / GetWidth() - 1f;

        public static float yToScreen(int y) => ((GetHeight() - y) * 2f) / GetHeight() - 1f;

        public static int xToInt(float x) => (int)(((x + 1) / 2) * GetWidth());

        public static int yToInt(float y) => (int)((1f - (y + 1f) * 0.5f) * GetHeight());

    }
}
