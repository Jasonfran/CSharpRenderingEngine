using System;
using System.Collections.Generic;

namespace Engine.Core
{
    public class EngineSystemsCollection
    {
        protected readonly Dictionary<string, IEngineSystem> _systems;

        public EngineSystemsCollection()
        {
            _systems = new Dictionary<string, IEngineSystem>();
        }

        public void AddSystem<T>(T system) where T : IEngineSystem
        {
            if (!_systems.ContainsKey(typeof(T).Name))
            {
                _systems.Add(typeof(T).ToString(), system);
                return;
            }

            throw new Exception($"{typeof(T).Name} is already added as a system!");
        }

        public T GetSystem<T>() where T : IEngineSystem
        {
            var name = typeof(T).ToString();

            if (_systems.TryGetValue(name, out var system))
            {
                return (T) system;
            }

            throw new Exception($"{typeof(T).Name} is not added as a system!");
        }

        public void InitAll()
        {
            foreach(var kv in _systems)
            {
                kv.Value.Init();
            }
        }
    }
}