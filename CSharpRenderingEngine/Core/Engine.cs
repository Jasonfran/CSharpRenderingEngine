using Engine.Entity;
using Engine.Game;
using Engine.Renderer;
using Engine.Resources;
using Engine.World;
using OpenTK;

namespace Engine.Core
{
    public class Engine
    {
        private Game.Game _game;

        private readonly EngineSystemsCollection _engineSystems;
        private WindowManager _windowManager;

        public Engine()
        {
            _engineSystems = new EngineSystemsCollection();
        }

        private void InitSystems(int width, int height, string title)
        {
            _engineSystems.AddSystem(new ResourceManager(_engineSystems));
            _engineSystems.AddSystem(new EntityManager(_engineSystems));
            _engineSystems.AddSystem(new WorldManager(_engineSystems));
            _engineSystems.AddSystem(new RenderManager(_engineSystems));
            _engineSystems.AddSystem(new WindowManager(_engineSystems));

            _windowManager = _engineSystems.GetSystem<WindowManager>();
            _windowManager.NewWindow(width, height, title);

            _engineSystems.InitAll();
        }

        public void Init(int width, int height, string title)
        {
            InitSystems(width, height, title);


            _windowManager.GetActiveWindow().TargetRenderFrequency = 60.0;
            _windowManager.GetActiveWindow().TargetUpdateFrequency = 60.0;
            _windowManager.GetActiveWindow().UpdateFrame += WindowOnUpdateFrame;
            _windowManager.GetActiveWindow().RenderFrame += WindowOnRenderFrame;
        }

        private void WindowOnUpdateFrame(object sender, FrameEventArgs e)
        {
            _game.UpdateGame((float)e.Time);
        }

        private void WindowOnRenderFrame(object sender, FrameEventArgs e)
        {
            _engineSystems.GetSystem<RenderManager>().Render();
            _windowManager.GetActiveWindow().Title = $"{_windowManager.Title} - Render time: {e.Time * 1000:F2}ms - Framerate: {_windowManager.GetActiveWindow().RenderFrequency:F2}";
            _windowManager.GetActiveWindow().SwapBuffers();
        }


        public void Go()
        {
            _game = new MyGame(_engineSystems, new GameSystemCollection());
            _game.Init();

            _windowManager.GetActiveWindow().Run();
        }
    }
}