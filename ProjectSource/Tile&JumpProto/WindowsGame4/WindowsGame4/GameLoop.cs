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
    public class GameLoop : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        ArrayList textures;
        IGameObject level;

        KeyboardState prevState;

        public GameLoop()
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
            base.Initialize();

            level = new Level(this, textures);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            textures = new ArrayList();
            textures.Insert(0, Content.Load<Texture2D>("Robro1.1"));
            textures.Insert(1, Content.Load<Texture2D>("Lazersharktopus"));
            textures.Insert(2, Content.Load<Texture2D>("groundSpriteFile"));

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

            KeyboardState currState = Keyboard.GetState();

            if (prevState == null)
            {
                prevState = currState;
            }

            if (currState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            if (currState.IsKeyDown(Keys.W))
            {
                level.Update(Action.down, 0);
            }

            if (currState.IsKeyDown(Keys.A))
            {
                level.Update(Action.left, -2);
            }

            if (currState.IsKeyDown(Keys.S))
            {
                level.Update(Action.up, 0);
            }

            if (currState.IsKeyDown(Keys.D))
            {
                level.Update(Action.right, 2);
            }

            if (currState.IsKeyDown(Keys.Space))
            {
                level.Update(Action.chargeJump, 0);
            }

            if (currState.IsKeyUp(Keys.Space) && prevState.IsKeyDown(Keys.Space))
            {
                level.Update(Action.jump, 0);
            }
            else
            {
                level.Update(Action.none, 0);
            }

            prevState = currState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            this.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            level.Draw(spriteBatch);

            // TODO: Add your drawing code here
            this.spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
