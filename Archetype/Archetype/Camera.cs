using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Archetype
{
    public class Camera
    {
        private float offsetX;
        private float offsetY;
        private float _zoom = 1.0f;
        private float _rotation;

        private Entity trackingEntity;

        public Matrix TransformationMatrix { get; private set; }
        public float MaxX { get; set; }
        public float MaxY { get; set; }

        public static readonly Camera Current = new Camera();

        public float getX { 
            get { return offsetX; }
             }

        public float getY { 
            get { return offsetY; }
            }
        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        private Camera()
        { }

        public void Update(GameTime gameTime)
        {
            if (trackingEntity != null)
            {
                if (trackingEntity.position.X != Constants.HalfScreenWidth + offsetX)
                {
                    offsetX = MathHelper.Clamp(MathHelper.SmoothStep(offsetX, (trackingEntity.position.X), (float)(gameTime.ElapsedGameTime.TotalSeconds) * 5f), 0, MaxX);
                }
                if (trackingEntity.position.Y != Constants.HalfScreenHeight + offsetY)
                {
                    offsetY = MathHelper.Clamp(MathHelper.SmoothStep(offsetY, (trackingEntity.position.Y), (float)(gameTime.ElapsedGameTime.TotalSeconds) * 5f), 0, MaxY);
                }
            }

            TransformationMatrix = Matrix.CreateTranslation(-offsetX, -offsetY, 0f) *
                                    Matrix.CreateRotationZ(Rotation) *
                                    Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                    Matrix.CreateTranslation(new Vector3(Constants.HalfScreenWidth, Constants.HalfScreenHeight, 0));
        }

        public void StartTracking(Entity entity)
        {
            trackingEntity = entity;
        }

        public void StopTracking()
        {
            trackingEntity = null;
        }

        public Vector2 ScreenToSimulation(Vector2 position)
        {
            return Vector2.Transform(position, Matrix.Invert(TransformationMatrix));
        }
    }
}
