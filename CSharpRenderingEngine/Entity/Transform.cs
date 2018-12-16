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
        private Transform _parentTransform;

        private List<Transform> childrenTransforms = new List<Transform>();

        public Transform ParentTransform
        {
            get => _parentTransform;
            set
            {
                _parentTransform = value;
                _parentTransform.Register(this);
                UpdateTransform();
            }
        }

        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                UpdateTransform();
            }
        }

        public Vector3 EularRotation
        {
            get => _eularRotation;
            set
            {
                _eularRotation = value;
                UpdateTransform();
            }
        }

        public Quaternion RotationQuat
        {
            get => _rotationQuat;
            set
            {
                _rotationQuat = value; 
                UpdateTransform();
            }
        }

        public Vector3 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                UpdateTransform();
            }
        }

        public Matrix4 ModelMatrix { get; private set; }

        public Vector3 Front { get; private set; }
        public Vector3 Right { get; private set; }
        public Vector3 Up { get; private set; }

        private bool Initialised = false;

        public Transform()
        {
            Position = Vector3.Zero;
            EularRotation = Vector3.Zero;
            RotationQuat = Quaternion.FromEulerAngles(EularRotation);
            Scale = Vector3.One;
            Initialised = true;
            UpdateTransform();
        }

        private void UpdateTransform()
        {
            if (Initialised)
            {
                _rotationQuat = Quaternion.FromEulerAngles(_eularRotation);

                ModelMatrix = Matrix4.Identity;

                var translationMatrix = Matrix4.CreateTranslation(_position);
                var rotationXMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(_eularRotation.X));
                var rotationYMatrix = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_eularRotation.Y));
                var rotationZMatrix = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(_eularRotation.Z));

                var scaleMatrix = Matrix4.CreateScale(_scale);

                var rotateTranslateMatrix = rotationXMatrix * rotationYMatrix * rotationZMatrix * translationMatrix;

                Right = -rotateTranslateMatrix.Row0.Xyz;
                Up = rotateTranslateMatrix.Row1.Xyz;
                Front = rotateTranslateMatrix.Row2.Xyz;

                ModelMatrix = scaleMatrix * rotateTranslateMatrix;
                if (ParentTransform != null)
                {
                    ModelMatrix *= ParentTransform.ModelMatrix;
                }

                foreach (var transform in childrenTransforms)
                {
                    transform.UpdateTransform();
                }
            }
        }

        private void Register(Transform transform)
        {
            childrenTransforms.Add(transform);
        }
    }
}