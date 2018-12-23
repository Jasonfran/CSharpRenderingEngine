using System.Collections.Generic;
using OpenTK;

namespace Engine.Renderer
{
    public class Line3D
    {
        private List<Vector3> points;
        private List<Vertex> vertices;
        public float Width { get; set; }
        public Vector3 Color { get; set; }
        public Vector3 NormalDirection { get; set; }

        public Line3D()
        {
            points = new List<Vector3>();
            vertices = new List<Vertex>();

            NormalDirection = Vector3.UnitY;
        }

        public void AddPoint(float x, float y, float z)
        {
            points.Add(new Vector3(x, y, z));
        }

        public List<Vertex> GetVertices()
        {
            foreach (var point in points)
            {
                var normalAbove = NormalDirection;
                var normalBelow = -NormalDirection;

                var vertexBelow = new Vertex();
                vertexBelow.Position = point;
                vertexBelow.Normal = new Vector3(normalBelow);
                vertexBelow.Color = Color;
                vertices.Add(vertexBelow);

                var vertexAbove = new Vertex();
                vertexAbove.Position = point;
                vertexAbove.Normal = new Vector3(normalAbove);
                vertexAbove.Color = Color;
                vertices.Add(vertexAbove);
            }

            return vertices;
        }
    }
}