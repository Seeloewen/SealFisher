namespace SealFisher.Rendering.Gui.Events
{
    public class ClickEvent : GuiEvent
    {
        public MouseButton button;

        public ClickEvent(MouseButton btn)
        {
            button = btn;
        }
    }
}
