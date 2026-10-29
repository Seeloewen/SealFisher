using SealFisher.Rendering.Graphics;
using SealFisher.Rendering.Graphics.Abstraction.Geometry;
using SealFisher.Rendering.Gui.Components;
using SealFisher.Rendering.Gui.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealFisher.Rendering.Gui.Components
{
    public class ScrollViewer : GuiComponent
    {
        public int yOffset;
        public int contentHeight;
        public int scrollSpeed = 20;

        public ScrollViewer(int x, int y, int width, int height) : base(x, y, width, height)
        {
            hasScrollEvent = true;
        }

        public override void Render()
        {
            if (!isVisible) return;

            //Render this component and all its children
            Renderer.SetScissor(parentWindow, new Rect2(GetAbsX(), GetAbsY(), width, height));
            foreach (GuiComponent comp in children)
            {
                comp.Render();
            }
            Renderer.RemoveScissor(parentWindow);
        }

        protected override void OnRender() { }

        protected override int GetChildOffset() => -yOffset;

        public override void AddChild(GuiComponent comp)
        {
            base.AddChild(comp);

            UpdateContentHeight();
        }

        public void UpdateContentHeight()
        {
            contentHeight = 0;
            foreach(GuiComponent c in children)
            {
                //Only update content height when the child would be out of bounds
                contentHeight = Math.Max(contentHeight, c.posY + c.height);
            }
        }

        protected override void OnScroll(ScrollEvent ev)
        {
            int maxOffset = Math.Max(0, contentHeight - height);
            yOffset = Math.Clamp(yOffset - ev.offset * scrollSpeed, 0, maxOffset);
        }
    }
}
