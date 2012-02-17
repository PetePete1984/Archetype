using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Archetype
{
    class ShootingEntity : Entity
    {
        // The rate of fire of the Entity
        TimeSpan fireTime;
        TimeSpan previousFireTime;

        Projectile Bullet { get; set; }

        public void Initialize(TimeSpan fireTime)
        {
            this.fireTime = fireTime;
        }

        public void Update(GameTime gameTime)
        {
            // Fire only every interval we set as the fireTime
            if (gameTime.TotalGameTime - this.previousFireTime > this.fireTime)
            {
                // Reset our current time
                this.previousFireTime = gameTime.TotalGameTime;
            }
        }
    }
}
