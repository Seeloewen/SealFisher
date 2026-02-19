namespace SealFisher.Rendering.Graphics.Abstraction.Geometry
{
    public struct Rect
    {
        public readonly float x1, x2, y1, y2;

        public Rect(Resolution r, int x1, int y1, int x2, int y2)
        {
            this.x1 = Screen.xToScreen(r, x1);
            this.y1 = Screen.yToScreen(r, y1);
            this.x2 = Screen.xToScreen(r, x2);
            this.y2 = Screen.yToScreen(r, y2);
        }

        public Rect(float x1, float y1, float x2, float y2)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
        }
    }

    public struct Rect2
    {
        public readonly int x, y, w, h;

        public Rect2(int x, int y, int w, int h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }

        public int GetArea() => w * h;
    }
}
