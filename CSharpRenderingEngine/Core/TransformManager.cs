using System;
using System.Runtime.InteropServices;
using Engine.Entity;
using Engine.World;
using OpenTK;
using NotImplementedException = System.NotImplementedException;

namespace Engine.Core
{
    public class TransformManager : EngineSystem
    {
        private WorldManager _worldManager;

        public TransformManager(EngineSystemsCollection engineSystems) : base(engineSystems)
        {

        }

        public override void Init()
        {
            _worldManager = EngineSystems.GetSystem<WorldManager>();
        }

        public void UpdateCameraTransforms()
        {
            var activeCamera = _worldManager.ActiveWorld.ActiveCamera;
            var transform = activeCamera.Transform;
            if (transform.IsDirty)
            {
                var rotationQuat = Quaternion.FromEulerAngles(transform.EularRotation);

                var translationMatrix = Matrix4.CreateTranslation(transform.Position);
                var rotationXMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(transform.EularRotation.X));
                var rotationYMatrix = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(transform.EularRotation.Y));
                var rotationZMatrix = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(transform.EularRotation.Z));

                var scaleMatrix = Matrix4.CreateScale(transform.Scale);

                var rotateTranslateMatrix = rotationXMatrix * rotationYMatrix * rotationZMatrix * translationMatrix;

                var right = -rotateTranslateMatrix.Row0.Xyz;
                var up = rotateTranslateMatrix.Row1.Xyz;
                var front = rotateTranslateMatrix.Row2.Xyz;

                var modelMatrix = scaleMatrix * Matrix4.CreateFromQuaternion(rotationQuat) * translationMatrix;

                transform.RotationQuat = rotationQuat;
                transform.ModelMatrix = modelMatrix;
                transform.Right = right;
                transform.Up = up;
                transform.Front = front;

                transform.IsDirty = false;
            }
        }

        public void UpdateEntityTransforms()
        {
            var currentWorld = _worldManager.ActiveWorld;

            foreach (var entity in currentWorld.ChildEntities)
            {
                UpdateEntityTransform(entity);
            }
        }

        private void UpdateEntityTransform(EntityManager.Entity entity)
        {
            if (entity.Transform.IsDirty)
            {
                var transform = entity.Transform;
                var rotationQuat = Quaternion.FromEulerAngles(transform.EularRotation);

                var modelMatrix = Matrix4.Identity;

                var translationMatrix = Matrix4.CreateTranslation(transform.Position);
                var rotationXMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(transform.EularRotation.X));
                var rotationYMatrix = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(transform.EularRotation.Y));
                var rotationZMatrix = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(transform.EularRotation.Z));

                var scaleMatrix = Matrix4.CreateScale(transform.Scale);

                var rotateTranslateMatrix = rotationXMatrix * rotationYMatrix * rotationZMatrix * translationMatrix;

                var right = -rotateTranslateMatrix.Row0.Xyz;
                var up = rotateTranslateMatrix.Row1.Xyz;
                var front = rotateTranslateMatrix.Row2.Xyz;

                modelMatrix = scaleMatrix * rotateTranslateMatrix;

                if (entity.Parent != null)
                {
                    modelMatrix *= entity.Parent.Transform.ModelMatrix;
                }

                entity.Transform.RotationQuat = rotationQuat;
                entity.Transform.ModelMatrix = modelMatrix;
                entity.Transform.Right = right;
                entity.Transform.Up = up;
                entity.Transform.Front = front;

                entity.Transform.IsDirty = false;
            }

            foreach (var entityChild in entity.Children)
            {
                UpdateEntityTransform(entityChild);
            }
        }
    }
}