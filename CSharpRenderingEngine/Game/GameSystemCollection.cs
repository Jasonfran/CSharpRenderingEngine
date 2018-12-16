using System.Collections.Generic;
using Engine.System;

namespace Engine.Game
{
    public class GameSystemCollection
    {
        protected readonly Dictionary<string, ISystem> _systems;

        public GameSystemCollection()
        {
            _systems = new Dictionary<string, ISystem>();
        }

        public void AddSystem<T>(T system) where T : ISystem
        {
            _systems.Add(typeof(T).ToString(), system);
        }

        public T GetSystem<T>() where T : ISystem
        {
            var name = typeof(T).ToString();

            if (_systems.TryGetValue(name, out var system))
            {
                return (T)system;
            }

            return default(T);
        }

        public void InitAll()
        {
            foreach (var kv in _systems)
            {
                kv.Value.Init();
            }
        }

        public void UpdateAll(float dt)
        {
            foreach (var kv in _systems)
            {
                kv.Value.Update(dt);
            }
        }
    }
}