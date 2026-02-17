using SealFisher.Rendering.Graphics.Abstraction.Geometry;
using SealFisher.Rendering.Gui.Events;
using SealFisher.Rendering.Windowing;
using System;
using System.Collections.Generic;

namespace SealFisher.Rendering.Gui.Components
{
    public abstract class GuiComponent
    {
        protected Window parentWindow;
        protected Rect parentBounds;
        protected GuiComponent parent;
        protected List<GuiComponent> children = new List<GuiComponent>();

        public int posX;
        public int posY;
        public int width;
        public int height;

        public bool isVisible = true;

        public Action onLeftClick;
        public Action onRightClick;

        public GuiComponent(int posX, int posY, int width, int height)
        {
            this.posX = posX;
            this.posY = posY;
            this.width = width;
            this.height = height;

            parentBounds = new Rect(new Resolution(0,0), 0, 0, 0, 0);
        }

        public void Render()
        {
            if (!isVisible) return;

            //Render this component and all its children
            OnRender();
            foreach (GuiComponent comp in children)
            {
                comp.Render();
            }
        }
        protected abstract void OnRender();

        public void Click(ClickEvent ev)
        {
            //If this component has a click event, consume the event
            if (onLeftClick != null || onRightClick != null)
            {
                OnClick(ev);
            }

            if (ev.consumed) return;

            //If the event hasn't been consumed by this point, send it to the children
            foreach (GuiComponent comp in children)
            {
                comp.Click(ev);
            }
        }

        protected virtual void OnClick(ClickEvent ev)
        {
            ev.consumed = true;

            if (onLeftClick != null && ev.button == MouseButton.Left) onLeftClick();

            if (onRightClick != null && ev.button == MouseButton.Right) onRightClick();
        }


        public void AddChild(GuiComponent comp)
        {
            comp.parent = this;

            //Update the parent window related properties if it's already in a window
            if(parentWindow!= null)
            {
                comp.parentWindow = parentWindow;
                comp.parentBounds = GetRelativeBounds();
            }

            children.Add(comp);
        }

        public void SetParent(GuiComponent comp) => parent = comp;

        public void SetParentWindow(Window wnd)
        {
            parentWindow = wnd;
            foreach(GuiComponent comp in children) //Don't forget to also update the parent window for the children
            {
                comp.SetParentWindow(wnd);
                comp.parentBounds = GetRelativeBounds();
            }
        }

        public Rect GetRelativeBounds()
        {
            int relativeX = Screen.xToInt(parentWindow.resolution, parentBounds.x1) +posX;
            int relativeY = Screen.yToInt(parentWindow.resolution, parentBounds.y1) +posY;

            //Returns a rectangle relative to the parent's bounds
            return new Rect(parentWindow.resolution, relativeX, relativeY, relativeX + width, relativeY + height);
        }

        public void SetParentBounds(Rect r) => parentBounds = r;

        public Rect GetParentBounds() => parentBounds;

        public int GetParentX() => Screen.xToInt(parentWindow.resolution, parentBounds.x1);

        public int GetParentY() => Screen.yToInt(parentWindow.resolution, parentBounds.y1);

        public void SetPosX(int x) => posX = x;

        public void SetPosY(int y) => posY = y;

        public int GetPosX() => posX;

        public int GetPosY() => posY;

        public void SetWidth(int w) => width = w;

        public void SetHeight(int h) => height = h;

        public int GetWidth() => width;

        public int GetHeight() => height;

        public void Show() => isVisible = true;

        public void Hide() => isVisible = false;
    }
}
