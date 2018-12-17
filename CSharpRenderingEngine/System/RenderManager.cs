using System.Collections.Generic;
using Engine.Component;
using Engine.Core;
using Engine.Entity;
using Engine.Renderer;
using Engine.World;
using OpenTK;
using NotImplementedException = System.NotImplementedException;

namespace Engine.System
{
    public class RenderManager : EngineSystem, IEntityReciever
    {
        private readonly EngineSystemsCollection _engineSystems;
        private WorldManager _worldManager;
        private OpenGLRendererCore _openGlRenderer;

        private List<EntityManager.Entity> _renderableEntities;
        private EntityManager _entityManager;

        public RenderManager(EngineSystemsCollection engineSystems) : base(engineSystems)
        {
            _engineSystems = engineSystems;
            _renderableEntities = new List<EntityManager.Entity>();

        }

        public override void Init()
        {
            _entityManager = _engineSystems.GetSystem<EntityManager>();
            _worldManager = _engineSystems.GetSystem<WorldManager>();
            _openGlRenderer = _engineSystems.GetSystem<OpenGLRendererCore>();

            _entityManager.RegisterForUpdates<RenderableComponent>(this);
        }

        public void EntityAdded(EntityManager.Entity entity)
        {
            _renderableEntities.Add(entity);
        }

        public void EntityRemoved(EntityManager.Entity entity)
        {
            _renderableEntities.Remove(entity);
        }

        public void Render()
        {
            _openGlRenderer.FrameBegin();
            foreach (var entity in _renderableEntities)
            {
                //var rotationY = entity.Value.Transform.EularRotation.Y;
                //entity.Value.Transform.EularRotation = new Vector3(0.0f, rotationY + 10.0f * dt, 0.0f);

                var renderableComponent = entity.GetComponent<RenderableComponent>();
                _openGlRenderer.RenderModel(renderableComponent.Model, entity.Transform.ModelMatrix);
            }
            _openGlRenderer.FrameEnd();
        }

    }
}