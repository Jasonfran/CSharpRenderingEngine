using System;
using System.Collections.Generic;
using Engine.Component;
using Engine.Core;

namespace Engine.Entity
{
    public class EntityManager : EngineSystem
    {
        public class Entity
        {
            private readonly EntityManager _entityManager;
            public int Id { get; }
            public Entity Parent { get; set; }
            public List<Entity> Children { get; }
            public Transform Transform { get; }


            public Entity(int id, EntityManager entityManager)
            {
                _entityManager = entityManager;
                Children = new List<Entity>();
                Transform = new Transform();
                Id = id;
            }

            public void AddChild(Entity entity)
            {
                Children.Add(entity);
            }

            public T AddComponent<T>(T component) where T : IComponent
            {
                return _entityManager.AddComponent(this, component);
            }

            public void RemoveComponent<T>() where T : IComponent
            {
                _entityManager.RemoveComponent<T>(this);
            }

            public T GetComponent<T>() where T : IComponent
            {
                return _entityManager.GetComponentForEntity<T>(this);
            }
        }

        private readonly Dictionary<int, Entity> _entitiesIndexId;
        private readonly Dictionary<int, List<IComponent>> _componentsForEntityId;
        private readonly Dictionary<string, List<IComponent>> _componentsByType;
        private readonly Dictionary<string, List<Entity>> _entitiesByComponent;
        private readonly Dictionary<Tuple<int, string>, IComponent> _componentForEntityByType;

        private readonly Dictionary<string, List<IEntityReciever>> _updateRecievers;

        private int _entityIdCounter = 0;

        public EntityManager(EngineSystemsCollection systems) : base(systems)
        {
            _entitiesIndexId = new Dictionary<int, Entity>();
            _componentsForEntityId = new Dictionary<int, List<IComponent>>();
            _componentsByType = new Dictionary<string, List<IComponent>>();
            _entitiesByComponent = new Dictionary<string, List<Entity>>();
            _componentForEntityByType = new Dictionary<Tuple<int, string>, IComponent>();

            _updateRecievers = new Dictionary<string, List<IEntityReciever>>();
        }

        public override void Init()
        {

        }

        public Entity NewEntity()
        {
            var entity = new Entity(_entityIdCounter++, this);
            _entitiesIndexId.Add(entity.Id, entity);
            return entity;
        }

        private T AddComponent<T>(Entity entity, T component) where T : IComponent
        {
            if (!_componentsForEntityId.ContainsKey(entity.Id))
            {
                _componentsForEntityId.Add(entity.Id, new List<IComponent>());
            }

            _componentsForEntityId[entity.Id].Add(component);

            string componentName = typeof(T).Name;
            if (!_componentsByType.ContainsKey(componentName))
            {
                _componentsByType.Add(componentName, new List<IComponent>());
            }

            _componentsByType[componentName].Add(component);

            if (!_entitiesByComponent.ContainsKey(componentName))
            {
                _entitiesByComponent.Add(componentName, new List<Entity>());
            }

            _entitiesByComponent[componentName].Add(entity);
            _componentForEntityByType.Add(new Tuple<int, string>(entity.Id, componentName), component);

            ComponentAddedNotify<T>(entity);

            return component;
        }

        private void RemoveComponent<T>(Entity entity, T component) where T : IComponent
        {
            string name = typeof(T).Name;
            _componentsForEntityId[entity.Id].Remove(component);
            _componentsByType[name].Remove(component);
            _entitiesByComponent[name].Remove(entity);
            _componentForEntityByType.Remove(new Tuple<int, string>(entity.Id, typeof(T).Name));

            ComponentRemovedNotify<T>(entity);
        }

        private void RemoveComponent<T>(Entity entity) where T : IComponent
        {
            var component = GetComponentForEntity<T>(entity);
            RemoveComponent(entity, component);
        }

        private T GetComponentForEntity<T>(Entity entity) where T : IComponent
        {
            if(_componentForEntityByType.TryGetValue(new Tuple<int, string>(entity.Id, typeof(T).Name), out var component))
            {
                return (T) component;
            }
            return default(T);
        }


        public List<Entity> GetEntitiesWithComponent<T>() where T : IComponent
        {
            if (_entitiesByComponent.TryGetValue(typeof(T).Name, out var entities))
            {
                return entities;
            }

            return null;
        }

        public void RegisterForUpdates<T>(IEntityReciever system) where T : IComponent
        {
            string name = typeof(T).Name;
            if (!_updateRecievers.ContainsKey(name))
            {
                _updateRecievers.Add(name, new List<IEntityReciever>());
            }
            _updateRecievers[name].Add(system);
        }

        private void ComponentAddedNotify<T>(Entity entity)
        {
            foreach (var receiver in _updateRecievers[typeof(T).Name])
            {
                receiver.EntityAdded(entity);
            }
        }

        private void ComponentRemovedNotify<T>(Entity entity)
        {
            foreach (var receiver in _updateRecievers[typeof(T).Name])
            {
                receiver.EntityRemoved(entity);
            }
        }
    }
}