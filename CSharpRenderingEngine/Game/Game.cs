using Engine.Core;

namespace Engine.Game
{
    public abstract class Game
    {
        protected readonly EngineSystemsCollection _engineSystems;
        protected readonly GameSystemCollection _gameSystems;

        private World.World _gameWorld;

        public Game(EngineSystemsCollection engineSystems, GameSystemCollection gameSystems)
        {
            _engineSystems = engineSystems;
            _gameSystems = gameSystems;
        }

        public abstract void Init();

        public void UpdateGame(float dt)
        {
            _gameSystems.UpdateAll(dt);
            Update(dt);
        }

        public abstract void Update(float dt);
    }
}