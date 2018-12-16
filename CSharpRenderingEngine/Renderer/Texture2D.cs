using OpenTK.Graphics;

namespace Engine.Renderer
{
    public class Texture2D
    {
        public bool Setup { get; set; }
        public int TextureId { get; set; }
        public Color4[,] Pixels { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public string Path { get; set; }

        public Texture2D() { }

        public Texture2D(int width, int height)
        {
            Width = width;
            Height = height;
            Pixels = new Color4[width, height];
        }

        public void SetPixel(int x, int y, Color4 color)
        {
            Pixels[x, y] = color;
        }
    }
}