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
            _systems.Add(typeof(T).ToString(), system);
        }

        public T GetSystem<T>() where T : IEngineSystem
        {
            var name = typeof(T).ToString();

            if (_systems.TryGetValue(name, out var system))
            {
                return (T) system;
            }

            return default(T);
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