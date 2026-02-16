namespace SealFisher.Rendering.Graphics.Abstraction.Geometry
{
    public struct Triangle
    {
        public readonly float x1, x2, x3, y1, y2, y3;

        public Triangle(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            this.x1 = Screen.xToScreen(x1); 
            this.y1 = Screen.yToScreen(y1);
            this.x2 = Screen.xToScreen(x2);
            this.y2 = Screen.yToScreen(y2);
            this.x3 = Screen.xToScreen(x3);
            this.y3 = Screen.yToScreen(y3);
        }

    }
}
