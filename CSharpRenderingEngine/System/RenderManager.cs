﻿using System;
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
        private WorldManager _worldManager;
        private OpenGLRendererCore _openGlRenderer;

        private List<EntityManager.Entity> _renderableEntities;
        private EntityManager _entityManager;

        public bool Debug { get; set; } = true;

        public RenderManager(EngineSystemsCollection engineSystems) : base(engineSystems)
        {
            _renderableEntities = new List<EntityManager.Entity>();

        }

        public override void Init()
        {
            _entityManager = EngineSystems.GetSystem<EntityManager>();
            _worldManager = EngineSystems.GetSystem<WorldManager>();
            _openGlRenderer = EngineSystems.GetSystem<OpenGLRendererCore>();

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
            _openGlRenderer.DebugMode = Debug;

            var activeWorld = _worldManager.ActiveWorld;
            var activeCamera = activeWorld.ActiveCamera;

            _openGlRenderer.SetView(activeCamera.GetViewMatrix());

            _openGlRenderer.UpdatePointLights(activeWorld.PointLights);

            _openGlRenderer.FrameBegin();
            foreach (var entity in _renderableEntities)
            {
                var renderableComponent = entity.GetComponent<RenderableComponent>();
                _openGlRenderer.RenderModel(renderableComponent.Model, entity.Transform);
            }

            _openGlRenderer.ProcessRenderCommands();

            _openGlRenderer.RenderText("Test", 100, 100, new Vector3(0.0f, 0.0f, 1.0f));

            _openGlRenderer.FrameEnd();
        }

    }
}