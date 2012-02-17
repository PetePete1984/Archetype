using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArchetypeEngine
{
    public class SpriteMap
    {
        public Texture2D texture;
        public int spriteWidth;
        public int spriteHeight;
        public int xAmount;
        public int yAmount;
        public List<Sprite> sprites;
        public static GraphicsDevice gfx;

        public void Initialize(Texture2D texture, int spriteWidth, int spriteHeight)
        {
            this.texture = texture;
            this.spriteHeight = spriteHeight;
            this.spriteWidth = spriteWidth;
            this.xAmount = texture.Width / spriteWidth;
            this.yAmount = texture.Height / spriteHeight;
            sprites = new List<Sprite>();
            initCF();
        }

        public Rectangle getFrame(string spriteName, int frame)
        {
            var s = (from spr in sprites
                     where spr.name.Equals(spriteName)
                     select spr).First();

            if (s != null)
            {
                var xPos = ((s.index + (s.animation[frame % s.length])) % xAmount) * spriteWidth;
                var yPos = (int)((s.index + (s.animation[frame % s.length])) / xAmount) * spriteHeight;
                return (new Rectangle(xPos, yPos, spriteWidth, spriteHeight));

            }
            else
                return new Rectangle(0, 0, spriteWidth, spriteHeight);
        }

        public void initCF()
        {
            var walkAnim = new int[] { 0, 1, 2, 1 };
            var grenAnim = new int[] { 0, 1, 2 };
            var trenchAnim = new int[] { 0 };
            var swimAnim = new int[] { 0, 1 };
            var deathAnim = swimAnim;
            var turretAnim = trenchAnim;
            var wildDeathAnim = new int[] { 0,1,2,3,4,5,6,7,8};




            sprites.Add(new Sprite { name = "playerS", index = 0, length = 4, animation = new int[] { 0, 1, 2, 1 } });
            sprites.Add(new Sprite { name = "playerSW", index = 3, length = 4, animation = new int[] { 0, 1, 2, 1 } });
            sprites.Add(new Sprite { name = "playerW", index = 6, length = 4, animation = new int[] { 0, 1, 2, 1 } });
            sprites.Add(new Sprite { name = "playerNW", index = 9, length = 4, animation = new int[] { 0, 1, 2, 1 } });
            sprites.Add(new Sprite { name = "playerN", index = 12, length = 4, animation = new int[] { 0, 1, 2, 1 } });
            sprites.Add(new Sprite { name = "playerNE", index = 15, length = 4, animation = new int[] { 0, 1, 2, 1 } });
            sprites.Add(new Sprite { name = "playerE", index = 18, length = 4, animation = new int[] { 0, 1, 2, 1 } });
            sprites.Add(new Sprite { name = "playerSE", index = 21, length = 4, animation = new int[] { 0, 1, 2, 1 } });

            sprites.Add(new Sprite { name = "playerGrenadeS", index = 24, length = 4, animation = new int[] { 0, 1, 2, 1 } });
            sprites.Add(new Sprite { name = "playerGrenadeSW", index = 27, length = 4, animation = new int[] { 0, 1, 2, 1 } });
            sprites.Add(new Sprite { name = "playerGrenadeW", index = 30, length = 4, animation = new int[] { 0, 1, 2, 1 } });
            sprites.Add(new Sprite { name = "playerGrenadeNW", index = 33, length = 4, animation = new int[] { 0, 1, 2, 1 } });
            sprites.Add(new Sprite { name = "playerGrenadeN", index = 36, length = 4, animation = new int[] { 0, 1, 2, 1 } });
            sprites.Add(new Sprite { name = "playerGrenadeNE", index = 39, length = 4, animation = new int[] { 0, 1, 2, 1 } });
            sprites.Add(new Sprite { name = "playerGrenadeE", index = 42, length = 4, animation = new int[] { 0, 1, 2, 1 } });
            sprites.Add(new Sprite { name = "playerGrenadeSE", index = 45, length = 4, animation = new int[] { 0, 1, 2, 1 } });

        }
    }
}
