using Engine.Component;
using Engine.Core;
using Engine.Entity;
using Engine.Resources;
using Engine.System;
using Engine.World;
using OpenTK;

namespace Engine.Game
{
    public class MyGame : Game
    {
        private WorldManager _worldManager;
        private EntityManager _entityManager;
        private ResourceManager _resourceManager;

        public MyGame(EngineSystemsCollection engineSystems, GameSystemCollection gameSystems) : base(engineSystems, gameSystems)
        {
            gameSystems.AddSystem(new RenderSystem(engineSystems));

            gameSystems.InitAll();
        }

        public override void Init()
        {
            _worldManager = _engineSystems.GetSystem<WorldManager>();
            _entityManager = _engineSystems.GetSystem<EntityManager>();
            _resourceManager = _engineSystems.GetSystem<ResourceManager>();

            var world = _worldManager.NewWorld();
            _worldManager.SetActiveWorld(world);

            var entity = _entityManager.NewEntity();
            entity.AddComponent(new ModelComponent("Models/cube.obj"));
            world.AddChild(entity);
        }

        public override void Update(float dt)
        {
            
        }
    }
}