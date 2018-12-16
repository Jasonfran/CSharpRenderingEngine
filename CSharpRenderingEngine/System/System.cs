using System.Collections.Generic;
using Engine.Component;
using Engine.Core;
using Engine.Entity;

namespace Engine.System
{
    public abstract class System<T> : ISystem, IEntityReciever where T : IComponent
    {
        private readonly EngineSystemsCollection _engineSystems;
        private readonly EntityManager _entityManager;

        protected readonly Dictionary<int, EntityManager.Entity> _systemEntities;

        public System(EngineSystemsCollection engineSystems)
        {
            _engineSystems = engineSystems;
            _entityManager = _engineSystems.GetSystem<EntityManager>();
            _entityManager.RegisterForUpdates<T>(this);
            _systemEntities = new Dictionary<int, EntityManager.Entity>();
        }

        public abstract void Init();

        public void EntityAdded(EntityManager.Entity entity)
        {
            _systemEntities.Add(entity.Id, entity);
            OnEntityAdded(entity);
        }

        public void EntityRemoved(EntityManager.Entity entity)
        {
            _systemEntities.Remove(entity.Id);
            OnEntityRemoved(entity);
        }

        public abstract void OnEntityAdded(EntityManager.Entity entity);
        public abstract void OnEntityRemoved(EntityManager.Entity entity);
        public abstract void Update(float dt);
    }
}