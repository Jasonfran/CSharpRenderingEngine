using Engine.Core;
using OpenTK;
using OpenTK.Graphics;

namespace Engine
{
    public class WindowManager : EngineSystem
    {
        private GameWindow _activeWindow;
        public string Title { get; set; }

        public WindowManager(EngineSystemsCollection engineSystems) : base(engineSystems)
        {
        }

        public override void Init()
        {
            
        }

        public GameWindow NewWindow(int width, int height, string title)
        {
            Title = title;
            var window = new GameWindow(width, height, GraphicsMode.Default, title);
            _activeWindow = window;
            return window;
        }

        public GameWindow GetActiveWindow()
        {
            return _activeWindow;
        }
    }
}