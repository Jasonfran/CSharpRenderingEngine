using Assimp.Configs;
using OpenTK;
using OpenTK.Graphics;

namespace Engine.UI
{
    public abstract class UIShape
    {
        public Transform2D Transform { get; }
        public Color4 Color { get; set; }
        public bool IsVisible { get; set; } = true;
        public int ZIndex = 0;

        protected UIShape(float x, float y, float w, float h)
        {
            Transform = new Transform2D(new Vector2(x, y), new Vector2(w, h));
        }

        public abstract float[] GetVerts();
    }
}