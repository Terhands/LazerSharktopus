using System;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame4
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        ArrayList tileTextures;
        ILevel level;
        IPlayer player;

        KeyboardState prevState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            if (!graphics.IsFullScreen)
            {
               // graphics.ToggleFullScreen();
            }
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

            tileTextures = new ArrayList();
            tileTextures.Insert(0, Content.Load<Texture2D>("blank"));

            level = new Level(this, "test.txt", tileTextures);

            int screenHeight = GraphicsDevice.Viewport.Height;

            Texture2D playerTexture = Content.Load<Texture2D>("Robro1.1");
            player = new Player(playerTexture, 390, screenHeight - 52 - (screenHeight / 32));

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
            {
                this.Exit();
            }

            KeyboardState keyState = Keyboard.GetState();

            if (prevState == null)
            {
                prevState = keyState;
            }

            if (keyState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            if (keyState.IsKeyDown(Keys.W))
            {
                level.Update(Direction.down, 2);
            }

            if (keyState.IsKeyDown(Keys.A))
            {
                level.Update(Direction.right, 2);
            }

            if (keyState.IsKeyDown(Keys.S))
            {
                level.Update(Direction.up, 2);
            }

            if(keyState.IsKeyDown(Keys.D))
            {
                level.Update(Direction.left, 2);
            }

            if (keyState.IsKeyDown(Keys.Space))
            {
                player.ChargeJumpPower();
            }

            if (keyState.IsKeyUp(Keys.Space) && prevState.IsKeyDown(Keys.Space))
            {
                player.Jump();
            }

            player.Update(Direction.down, 0);

            prevState = keyState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            this.spriteBatch.Begin();
            GraphicsDevice.Clear(Color.CornflowerBlue);

            level.Draw(spriteBatch);
            player.Draw(spriteBatch);

            // TODO: Add your drawing code here
            this.spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
