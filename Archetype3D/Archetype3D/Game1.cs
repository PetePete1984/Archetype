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

namespace Archetype3D
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        GraphicsDevice device;
        SpriteBatch spriteBatch;

        BasicEffect basicEffect;
        Texture2D druidTexture;

        Texture2D[] tower;
        Model paddle;
        int[,] bricks;
        RenderTarget2D towerCombined;

        Matrix world, view, projection;
        VertexPositionColorTexture[] vertices;
        int[] indices;

        Random random;

        Vector3 paddlePos;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
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
            base.Initialize();
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            device = graphics.GraphicsDevice;
            spriteBatch = new SpriteBatch(device);
            basicEffect = new BasicEffect(device);
            druidTexture = Content.Load<Texture2D>("druid");
            tower = new Texture2D[3];
            tower[0] = Content.Load<Texture2D>("tower2_albedo");
            tower[1] = Content.Load<Texture2D>("tower2_normal");
            tower[2] = Content.Load<Texture2D>("tower2_height");
            InitBricks();
            paddle = Content.Load<Model>("Paddle");
            paddlePos = Vector3.Zero;

            towerCombined = new RenderTarget2D(device, tower[0].Width, tower[0].Height);
            world = Matrix.Identity;
            //projection = Matrix.CreateOrthographic(-10, -10, 1, 50);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, device.Viewport.AspectRatio, 1f, 300f);
            view = Matrix.CreateLookAt(new Vector3(0, 30, -10), new Vector3(0, -80, -40), Vector3.Up);

            /*view = new Matrix(
                1.0f, 0.0f, 0.0f, 0.0f,
                0.0f, -1.0f, 0.0f, 0.0f,
                0.0f, 0.0f, -1.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f
            );*/
            //Matrix projection = Matrix.CreateOrthographicOffCenter(0, this.Width, -this.Height, 0, 0, 1);
            random = new Random();
            initVertices();
            // TODO: use this.Content to load your game content here
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
            paddlePos.X += GamePad.GetState(0).ThumbSticks.Left.X;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            device.Clear(Color.CornflowerBlue);
            RasterizerState rasterizerState1 = new RasterizerState();
            rasterizerState1.CullMode = CullMode.None;
            graphics.GraphicsDevice.RasterizerState = rasterizerState1;
            // TODO: Add your drawing code here
            //basicEffect.EnableDefaultLighting();
            basicEffect.World = world;
            //basicEffect.World = world;// *Matrix.CreateTranslation(0, 0, 0);
            basicEffect.View = view;
            basicEffect.Projection = projection;
            basicEffect.VertexColorEnabled = true;
            basicEffect.EmissiveColor = new Vector3(1.0f, 0f, 0f);
            basicEffect.TextureEnabled = true;
            basicEffect.Techniques[0].Passes[0].Apply();
            basicEffect.Texture = tower[1];
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //device.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, vertices, 0, 8, indices, 0, 12);

                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 40; j++)
                    {
                        if (bricks[i, j] == 1)
                        {
                            paddle.Draw(world * Matrix.CreateTranslation(new Vector3(2*i-10, 0, -j)), view, projection);
                            //device.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, vertices, 0, 8, indices, 0, 12);
                        }
                    }
                }
            }
            paddle.Draw(world * Matrix.CreateRotationY(MathHelper.ToRadians(90f)) * Matrix.CreateTranslation(paddlePos), view, projection);
            //DrawModel(paddle);


            base.Draw(gameTime);
        }

        private void InitBricks()
        {
            bricks = new int[10, 40];
            bricks[1, 25] = 1;
            bricks[2, 26] = 1;
            bricks[3, 25] = 1;
            bricks[4, 26] = 1;
            bricks[5, 25] = 1;
            bricks[6, 26] = 1;
            bricks[7, 25] = 1;
            bricks[8, 26] = 1;
        }

        private void initVertices()
        {
            vertices = new VertexPositionColorTexture[8];

            vertices[0] = new VertexPositionColorTexture(new Vector3(0, 0, 0), Color.White, new Vector2(0, 0));
            vertices[1] = new VertexPositionColorTexture(new Vector3(10, 0, 0), Color.White, new Vector2(1, 0));
            vertices[2] = new VertexPositionColorTexture(new Vector3(0, 15, 0), Color.White, new Vector2(0, 1));
            vertices[3] = new VertexPositionColorTexture(new Vector3(10, 15, 0), Color.White, new Vector2(1, 1));

            vertices[4] = new VertexPositionColorTexture(new Vector3(0, 0, 5), Color.White, new Vector2(0, 0));
            vertices[5] = new VertexPositionColorTexture(new Vector3(10, 0, 5), Color.White, new Vector2(1, 0));
            vertices[6] = new VertexPositionColorTexture(new Vector3(0, 15, 5), Color.White, new Vector2(0, 1));
            vertices[7] = new VertexPositionColorTexture(new Vector3(10, 15, 5), Color.White, new Vector2(1, 1));

            indices = new int[36];
            //front
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 3;
            indices[3] = 0;
            indices[4] = 3;
            indices[5] = 2;

            //right
            indices[6] = 1;
            indices[7] = 5;
            indices[8] = 3;
            indices[9] = 3;
            indices[10] = 5;
            indices[11] = 7;

            //top
            indices[12] = 0;
            indices[13] = 4;
            indices[14] = 5;
            indices[15] = 0;
            indices[16] = 5;
            indices[17] = 1;

            //left
            indices[18] = 0;
            indices[19] = 2;
            indices[20] = 4;
            indices[21] = 2;
            indices[22] = 6;
            indices[23] = 4;

            //bottom
            indices[24] = 6;
            indices[25] = 2;
            indices[26] = 3;
            indices[27] = 6;
            indices[28] = 3;
            indices[29] = 7;

            //back
            indices[30] = 5;
            indices[31] = 4;
            indices[32] = 6;
            indices[33] = 6;
            indices[34] = 4;
            indices[35] = 7;
        
        }

        private void DrawModel(Model m)
        {
            Vector3 Position = Vector3.One;
            float Zoom = 2500;
            float RotationY = 0.0f;
            float RotationX = 0.0f;
            Matrix gameWorldRotation;
            gameWorldRotation = Matrix.Identity;

            Matrix[] transforms = new Matrix[m.Bones.Count];
            float aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
            m.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix projection =
                Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
                aspectRatio, 1.0f, 10000.0f);
            Matrix view = Matrix.CreateLookAt(new Vector3(0.0f, 50.0f, Zoom),
                Vector3.Zero, Vector3.Up);

            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.View = view;
                    effect.Projection = projection;
                    effect.World = gameWorldRotation *
                        transforms[mesh.ParentBone.Index] *
                        Matrix.CreateTranslation(Position);
                }
                mesh.Draw();
            }
        }
    }
}
