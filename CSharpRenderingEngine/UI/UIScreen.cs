using System;
using System.Collections.Generic;
using OpenTK.Input;

namespace Engine.UI
{
    public class UIUpdateEventArgs : EventArgs
    {
        public UIUpdateEventArgs(float delta)
        {
            Delta = delta;
        }

        public float Delta { get; set; }
    }

    public class UIMouseClickEventArgs : EventArgs
    {
        public UIMouseClickEventArgs(MouseButton button, int x, int y)
        {
            Button = button;
            X = x;
            Y = y;
        }

        public MouseButton Button { get; set; }
        public int X { get; set; }
        public int Y { get; set; }


    }

    public abstract class UIScreen
    {
        protected event EventHandler<UIUpdateEventArgs> OnUpdate;
        protected event EventHandler<UIMouseClickEventArgs> OnMouseClick;

        private List<UIComponent> Components;

        protected UIScreen()
        {
            Components = new List<UIComponent>();
        }

        protected T AddComponent<T>(T component) where T : UIComponent
        {
            if (!Components.Contains(component))
            {
                Components.Add(component);
            }
            return component;
        }

        protected void RemoveComponent(UIComponent component)
        {
            Components.Remove(component);
        }

        public void Update(float dt)
        {
            var args = new UIUpdateEventArgs(dt);
            foreach (var uiComponent in Components)
            {
                uiComponent.Update(args);
            }

            var handler = OnUpdate;

            handler?.Invoke(this, args);

        }

        public void MouseClick(MouseButton button, int x, int y)
        {
            var args = new UIMouseClickEventArgs(button, x, y);
            foreach (var uiComponent in Components)
            {
                uiComponent.OnMouseClick(args);
            }

            var handler = OnMouseClick;

            handler?.Invoke(this, args);
        }

        public List<UIComponent> GetComponents()
        {
            return Components;
        }
    }
}