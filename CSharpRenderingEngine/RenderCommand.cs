using Engine.Entity;

namespace Engine
{
    public struct RenderCommand
    {
        public string ModelName { get; }
        public EntityManager.Entity Entity{ get; }

        public RenderCommand(string name, EntityManager.Entity entity)
        {
            ModelName = name;
            Entity = entity;
        }

    }
}