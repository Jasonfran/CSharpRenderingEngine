namespace Engine.Renderer
{
    public class MeshDataPointer
    {
        public int Start;
        public int Count;
        public Material MeshMaterial;

        public MeshDataPointer(int start, int count, Material meshMaterial)
        {
            Start = start;
            Count = count;
            MeshMaterial = meshMaterial;
        }
    }
}