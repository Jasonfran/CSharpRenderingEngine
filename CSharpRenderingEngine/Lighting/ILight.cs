using Engine.Entity;
using OpenTK.Graphics;

namespace Engine.Lighting
{
    public interface ILight
    {
        Transform Transform { get; }
        Color4 AmbientColor { get; set; }
        Color4 DiffuseColor { get; set; }
        Color4 SpecularColor { get; set; }
    }
}