using System.Collections.Generic;

namespace Engine.World
{
    public interface IWorldManager
    {
        List<World> Worlds { get; }
        World ActiveWorld { get; }
        World NewWorld();
        void SetActiveWorld(World world);
    }
}