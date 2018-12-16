using System.Collections.Generic;
using Engine.Entity;

namespace Engine.World
{
    public class World
    {
        public List<EntityManager.Entity> ChildEntities { get; }

        public World()
        {
            ChildEntities = new List<EntityManager.Entity>();
        }

        public void AddChild(EntityManager.Entity entity)
        {
            ChildEntities.Add(entity);
        }
    }
}