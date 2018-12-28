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
        private RenderManager _renderManager;
        private TransformManager _transformManager;

        private GameWindow _activeWindow;

        public Engine()
        {
            _engineSystems = new EngineSystemsCollection();
        }

        private void InitSystems(int width, int height, string title)
        {
            _engineSystems.AddSystem(new Logger(_engineSystems));
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

            _renderManager = _engineSystems.GetSystem<RenderManager>();

            _transformManager = _engineSystems.GetSystem<TransformManager>();

            _renderManager = _engineSystems.GetSystem<RenderManager>();

            _activeWindow = _windowManager.GetActiveWindow();

            _activeWindow.TargetRenderFrequency = 60.0;
            _activeWindow.TargetUpdateFrequency = 60.0;
            _activeWindow.UpdateFrame += WindowOnUpdateFrame;
            _activeWindow.RenderFrame += WindowOnRenderFrame;

            _activeWindow.KeyDown += (sender, args) => { _inputManager.OnKeyDown(args); };

            _activeWindow.KeyUp += (sender, args) => { _inputManager.OnKeyUp(args); };

            _activeWindow.KeyPress += (sender, args) => { _inputManager.OnKeyPress(args); };

            _activeWindow.MouseMove += (sender, args) => { _inputManager.OnMouseMove(args); };

            _activeWindow.CursorVisible = false;

            _activeWindow.TargetRenderFrequency = 500f;
            _activeWindow.VSync = VSyncMode.Off;
        }

        private void WindowOnUpdateFrame(object sender, FrameEventArgs e)
        {
            _inputManager.ProcessMouseInput();
            _engineSystems.GetSystem<CameraManager>().UpdateActiveCamera((float)e.Time);
            _game.UpdateGame((float)e.Time);

            if (_inputManager.KeyPressed(Key.Escape))
            {
                _activeWindow.Close();
            }

            if (_inputManager.KeyPressed(Key.F2))
            {
                _activeWindow.CursorVisible = !_activeWindow.CursorVisible;
                _inputManager.CaptureMouse = !_inputManager.CaptureMouse;
            }

            if (_inputManager.KeyPressed(Key.F3))
            {
                _renderManager.Debug = !_renderManager.Debug;
            }

            _inputManager.Update();
        }

        private void WindowOnRenderFrame(object sender, FrameEventArgs e)
        {
            _transformManager.UpdateCameraTransforms();
            _transformManager.UpdateEntityTransforms();
            _renderManager.Render();
            _activeWindow.Title = $"{_windowManager.Title} - Render time: {e.Time * 1000:F2}ms - Framerate: {_activeWindow.RenderFrequency:F2}";
        }


        public void Go()
        {
            _game = new MyGame(_engineSystems, new GameSystemCollection());
            _game.Init();

            _activeWindow.Run();
        }
    }
}