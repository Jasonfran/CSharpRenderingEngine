using System;
using System.Collections.Generic;
using Engine.Entity;
using Engine.Lighting;

namespace Engine.World
{
    public class World
    {
        public List<EntityManager.Entity> ChildEntities { get; }
        public List<Camera> Cameras { get; }
        public Camera ActiveCamera { get; private set; }
        public List<PointLight> PointLights { get; }


        public World()
        {
            ChildEntities = new List<EntityManager.Entity>();
            Cameras = new List<Camera>();
            PointLights = new List<PointLight>();
        }

        public void AddChild(EntityManager.Entity entity)
        {
            ChildEntities.Add(entity);
        }

        public Camera AddCamera(Camera camera)
        {
            if (!Cameras.Contains(camera))
            {
                Cameras.Add(camera);
            }

            return camera;
        }

        public void SetActiveCamera(Camera camera)
        {
            if (!Cameras.Contains(camera))
            {
                throw new Exception("Camera must be added to world before being set as active");
            }
            ActiveCamera = camera;
        }

        public PointLight AddPointLight(PointLight light)
        {
            if (!PointLights.Contains(light))
            {
                PointLights.Add(light);
            }
            return light;
        }
    }
}