using Engine.Component;
using Engine.Core;
using Engine.Entity;
using Engine.Renderer;
using Engine.World;
using OpenTK;

namespace Engine.System
{
    public class RenderSystem : System<ModelComponent>
    {
        private readonly EngineSystemsCollection _engineSystems;
        private WorldManager _worldManager;
        private RenderManager _renderManager;

        public RenderSystem(EngineSystemsCollection engineSystems) : base(engineSystems)
        {
            _engineSystems = engineSystems;
        }

        public override void Init()
        {
            _worldManager = _engineSystems.GetSystem<WorldManager>();
            _renderManager = _engineSystems.GetSystem<RenderManager>();
        }

        public override void OnEntityAdded(EntityManager.Entity entity)
        {
            
        }

        public override void OnEntityRemoved(EntityManager.Entity entity)
        {
            
        }

        public override void Update(float dt)
        {
            foreach (var entity in _systemEntities)
            {
                var rotationY = entity.Value.Transform.EularRotation.Y;
                entity.Value.Transform.EularRotation = new Vector3(0.0f, rotationY + 10.0f * dt, 0.0f);

                var modelComponent = entity.Value.GetComponent<ModelComponent>();
                _renderManager.SendRenderData(new RenderCommand(modelComponent.Name, entity.Value));
            }
        }
    }
}