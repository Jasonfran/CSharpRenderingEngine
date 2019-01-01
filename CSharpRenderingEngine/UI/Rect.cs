using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;

namespace Engine.UI
{
    public class Rect : UIShape
    {
       
        public Rect(float x, float y, float w, float h) : base(x, y, w, h)
        {
            
        }

        public override float[] GetVerts()
        {
            var x = Transform.Position.X;
            var y = Transform.Position.Y;
            var w = Transform.Size.X;
            var h = Transform.Size.Y;

            var verts = new[]
            {
                x, y + h, 0.0f, 0.0f,
                x, y, 0.0f, 1.0f,
                x + w, y, 1.0f, 1.0f,
                x, y + h, 0.0f, 0.0f,
                x + w, y, 1.0f, 1.0f,
                x + w, y + h, 1.0f, 0.0f
            };

            return verts;
        }
    }
}