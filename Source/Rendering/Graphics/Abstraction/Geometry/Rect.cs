namespace SealFisher.Rendering.Graphics.Abstraction.Geometry
{
    public struct Rect
    {
        public readonly float x1, x2, y1, y2;

        public Rect(Resolution r,int x1, int y1, int x2, int y2)
        {
            this.x1 = Screen.xToScreen(r,x1);
            this.y1 = Screen.yToScreen(r,y1);
            this.x2 = Screen.xToScreen(r,x2);
            this.y2 = Screen.yToScreen(r,y2);
        }
    }
}
