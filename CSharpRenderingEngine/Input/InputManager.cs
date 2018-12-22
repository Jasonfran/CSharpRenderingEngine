using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Engine.Core;
using OpenTK;
using OpenTK.Input;

namespace Engine.Input
{
    public class InputManager : EngineSystem
    {
        private Dictionary<Key, bool> keyStates;
        private Dictionary<Key, bool> wasPressedStates;

        private MouseData mouseData;
        private MouseState previousState;
        private WindowManager _windowManager;
        private bool _centering = false;

        public bool CaptureMouse { get; set; } = true;


        public InputManager(EngineSystemsCollection engineSystems) : base(engineSystems)
        {
            keyStates = new Dictionary<Key, bool>();
            wasPressedStates = new Dictionary<Key, bool>();
            mouseData = new MouseData();
        }

        public override void Init()
        {
            _windowManager = EngineSystems.GetSystem<WindowManager>();
        }

        public void OnKeyDown(KeyboardKeyEventArgs args)
        {
            if (!keyStates.ContainsKey(args.Key) || !keyStates[args.Key])
            {
                wasPressedStates[args.Key] = true;
            }
            keyStates[args.Key] = true;
        }

        public void OnKeyUp(KeyboardKeyEventArgs args)
        {
            keyStates[args.Key] = false;
        }

        public void OnKeyPress(KeyPressEventArgs args)
        {
        }

        public void OnMouseMove(MouseMoveEventArgs args)
        {
            if (CaptureMouse)
            {
                mouseData.MouseX = args.X;
                mouseData.MouseY = args.Y;
            }
        }

        public void ProcessMouseInput()
        {
            if (CaptureMouse)
            {
                var current = Mouse.GetState();
                if (current != previousState)
                {
                    mouseData.MouseX = current.X;
                    mouseData.MouseY = current.Y;
                    mouseData.XDelta = current.X - previousState.X;
                    mouseData.YDelta = current.Y - previousState.Y;
                }
                else
                {
                    mouseData.XDelta = 0;
                    mouseData.YDelta = 0;
                }

                previousState = current;
            }
        }

        public void Update()
        {
            foreach (var key in wasPressedStates.Keys.ToArray())
            {
                wasPressedStates[key] = false;
            }
        }

        public bool KeyPressed(Key key)
        {
            return wasPressedStates.ContainsKey(key) && wasPressedStates[key];
        }

        public bool KeyDown(Key key)
        {
            return keyStates.ContainsKey(key) && keyStates[key];
        }

        public bool KeyUp(Key key)
        {
            return keyStates.ContainsKey(key) && !keyStates[key];
        }

        public MouseData GetMouseData()
        {
            return mouseData;
        }
    }
}