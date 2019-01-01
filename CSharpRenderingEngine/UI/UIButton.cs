using System;
using Engine.System;
using OpenTK.Graphics;
using NotImplementedException = System.NotImplementedException;

namespace Engine.UI
{
    public class UIButtonEventArgs : EventArgs
    {
        public UIButtonEventArgs(int x, int y, int relativeX, int relativeY)
        {
            X = x;
            Y = y;
            RelativeX = relativeX;
            RelativeY = relativeY;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int RelativeX { get; set; }
        public int RelativeY { get; set; }
    }

    public class UIButton : UIComponent
    {
        public event EventHandler<UIButtonEventArgs> OnClick;

        private int borderSize = 10;
        private Rect _backgroundRect;
        private bool toggle = false;
        private Rect _foregroundRect;

        public UIButton(int x, int y, int w, int h)
        {
            _backgroundRect = new Rect(x, y, w, h);
            _backgroundRect.Color = Color4.Red;

            _foregroundRect = new Rect(x+borderSize, y+borderSize, w-borderSize*2, h-borderSize*2);
            _foregroundRect.Color = Color4.White;
        }

        public override void Update(UIUpdateEventArgs args)
        {
            
        }

        public override void OnMouseClick(UIMouseClickEventArgs args)
        {
            if (_backgroundRect.Transform.HitTest(args.X, args.Y))
            {
                var buttonEventArgs = new UIButtonEventArgs(args.X, args.Y, args.X - (int)_backgroundRect.Transform.Position.X, args.Y - (int)_backgroundRect.Transform.Position.Y);
                var handler = OnClick;

                handler?.Invoke(this, buttonEventArgs);
            }

        }

        public override void OnMouseMove(int x, int y)
        {
        }

        public override void Render(RenderManager renderManager)
        {
            renderManager.RenderShape(_backgroundRect);
            renderManager.RenderShape(_foregroundRect);
        }
    }
}