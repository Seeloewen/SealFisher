using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealFisher.Rendering.Gui.Events
{
    public class ScrollEvent : GuiEvent
    {
        public readonly int offset;

        public ScrollEvent(int offset)
        {
            this.offset = offset; 
        }
    }
}
