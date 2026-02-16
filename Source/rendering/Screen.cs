using SealFisher.rendering.graphics;
using Silk.NET.GLFW;
using System.Windows.Forms;

namespace SealFisher.rendering
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
