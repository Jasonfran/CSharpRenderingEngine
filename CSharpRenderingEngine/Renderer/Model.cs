using System.Collections.Generic;

namespace Engine.Renderer
{
    public struct Model
    {
        public List<Mesh> Meshes { get; }
        public string Path { get; }
        public Material CustomMaterial { get; set; }

        public Model(string path)
        {
            Meshes = new List<Mesh>();
            Path = path;
            CustomMaterial = null;
        }
    }
}