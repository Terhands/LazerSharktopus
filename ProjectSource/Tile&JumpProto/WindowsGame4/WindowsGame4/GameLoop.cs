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
using System.Text;

namespace WindowsGame4
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameLoop : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        ArrayList fonts;
        ArrayList textures;
        ArrayList sounds;

        GameLoader config;
        IGameObject level;

        enum States { title, level, gameOver };
        /* Keyboard controls */
        protected const Keys keyRight = Keys.D;
        protected const Keys keyLeft = Keys.A;
        protected const Keys keyUp = Keys.W;
        protected const Keys keyDown = Keys.S;
        protected const Keys keyJump = Keys.Space;
        protected const Keys keyBolt = Keys.E;
        protected const Keys keyQuit = Keys.Escape;

        KeyboardState prevState;

        public GameLoop()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            if (!graphics.IsFullScreen)
            {
                 //graphics.ToggleFullScreen();
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

            level = new Level(this, textures, fonts, sounds, new LevelLoader(config.LevelFiles));
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            config = new GameLoader("Content\\lathraia.config");

            textures = new ArrayList();
            for (int i = 0; i < config.NumTextures; i++)
            {
                textures.Insert(i, Content.Load<Texture2D>(config.getTextureFile(i)));
            }

            fonts = new ArrayList();
            for (int i = 0; i < config.NumFonts; i++)
            {
                fonts.Insert(i, Content.Load<SpriteFont>(config.getFontFile(i)));
            }

            sounds = new ArrayList();
            for (int i = 0; i < config.NumSoundEffects; i++)
            {
                sounds.Insert(i, Content.Load<SoundEffect>(config.getSoundFile(i)));
            }
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
            KeyboardState currState = Keyboard.GetState();

            if (prevState == null)
            {
                prevState = currState;
            }

            if (currState.IsKeyDown(keyQuit))
            {
                this.Exit();
            }

            if (currState.IsKeyDown(keyUp))
            {
                level.Update(Action.down, 0);
            }

            if (currState.IsKeyDown(keyLeft))
            {
                level.Update(Action.left, -2);
            }

            if (currState.IsKeyDown(keyDown))
            {
                level.Update(Action.up, 0);
            }

            if (currState.IsKeyDown(keyRight))
            {
                level.Update(Action.right, 2);
            }

            if (currState.IsKeyDown(keyJump))
            {
                level.Update(Action.chargeJump, 0);
            }

            if (currState.IsKeyDown(keyBolt) && prevState.IsKeyUp(keyBolt))
            {
                level.Update(Action.throwBolt, 0);
            }

            if (currState.IsKeyUp(keyJump) && prevState.IsKeyDown(keyJump))
            {
                level.Update(Action.jump, 0);
            }

            else
            {
                level.Update(Action.none, 0);
            }
            level.Update(Action.boltUpdates, 0);
            prevState = currState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            level.Draw(spriteBatch);

            this.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}