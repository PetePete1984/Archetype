using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Archetype
{
    class Projectile
    {
        public Texture2D texture;
        public Vector2 position;
        public Vector2 direction;

        public int currentFrame;
        public int frameWidth;
        public int frameHeight;

        public Vector2 origin;

        public float speed;
        public Rectangle boundingBox;

        public Projectile()
        { }

        public void Initialize()
        { }

        public void Update()
        { }

        public void Draw()
        { }

    }


}
