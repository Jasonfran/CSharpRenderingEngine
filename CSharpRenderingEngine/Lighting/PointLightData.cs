using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;

namespace Engine.Lighting
{
    [StructLayout(LayoutKind.Explicit, Size=76)]
    public struct PointLightData : ILightData
    {
        public PointLightData(Vector3 position, Color4 ambientColor, Color4 diffuseColor, Color4 specularColor, float constant, float linear, float quadratic)
        {
            Position = position;
            AmbientColor = ambientColor;
            DiffuseColor = diffuseColor;
            SpecularColor = specularColor;
            Constant = constant;
            Linear = linear;
            Quadratic = quadratic;
        }

        [FieldOffset(0)] public Vector3 Position;
        [FieldOffset(16)] public Color4 AmbientColor;
        [FieldOffset(32)] public Color4 DiffuseColor;
        [FieldOffset(48)] public Color4 SpecularColor;
        [FieldOffset(64)] public float Constant;
        [FieldOffset(68)] public float Linear;
        [FieldOffset(72)] public float Quadratic;
    }
}