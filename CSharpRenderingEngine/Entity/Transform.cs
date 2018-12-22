using System.Collections.Generic;
using OpenTK;

namespace Engine.Entity
{
    public class Transform
    {
        private Vector3 _position;
        private Vector3 _eularRotation;
        private Quaternion _rotationQuat;
        private Vector3 _scale;

        public bool IsDirty { get; set; }

        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                IsDirty = true;
            }
        }

        public Vector3 EularRotation
        {
            get => _eularRotation;
            set
            {
                _eularRotation = value;
                IsDirty = true;
            }
        }

        public Quaternion RotationQuat
        {
            get => _rotationQuat;
            set
            {
                _rotationQuat = value;
                IsDirty = true;
            }
        }

        public Vector3 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                IsDirty = true;
            }
        }

        public Matrix4 ModelMatrix { get; set; }

        public Vector3 Front { get; set; }
        public Vector3 Right { get; set; }
        public Vector3 Up { get; set; }

        public Transform()
        {
            Position = Vector3.Zero;
            EularRotation = Vector3.Zero;
            RotationQuat = Quaternion.FromEulerAngles(EularRotation);
            Scale = Vector3.One;
        }
    }
}