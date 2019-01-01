using OpenTK;

namespace Engine.UI
{
    public class Transform2D
    {
        public Vector2 Size { get; set; }
        public Vector2 Position { get; set; }

        public Transform2D(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
        }

        public bool HitTest(int x, int y)
        {
            return x >= Position.X && x <= Position.X + Size.X && y >= Position.Y && y <= Position.Y + Size.Y;
        }
    }
}