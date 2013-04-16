#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

using LostGeneration.View;
using LostGeneration.Model;
#endregion

namespace LostGeneration
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class LostGen : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Camera camera;
        AutoTileMap tileMap;
        SpriteFont font;

        public LostGen()
            : base()
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
            camera = new Camera(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            int[,] grid = new int[,] {
                { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 1, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 1, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 0, 1, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 1, 0, 1, 0, 0, 0, 1, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0 },
                { 0, 0, 1, 1, 1, 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
                { 0, 0, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }
            };

            Model.Terrain[,] data = new Model.Terrain[15, 20];
            for (int j = 0; j < 15; j++) {
                for (int i = 0; i < 20; i++) {
                    switch (grid[j, i]) {
                        case 0:
                            data[j, i] = Model.Terrain.Wall;
                            break;
                        case 1:
                            data[j, i] = Model.Terrain.Floor;
                            break;
                    }
                }
            }

            tileMap = new AutoTileMap();
            tileMap.Create(grid);
            tileMap.SetTexture(Content.Load<Texture2D>("Sprites\\Terrain\\sprAutoTileSequence"), 32, 32);

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
            font = Content.Load<SpriteFont>("Fonts\\fntDefault");
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
            float dt = (float)(gameTime.ElapsedGameTime.Milliseconds / 1000.0F);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            KeyboardState keyState = Keyboard.GetState();
            int horz = (keyState.IsKeyDown(Keys.D) ? 1 : 0) - (keyState.IsKeyDown(Keys.A) ? 1 : 0);
            int vert = (keyState.IsKeyDown(Keys.S) ? 1 : 0) - (keyState.IsKeyDown(Keys.W) ? 1 : 0);

            int cvert = (keyState.IsKeyDown(Keys.Up) ? 1 : 0) - (keyState.IsKeyDown(Keys.Down) ? 1 : 0);

            camera.Position.X += 100.0F * horz * dt;
            camera.Position.Y += 100.0F * vert * dt;
            camera.Zoom += cvert * dt;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise,
                null,
                camera.Transform
            );

            tileMap.Draw(spriteBatch, camera);

            Vector2 topLeft = new Vector2(camera.Left, camera.Top);
            Vector2 bottomRight = new Vector2(camera.Right - 32, camera.Bottom - 32);
            spriteBatch.DrawString(font, "!", topLeft, Color.Red);
            spriteBatch.DrawString(font, "!", bottomRight, Color.Red);

            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
