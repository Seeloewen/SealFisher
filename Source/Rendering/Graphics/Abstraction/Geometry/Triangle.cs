namespace SealFisher.Rendering.Graphics.Abstraction.Geometry
{
    public struct Triangle
    {
        public readonly float x1, x2, x3, y1, y2, y3;

        public Triangle(Resolution r, int x1, int y1, int x2, int y2, int x3, int y3)
        {
            this.x1 = Screen.xToScreen(r,x1); 
            this.y1 = Screen.yToScreen(r,y1);
            this.x2 = Screen.xToScreen(r,x2);
            this.y2 = Screen.yToScreen(r,y2);
            this.x3 = Screen.xToScreen(r,x3);
            this.y3 = Screen.yToScreen(r,y3);
        }

    }
}
