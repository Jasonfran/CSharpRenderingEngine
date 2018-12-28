using Engine.Entity;
using OpenTK;
using OpenTK.Graphics;

namespace Engine.Lighting
{
    public class PointLight : ILight
    {
        public Transform Transform { get; }
        public Color4 AmbientColor { get; set; }
        public Color4 DiffuseColor { get; set; }
        public Color4 SpecularColor { get; set; }

        public float Constant { get; set; }
        public float Linear { get; set; }
        public float Quadratic { get; set; }

        public PointLight(Color4 ambient, Color4 diffuse, Color4 specular)
        {
            AmbientColor = ambient;
            DiffuseColor = diffuse;
            SpecularColor = specular;
            Constant = 1.0f;
            Linear = 0.045f;
            Quadratic = 0.0075f;

            Transform = new Transform();
        }

    }
}