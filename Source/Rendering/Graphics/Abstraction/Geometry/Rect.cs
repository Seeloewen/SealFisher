namespace SealFisher.Rendering.Graphics.Abstraction.Geometry
{
    public struct Rect
    {
        public readonly float x1, x2, y1, y2;

        public Rect(int x1, int y1, int x2, int y2)
        {
            this.x1 = Screen.xToScreen(x1);
            this.y1 = Screen.yToScreen(y1);
            this.x2 = Screen.xToScreen(x2);
            this.y2 = Screen.yToScreen(y2);
        }
    }
}
