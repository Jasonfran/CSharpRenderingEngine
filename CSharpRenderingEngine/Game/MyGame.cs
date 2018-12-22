using System;
using Engine.Component;
using Engine.Core;
using Engine.Entity;
using Engine.Input;
using Engine.Lighting;
using Engine.Resources;
using Engine.System;
using Engine.World;
using OpenTK;
using OpenTK.Input;

namespace Engine.Game
{
    public class MyGame : Game
    {
        private WorldManager _worldManager;
        private EntityManager _entityManager;
        private ResourceManager _resourceManager;
        private InputManager _inputManager;

        private Light light;


        public MyGame(EngineSystemsCollection engineSystems, GameSystemCollection gameSystems) : base(engineSystems, gameSystems)
        {
            //gameSystems.AddSystem(new RenderManager(engineSystems));

            //gameSystems.InitAll();
        }

        public override void Init()
        {
            _inputManager = _engineSystems.GetSystem<InputManager>();
            _worldManager = _engineSystems.GetSystem<WorldManager>();
            _entityManager = _engineSystems.GetSystem<EntityManager>();
            _resourceManager = _engineSystems.GetSystem<ResourceManager>();

            var world = _worldManager.NewWorld();
            _worldManager.SetActiveWorld(world);

            var entity = _entityManager.NewEntity();
            entity.AddComponent(new RenderableComponent(_resourceManager.LoadModel("Models/2b.obj")));
            world.AddChild(entity);

            entity.Transform.Scale = new Vector3(0.3f, 0.3f, 0.3f);

            var camera = world.AddCamera(new Camera(new Vector3(0.0f, 0.0f, -50.0f)));
            world.SetActiveCamera(camera);

            light = world.AddLight(new PointLight(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), Vector3.One));
            
        }

        public override void Update(float dt)
        {
            light.Transform.Position = _worldManager.ActiveWorld.ActiveCamera.Transform.Position;
        }
    }
}