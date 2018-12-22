using OpenTK;

namespace Engine.Lighting
{
    public class PointLight : Light
    {
        public float Constant = 1.0f;
        public float Linear = 0.045f;
        public float Quadratic = 0.0075f;

        public PointLight(Vector3 ambient, Vector3 diffuse, Vector3 specular)
        {
            AmbientColor = ambient;
            DiffuseColor = diffuse;
            SpecularColor = specular;
        }
    }
}