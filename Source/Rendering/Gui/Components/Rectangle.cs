using SealFisher.Rendering.Graphics;
using SealFisher.Rendering.Graphics.Abstraction;
using SealFisher.Rendering.Graphics.Abstraction.Geometry;

namespace SealFisher.Rendering.Gui.Components
{
    public class Rectangle : GuiComponent
    {
        protected Color color;
        public Rectangle(int posX, int posY, int width, int height, Color c) : base(posX, posY, width, height)
        {
            color = c;
        }

        protected override void OnRender()
        {
            int relativeX = GetParentX() + posX;
            int relativeY = GetParentY() + posY;
            PrimitiveRenderer.DrawRect(parentWindow, new Rect(relativeX, relativeY, relativeX + width, relativeY + height), color);
        }

        public void SetColor(Color c) => color = c;
    }
}
