using System.Drawing;

namespace Engine.Input
{
    public class MouseData
    {
        public int MouseX { get; set; }
        public int MouseY { get; set; }
        public int XDelta { get; set; }
        public int YDelta { get; set; }

        public Point Position { get; set; }
    }
}