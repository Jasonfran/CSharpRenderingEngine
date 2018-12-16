namespace Engine.Core
{
    public interface IEngineSystem
    {
        EngineSystemsCollection EngineSystems { get; }

        void Init();
    }
}