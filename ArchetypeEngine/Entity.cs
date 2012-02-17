using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Archetype
{
    public class Entity
    {
        public Vector3 position;  //position in 3d space
        float height;               //virtual object height (z-axis up)

        public Vector2 origin;      //"footpoint" at center / bottom of object
        public Vector3 direction;

        public float speed;
        public float maxSpeed;

        public float scale;
        public float defaultScale;

        public string sDirection = "S";
        string sVertical = "";
        string sHorizontal = "";
        public string spritebase;


        public Texture2D texture { get; set; }
        public Texture2D normalmap { get; set; }
        public Texture2D heightmap { get; set; }

        public Rectangle spriteRect;
        public Vector2 spriteOffset;

        public virtual void Update(GameTime gameTime)
        {
            var vertical = direction.Y;
            var horizontal = direction.X;
            sHorizontal = sVertical = "";
            if (vertical != 0 || horizontal != 0)
            {
                if (vertical > 0.5)
                    sVertical = "S";
                else if (vertical < -0.5)
                    sVertical = "N";

                if (horizontal > 0.5)
                    sHorizontal = "E";
                else if (horizontal < -0.5)
                    sHorizontal = "W";


                sDirection = sVertical + sHorizontal;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        { }
    }
}
