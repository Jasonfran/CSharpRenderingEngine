using System;
using System.Collections.Generic;
using Engine.Core;
using Engine.System;
using OpenTK.Input;
using NotImplementedException = System.NotImplementedException;

namespace Engine.UI
{
    public class UIManager : EngineSystem
    {
        private Dictionary<string, UIScreen> _screens;
        private UIScreen _activeScreen;
        private RenderManager _renderManager;


        public UIManager(EngineSystemsCollection engineSystems) : base(engineSystems)
        {
            _screens = new Dictionary<string, UIScreen>();
        }

        public override void Init()
        {
            _renderManager = EngineSystems.GetSystem<RenderManager>();
        }

        public void AddScreen(string name, UIScreen screen)
        {
            if (_screens.ContainsKey(name))
            {
                throw new Exception("Screen already exists");
            }
            else
            {
                _screens.Add(name, screen);
            }
        }

        public void SetActiveScreen(string name)
        {
            if (_screens.ContainsKey(name))
            {
                _activeScreen = _screens[name];
                return;
            }

            throw new Exception("Screen has not been added!");
        }

        public void Update(float dt)
        {
            _activeScreen.Update(dt);
        }

        public void Render()
        {
            foreach (var uiComponent in _activeScreen.GetComponents())
            {
                uiComponent.Render(_renderManager);
            }
        }

        public void MouseClick(MouseButtonEventArgs mouseButtonEventArgs)
        {
            _activeScreen.MouseClick(mouseButtonEventArgs.Button, mouseButtonEventArgs.X, mouseButtonEventArgs.Y);
        }
    }
}