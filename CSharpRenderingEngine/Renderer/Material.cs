using OpenTK;

namespace Engine.Renderer
{
    public class Material
    {
        public Texture2D AmbientTexture { get; set; }
        public Vector4 AmbientColor { get; set; }

        public Texture2D DiffuseTexture { get; set; }
        public Vector4 DiffuseColor { get; set; }

        public Texture2D SpecularTexture { get; set; }
        public Vector4 SpecularColor { get; set; }
    }
}