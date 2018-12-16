namespace Engine.Core
{
    public abstract class EngineSystem : IEngineSystem
    {
        public EngineSystemsCollection EngineSystems { get; }

        protected EngineSystem(EngineSystemsCollection engineSystems)
        {
            EngineSystems = engineSystems;
        }

        public abstract void Init();
    }
}