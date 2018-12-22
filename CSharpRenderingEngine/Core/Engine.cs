using System.Drawing;
using Engine.Entity;
using Engine.Game;
using Engine.Input;
using Engine.Renderer;
using Engine.Resources;
using Engine.System;
using Engine.World;
using OpenTK;
using OpenTK.Input;

namespace Engine.Core
{
    public class Engine
    {
        private Game.Game _game;

        private readonly EngineSystemsCollection _engineSystems;
        private WindowManager _windowManager;
        private InputManager _inputManager;

        public Engine()
        {
            _engineSystems = new EngineSystemsCollection();
        }

        private void InitSystems(int width, int height, string title)
        {
            _engineSystems.AddSystem(new InputManager(_engineSystems));
            _engineSystems.AddSystem(new ResourceManager(_engineSystems));
            _engineSystems.AddSystem(new EntityManager(_engineSystems));
            _engineSystems.AddSystem(new WorldManager(_engineSystems));
            _engineSystems.AddSystem(new CameraManager(_engineSystems));
            _engineSystems.AddSystem(new WindowManager(_engineSystems));
            _engineSystems.AddSystem(new TransformManager(_engineSystems));
            _engineSystems.AddSystem(new RenderManager(_engineSystems));
            _engineSystems.AddSystem(new OpenGLRendererCore(_engineSystems));

            _windowManager = _engineSystems.GetSystem<WindowManager>();
            _windowManager.NewWindow(width, height, title);

            _engineSystems.InitAll();
        }

        public void Init(int width, int height, string title)
        {
            InitSystems(width, height, title);

            _inputManager = _engineSystems.GetSystem<InputManager>();

            _windowManager.GetActiveWindow().TargetRenderFrequency = 60.0;
            _windowManager.GetActiveWindow().TargetUpdateFrequency = 60.0;
            _windowManager.GetActiveWindow().UpdateFrame += WindowOnUpdateFrame;
            _windowManager.GetActiveWindow().RenderFrame += WindowOnRenderFrame;

            _windowManager.GetActiveWindow().KeyDown += (sender, args) => { _inputManager.OnKeyDown(args); };

            _windowManager.GetActiveWindow().KeyUp += (sender, args) => { _inputManager.OnKeyUp(args); };

            _windowManager.GetActiveWindow().KeyPress += (sender, args) => { _inputManager.OnKeyPress(args); };

            _windowManager.GetActiveWindow().MouseMove += (sender, args) => { _inputManager.OnMouseMove(args); };

            _windowManager.GetActiveWindow().CursorVisible = false;
        }

        private void WindowOnUpdateFrame(object sender, FrameEventArgs e)
        {
            _inputManager.ProcessMouseInput();
            _engineSystems.GetSystem<CameraManager>().UpdateActiveCamera((float)e.Time);
            _game.UpdateGame((float)e.Time);

            if (_inputManager.KeyPressed(Key.Escape))
            {
                _windowManager.GetActiveWindow().Close();
            }

            if (_inputManager.KeyPressed(Key.F2))
            {
                _windowManager.GetActiveWindow().CursorVisible = !_windowManager.GetActiveWindow().CursorVisible;
                _inputManager.CaptureMouse = !_inputManager.CaptureMouse;
            }
            _inputManager.Update();
        }

        private void WindowOnRenderFrame(object sender, FrameEventArgs e)
        {
            _engineSystems.GetSystem<TransformManager>().UpdateCameraTransforms();
            _engineSystems.GetSystem<TransformManager>().UpdateEntityTransforms();
            _engineSystems.GetSystem<RenderManager>().Render();
            _windowManager.GetActiveWindow().Title = $"{_windowManager.Title} - Render time: {e.Time * 1000:F2}ms - Framerate: {_windowManager.GetActiveWindow().RenderFrequency:F2}";
        }


        public void Go()
        {
            _game = new MyGame(_engineSystems, new GameSystemCollection());
            _game.Init();

            _windowManager.GetActiveWindow().Run();
        }
    }
}