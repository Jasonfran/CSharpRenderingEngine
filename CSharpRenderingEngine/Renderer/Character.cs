using OpenTK;

namespace Engine.Renderer
{
    public struct Character
    {
        public int TextureId;
        public Vector2 Size;
        public Vector2 Bearing;
        public int Advance;

        public Character(int textureId, Vector2 size, Vector2 bearing, int advance)
        {
            TextureId = textureId;
            Size = size;
            Bearing = bearing;
            Advance = advance;
        }
    }
}