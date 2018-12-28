using Engine.Entity;

namespace Engine.Renderer
{
    public struct DrawElementsBaseVertexCommand : IRenderCommand
    {
        public MeshDataPointer DataPointer { get; private set; }
        public Transform Transform { get; private set; }
        public Shader Shader { get; private set; }
        public RenderCommandType Type { get; private set; }

        public DrawElementsBaseVertexCommand(MeshDataPointer dataPointer, Transform transform, Shader shader)
        {
            DataPointer = dataPointer;
            Transform = transform;
            Shader = shader;
            Type = RenderCommandType.RenderMesh;
        }

        public void Repackage(MeshDataPointer dataPointer, Transform transform, Shader shader)
        {
            DataPointer = dataPointer;
            Transform = transform;
            Shader = shader;
        }

    }
}