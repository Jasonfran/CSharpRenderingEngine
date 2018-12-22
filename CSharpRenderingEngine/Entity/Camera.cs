using System;
using OpenTK;

namespace Engine.Entity
{
    public class Camera
    {
        public Transform Transform { get; }

        public Camera(Vector3 position)
        {
            Transform = new Transform();
            Transform.Position = position;
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Transform.Position, Transform.Position + Transform.Front, Vector3.UnitY);
        }
    }
}