using Engine.Entity;
using OpenTK;

namespace Engine.Lighting
{
    public abstract class Light
    {
        public Transform Transform { get; }

        public Vector3 AmbientColor { get; set; }
        public Vector3 DiffuseColor { get; set; }
        public Vector3 SpecularColor { get; set; }

        protected Light()
        {
            Transform = new Transform();
        }
    }
}