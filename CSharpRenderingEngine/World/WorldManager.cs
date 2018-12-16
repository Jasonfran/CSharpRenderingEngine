using System;
using System.Collections.Generic;
using Engine.Core;

namespace Engine.World
{
    public class WorldManager : EngineSystem, IWorldManager
    {
        private readonly EngineSystemsCollection _engineSystems;
        public List<World> Worlds { get; set; }
        public World ActiveWorld { get; set; }

        public WorldManager(EngineSystemsCollection engineSystems) : base(engineSystems)
        {
            _engineSystems = engineSystems;
            Worlds = new List<World>();
        }


        public override void Init()
        {
        }

        public World NewWorld()
        {
            var world = new World();
            Worlds.Add(world);
            return world;
        }

        public void SetActiveWorld(World world)
        {
            if (Worlds.Contains(world))
            {
                ActiveWorld = world;
            }
            else
            {
                throw new Exception("Cannot set active world to unregistered world");
            }
        }
    }
}