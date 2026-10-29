namespace SealFisher.Rendering.Gui.Events
{
    public class ClickEvent : GuiEvent
    {
        public readonly MouseButton button;

        public ClickEvent(MouseButton btn)
        {
            button = btn;
        }
    }
}
