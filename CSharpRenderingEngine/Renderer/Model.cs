using System.Collections.Generic;

namespace Engine.Renderer
{
    public class Model
    {
        public List<Mesh> Meshes { get; }

        public Model()
        {
            Meshes = new List<Mesh>();
        }
    }
}