namespace SealFisher.Rendering.Graphics.Abstraction
{
    public struct Color
    {
        public readonly float r, g, b, a;

        public Color(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public Color(float f)
        {
            r = f;
            g = f;
            b = f;
            a = 1.0f;
        }

        public Color(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            a = 1.0f;
        }

        public Color(int r, int g, int b, int a)
        {
            this.r = r / 255; 
            this.g = g / 255; 
            this.b = b / 255; 
            this.a = a / 255;
        }

        public Color(int i)
        {
            r = i / 255;
            g = i / 255;
            b = i / 255;
            a = 255;
        }

        public Color(int r, int g, int b)
        {
            this.r = r / 255;
            this.g = g / 255;
            this.b = b / 255;
            a = 255;
        }
    }
}
