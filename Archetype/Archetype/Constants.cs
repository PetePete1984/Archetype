using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Archetype
{
    class Constants
    {

        public const float Scale = 100f;
        public static float HalfScreenWidth { get; private set; }
        public static float ScreenWidth { get; private set; }
        public static float HalfScreenHeight { get; private set; }
        public static float ScreenHeight { get; private set; }

        public static void Initialize(GraphicsDevice graphics)
        {
            ScreenHeight = graphics.Viewport.Height;
            ScreenWidth = graphics.Viewport.Width;
            HalfScreenHeight = graphics.Viewport.Height / 2f;
            HalfScreenWidth = graphics.Viewport.Width / 2f;
        }
    }
}
