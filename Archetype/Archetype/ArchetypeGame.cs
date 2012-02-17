using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SpriteSheetRuntime;
using ArchetypeEngine;

namespace Archetype
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ArchetypeGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont characterFont;
        Texture2D grassTile;

        Texture2D mouseCursor;
        Texture2D fireballTexture;

        Texture2D projectile;

        SpriteSheet sprites;

        Entity player = new ShootingEntity();

        // Keyboard states used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        // Gamepad states used to determine button presses
        GamePadState currentGamePadState;
        GamePadState previousGamePadState;

        MouseState currentMouseState;
        MouseState previousMouseState;

        Vector3 playerWalkTarget = new Vector3(-1, -1, 0);
        SpriteMap CF = new SpriteMap();

        List<List<Particle>> ParticleSystems = new List<List<Particle>>();

        Random rng = new Random();

        float frameBuffer = 0f;
        float targetZoom = 2.0f;
        int frame = 0;

        public ArchetypeGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //this.IsMouseVisible = true;
            //this.
            player.position = new Vector3(1200f, 1600f, 0f);

            Constants.Initialize(graphics.GraphicsDevice);
            SpriteMap.gfx = graphics.GraphicsDevice;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            sprites = Content.Load<SpriteSheet>("PlayerSprites");
            Camera.Current.MaxX = 30000;
            Camera.Current.MaxY = 20000;
            Camera.Current.Zoom = targetZoom;
            Camera.Current.StartTracking(player);

            characterFont = Content.Load<SpriteFont>("Courier");

            grassTile = Content.Load<Texture2D>("grass");
            fireballTexture = Content.Load<Texture2D>("fireball");

            mouseCursor = Content.Load<Texture2D>("arrow");

            projectile = Content.Load<Texture2D>("projectile");

            player.spriteRect = new Rectangle(0, 0, 16, 16);
            player.spritebase = "player";
            CF.Initialize(Content.Load<Texture2D>("CF/cannonfodder1_sprites_grass2"), 16, 14);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            // Save the previous state of the keyboard and game pad so we can determine single key/button presses
            previousGamePadState = currentGamePadState;
            previousKeyboardState = currentKeyboardState;

            previousMouseState = currentMouseState;

            // Read the current state of the keyboard and gamepad and store it
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            currentMouseState = Mouse.GetState();

            float playerMoveSpeed = 4f;
            Vector3 playerMovement = Vector3.Zero;

            if (this.IsActive)
            {
                // Get Thumbstick Controls
                playerMovement.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
                playerMovement.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;

                if (playerMovement.Length() > 0f)
                    playerWalkTarget = new Vector3(-1, -1, 0);

                // Use the Keyboard / Dpad
                if (currentKeyboardState.IsKeyDown(Keys.Left) ||
                currentGamePadState.DPad.Left == ButtonState.Pressed)
                {
                    playerMovement.X = -1;
                }
                if (currentKeyboardState.IsKeyDown(Keys.Right) ||
                currentGamePadState.DPad.Right == ButtonState.Pressed)
                {
                    playerMovement.X = 1;
                }
                if (currentKeyboardState.IsKeyDown(Keys.Up) ||
                currentGamePadState.DPad.Up == ButtonState.Pressed)
                {
                    playerMovement.Y = -1;
                }
                if (currentKeyboardState.IsKeyDown(Keys.Down) ||
                currentGamePadState.DPad.Down == ButtonState.Pressed)
                {
                    playerMovement.Y = 1;
                }

                if (playerMovement.Length() > 0f)
                    playerWalkTarget = new Vector3(-1, -1, 0);

                if (currentMouseState.X + currentMouseState.Y > 0)
                {
                    var MouseToSim = Vector3.Transform(new Vector3(currentMouseState.X, currentMouseState.Y, 0), Matrix.Invert(Camera.Current.TransformationMatrix));

                    //get Mouse controls
                    if (previousMouseState.LeftButton == ButtonState.Pressed)
                    {
                        playerWalkTarget = MouseToSim;
                        /*playerWalkTarget.X = (float)Math.Round(playerWalkTarget.X, 0);
                        playerWalkTarget.Y = (float)Math.Round(playerWalkTarget.Y, 0);*/
                    }

                    if (previousMouseState.RightButton == ButtonState.Pressed)
                    {
                        if (currentMouseState.RightButton == ButtonState.Released)
                        {
                            SpawnFireNova(player, gameTime);
                        }
                    }
                }

            }
            if (playerWalkTarget.X != -1 && playerWalkTarget.Y != -1)
            {
                var playerDirection = playerWalkTarget - player.position;
                if (playerDirection.Length() > playerMoveSpeed)
                {
                    playerDirection.Normalize();
                    playerMovement = playerDirection;
                }
                else
                {
                    playerWalkTarget = new Vector3(-1, -1, 0);
                }
            }

            if (playerMovement.Length() > 0f)
            {
                playerMovement.Normalize();
                //Store current Direction
                player.direction = playerMovement;
            }

            player.Update(gameTime);

            playerMovement *= playerMoveSpeed;

            player.position += playerMovement;

            // Make sure that the player does not go out of bounds
            player.position = new Vector3(MathHelper.Clamp(player.position.X, 0, 30000 - 16), MathHelper.Clamp(player.position.Y, 0, 30000 - 16), 0);

            foreach (var lP in ParticleSystems)
            {
                if (lP.Count > 0)
                {
                    foreach (var P in lP)
                    {
                        P.Update(gameTime);
                    }
                    lP.RemoveAll(xpart => xpart.active == false);
                }
            }
            ParticleSystems.RemoveAll(ps => ps.Count == 0);

            float wheelDelta = (currentMouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue) / 120;
            targetZoom += wheelDelta * 0.1f;
            if (targetZoom < 0.1f)
                targetZoom = 0.1f;
            Camera.Current.Zoom = MathHelper.Lerp(Camera.Current.Zoom, targetZoom, 0.05f);

            Camera.Current.Update(gameTime);

            frameBuffer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (frameBuffer >= 150f / 1000f)
            {
                frame = (frame + 1) % 100;
                frameBuffer = 0;
            }

            base.Update(gameTime);
        }

        public void SpawnFireNova(Entity caster, GameTime gameTime)
        {
            var particles = new List<Particle>(36);
            var variation = rng.Next(0, 11);
            for (int i = 0; i < 36; i++)
            {
                Particle fireball = new Particle();
                Vector3 particleDirection = new Vector3((float)Math.Cos(MathHelper.ToRadians(i * 10 + variation)), (float)Math.Sin(MathHelper.ToRadians(i * 10 + variation)), 0);
                particleDirection.Normalize();
                fireball.Initialize(fireballTexture, caster.position, particleDirection, 12f, 0.99f);
                particles.Add(fireball);
            }

            ParticleSystems.Add(particles);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            /*SamplerState ss = new SamplerState();
            ss.*/
            GraphicsDevice.Clear(Color.DarkGreen);

            // TODO: Add your drawing code here
            spriteBatch.Begin(0, null, SamplerState.PointClamp, null, null, null, Camera.Current.TransformationMatrix);

            bool odd = true;
            int offset = 0;

            for (int y = 0; y <= 100; y++)
            {
                if (odd)
                {
                    offset = -64;
                    odd = false;
                }
                else
                {
                    offset = 0;
                    odd = true;
                }
                for (int x = 0; x <= 100; x++)
                {
                    int drawX = 0;
                    int drawY = 0;
                    drawX = (int)(Constants.HalfScreenWidth - grassTile.Width / 2) + x * grassTile.Width / 2 - y * grassTile.Width / 2;
                    drawY = (int)x * grassTile.Width / 4 + y * grassTile.Width / 4;
                    spriteBatch.Draw(grassTile, new Vector2(drawX, drawY), new Rectangle(0, 64, 128, 64), Color.White);
                    //spriteBatch.DrawString(characterFont, drawX + ", " + drawY, new Vector2(drawX, drawY), Color.Wheat);
                }
            }

            foreach (var lP in ParticleSystems)
                foreach (var P in lP)
                    P.Draw(spriteBatch);

            var playerRect = CF.getFrame(player.spritebase + player.sDirection, frame);
            spriteBatch.Draw(CF.texture,
                new Vector2(player.position.X, player.position.Y),
                playerRect,
                Color.White);
            //var playerRect = new Rectangle(sprites.SourceRectangle("sprites").X + player.spriteRect.X, sprites.SourceRectangle("sprites").Y + player.spriteRect.Y, player.spriteRect.Width, player.spriteRect.Height);
            /*spriteBatch.Draw(sprites.Texture,
                player.Position, 
                playerRect, 
                Color.White);*/
            spriteBatch.End();

            //Non-Offset Text drawing
            spriteBatch.Begin();
            spriteBatch.DrawString(characterFont, String.Format("CameraX: {0} ## CameraY: {1}", Camera.Current.getX, Camera.Current.getY), new Vector2(20, Constants.ScreenHeight - 20), Color.White);
            spriteBatch.DrawString(characterFont, String.Format("PlayerX: {0} ## PlayerY: {1}", player.position.X, player.position.Y), new Vector2(20, Constants.ScreenHeight - 45), Color.White);
            var MouseToSim = Vector2.Transform(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Matrix.Invert(Camera.Current.TransformationMatrix));
            spriteBatch.DrawString(characterFont, String.Format("MouseX: {0} ## MouseY: {1}", MouseToSim.X, MouseToSim.Y), new Vector2(20, Constants.ScreenHeight - 70), Color.White);
            //Cursor
            spriteBatch.Draw(mouseCursor, new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Color.Cyan);
            spriteBatch.End();
            base.Draw(gameTime);
            /*
            for (int y = 0; y < Layers[LayerName].Height; y++) 
            { 
                for (int x = 0; x < Layers[LayerName].Width; x++) 
                { 
                    Rectangle DstRect = new Rectangle(); 
                    DstRect = GetDestRect(x, y); 
                    int id = Layers[LayerName][x, y]; 
                    if (id > 0) 
                    { 
                        int set = GetTileSetID(id); 
                        batch.Draw(TileSets[set].TileSheet.Texture, DstRect, TileSets[set].TileSheet[id], Color.White); 
                    } 
                } 
            }
             */
        }
    }
}
