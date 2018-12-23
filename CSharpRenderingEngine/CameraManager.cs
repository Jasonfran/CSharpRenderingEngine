using System;
using Engine.Core;
using Engine.Input;
using Engine.World;
using OpenTK;
using OpenTK.Input;
using NotImplementedException = System.NotImplementedException;

namespace Engine
{
    public class CameraManager : EngineSystem
    {
        private WorldManager _worldManager;
        private InputManager _inputManager;

        public float MouseSensitivity { get; set; } = 4.0f;

        public CameraManager(EngineSystemsCollection engineSystems) : base(engineSystems)
        {

        }

        public override void Init()
        {
            _inputManager = EngineSystems.GetSystem<InputManager>();
            _worldManager = EngineSystems.GetSystem<WorldManager>();
        }

        public void UpdateActiveCamera(float dt)
        {
            var activeCamera = _worldManager.ActiveWorld.ActiveCamera;
            var transform = activeCamera.Transform;
            var rotation = activeCamera.Transform.EularRotation;

            if (_inputManager.KeyDown(Key.W))
            {
                transform.Position += transform.Front * dt;
            }

            if (_inputManager.KeyDown(Key.S))
            {
                transform.Position -= transform.Front * dt;
            }

            if (_inputManager.KeyDown(Key.A))
            {
                transform.Position -= transform.Right * dt;
            }

            if (_inputManager.KeyDown(Key.D))
            {
                transform.Position += transform.Right * dt;
            }

            if (_inputManager.KeyDown(Key.Space))
            {
                transform.Position += Vector3.UnitY * dt;
            }

            if (_inputManager.KeyDown(Key.ControlLeft))
            {
                transform.Position -= Vector3.UnitY * dt;
            }

            if (_inputManager.CaptureMouse)
            {
                var mouseData = _inputManager.GetMouseData();
                var newRotationX = rotation.X + mouseData.YDelta * dt * MouseSensitivity;
                var newRotationY = rotation.Y - mouseData.XDelta * dt * MouseSensitivity;

                if (newRotationX > 85)
                {
                    newRotationX = 85.0f;
                }

                if (newRotationX < -85)
                {
                    newRotationX = -85.0f;
                }

                if (newRotationY > 360 || newRotationY < -360)
                {
                    newRotationY %= 360.0f;
                }

                transform.EularRotation =
                    new Vector3(newRotationX, newRotationY, rotation.Z);
            }
        }
    }
}