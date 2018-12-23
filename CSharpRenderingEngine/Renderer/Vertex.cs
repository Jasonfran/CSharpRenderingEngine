using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTK;

namespace Engine.Renderer
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 Color;
        public Vector2 TexCoords;

        public static readonly int Stride = Marshal.SizeOf(default(Vertex));

        public override bool Equals(object obj)
        {
            if (!(obj is Vertex))
            {
                return false;
            }

            var vertex = (Vertex)obj;
            return Position.Equals(vertex.Position) &&
                   Normal.Equals(vertex.Normal) &&
                   TexCoords.Equals(vertex.TexCoords);
        }

        public override int GetHashCode()
        {
            var hashCode = 582907564;
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(Position);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(Normal);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(TexCoords);
            return hashCode;
        }
    }
}
