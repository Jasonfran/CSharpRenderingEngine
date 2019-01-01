using System.Collections.Generic;
using Engine.System;
using OpenTK.Input;

namespace Engine.UI
{
    public abstract class UIComponent
    {
        protected List<UIComponent> Components;

        protected UIComponent()
        {
            Components = new List<UIComponent>();
        }

        public abstract void Update(UIUpdateEventArgs args);

        public abstract void OnMouseClick(UIMouseClickEventArgs args);

        public abstract void OnMouseMove(int x, int y);

        public abstract void Render(RenderManager renderManager);
    }
}