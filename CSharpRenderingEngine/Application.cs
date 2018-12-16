namespace Engine
{
    class Application
    {
        private static Core.Engine engine;
        static void Main(string[] args)
        {
            engine = new Core.Engine();
            engine.Init(1920, 1080, "Test");
            engine.Go();
        }
    }
}
