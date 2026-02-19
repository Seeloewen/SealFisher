using SealFisher.Rendering.Gui.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealFisher.Rendering.Gui.Components
{
    public class ScrollViewer : GuiComponent
    {
        public float yOffset;
        public int contentHeight;

        public ScrollViewer(int x, int y, int width, int height) : base(x, y, width, height)
        {

        }

        protected override void OnRender()
        {

        }

        public override void AddChild(GuiComponent comp)
        {
            base.AddChild(comp);

            UpdateContentHeight();
        }

        public void UpdateContentHeight()
        {
            int h = 0;
            foreach(GuiComponent c in children)
            {
                h += c.height;
            }
        }

        public override void OnScroll(Scroll)
    }
}
