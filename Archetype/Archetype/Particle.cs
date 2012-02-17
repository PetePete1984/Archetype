using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Archetype
{
    class Particle
    {
        Color tint;
        TimeSpan lifeTime;
        TimeSpan remainingTime;

        Texture2D texture;
        Vector3 position;
        Vector3 direction;

        float speed;
        float initialSpeed;
        float speedFalloff;
        float size;

        public bool active;
        int currentFrame;
        int frameWidth;
        int frameHeight;

        public Particle()
        {
            tint = Color.White;
            lifeTime = TimeSpan.FromSeconds(8);
            remainingTime = lifeTime;
            size = 1.0f;
            active = true;
            currentFrame = 0;
            frameWidth = 32;
            frameHeight = 32;

        }

        public void Initialize(Texture2D texture, Vector3 position, Vector3 direction, float speed, float speedFalloff)
        {
            this.texture = texture;
            this.position = position;
            this.direction = direction;
            this.speed = speed;
            this.initialSpeed = speed;
            this.speedFalloff = speedFalloff;
        }

        public void Update(GameTime gameTime)
        {
            remainingTime -= TimeSpan.FromSeconds(gameTime.ElapsedGameTime.TotalSeconds);

            if (remainingTime.TotalSeconds <= 0f)
            {
                active = false;
            }

            if (active)
            {
                //tint.A = (byte)(MathHelper.Clamp(remainingTime.TotalSeconds / lifeTime.TotalSeconds * 255, 0, 255));
                speed *= speedFalloff;
                var color = (byte)((speed / initialSpeed) * 255);
                tint = new Color(color, color, color, color);
                //position += direction * speed;
                direction = Vector3.Transform(direction, Matrix.CreateRotationZ(speed / initialSpeed / 20));
                direction.Normalize();
                position.X += (float)(direction.X * speed); 
                position.Y += (float)(direction.Y / 2 * speed);
            }

            if (remainingTime.TotalSeconds <= 0f)
            {
                active = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (active)
                spriteBatch.Draw(texture, new Vector2(position.X, position.Y), null, tint, 0f, Vector2.Zero, speed / initialSpeed, SpriteEffects.None, 0f);

        }

    }


}
