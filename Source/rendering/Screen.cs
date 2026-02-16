using SealFisher.Rendering.Graphics;
using Silk.NET.GLFW;

namespace SealFisher.Rendering
{
    static internal unsafe class Screen
    {
        internal static Monitor* monitor; //Primary monitor
        private static VideoMode* videoMode;

        internal static void Init()
        {
            monitor = Renderer.glfw.GetPrimaryMonitor();
            videoMode = Renderer.glfw.GetVideoMode(monitor);
        }

        internal static int GetWidth() => videoMode->Width;

        internal static int GetHeight() => videoMode->Height;
    }
}
