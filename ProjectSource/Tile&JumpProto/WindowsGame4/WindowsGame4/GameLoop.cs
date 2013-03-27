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
        MusicManager musicPlayer;
        SpriteBatch spriteBatch;

        ArrayList fonts;
        ArrayList textures;
        ArrayList sounds;
        ArrayList songs;

        GameLoader config;
        
        public enum GameState { titleMenu, titleScreen, level, gameOver, victory, levelIntro };
        GameState gameState;
        GameOver gameOver;
        TitleMenu titleMenu;
        TitleScreen titleScreen;
        LevelIntroScreen levelIntroScreen;

        Level level;

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
                graphics.ToggleFullScreen();
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
            gameState = GameState.titleScreen;

            level = new Level(this, textures, fonts, sounds, musicPlayer, new LevelLoader(config.LevelFiles));
            gameOver = new GameOver(this, (Texture2D)textures[3], (SpriteFont)fonts[2]);
            titleScreen = new TitleScreen(this, (Texture2D)textures[7], (SpriteFont)fonts[2]);
            titleMenu = new TitleMenu(this, (Texture2D)textures[7], (SpriteFont)fonts[2]);
            levelIntroScreen = new LevelIntroScreen(this, (SpriteFont)fonts[1]);
            levelIntroScreen.InitLevelScreen(level.LevelName);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            config = new GameLoader(@"Content\lathraia.config");

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

            songs = new ArrayList();
            for (int i = 0; i < config.NumSongs; i++)
            {
                songs.Insert(i, Content.Load<Song>(config.getSongFile(i)));
            }

            musicPlayer = new MusicManager(songs);
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
            if (gameState == GameState.level)
            {
                level.Update(gameTime);
            }
            else if (gameState == GameState.levelIntro)
            {
                levelIntroScreen.Update();
            }
            else if (gameState == GameState.gameOver)
            {
                gameOver.Update();
            }
            else if (gameState == GameState.titleMenu)
            {
                titleMenu.Update();
            }
            else if (gameState == GameState.victory)
            {
                this.Exit();
            }
            else if (gameState == GameState.titleScreen)
            {
                level.CurrentLevel = 0;
                titleScreen.Update();
            }

            if (prevState == null)
            {
                prevState = currState;
            }

            if (currState.IsKeyDown(keyQuit))
            {
                this.Exit();
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

            if (gameState == GameState.level)
            {
                level.Draw(spriteBatch);
            }
            else if (gameState == GameState.levelIntro)
            {
                levelIntroScreen.Draw(spriteBatch);
            }
            else if (gameState == GameState.gameOver)
            {
                gameOver.Draw(spriteBatch);
            }
            else if (gameState == GameState.titleMenu)
            {
                titleMenu.Draw(spriteBatch);
            }
            else if (gameState == GameState.victory)
            {
                // Victory Screen Updates
            }
            else if (gameState == GameState.titleScreen)
            {
                titleScreen.Draw(spriteBatch);
            }



            this.spriteBatch.End();

            base.Draw(gameTime);
        }

        public GameState State
        {
            set { gameState = value; }
        }

        // when level state is set the new map needs to be built
        public void SetGameState(GameState _gameState)
        {
            gameState = _gameState;
            if (GameState.level == gameState)
            {
                level.InitLevel();
            }
            else if (GameState.levelIntro == gameState)
            {
                levelIntroScreen.InitLevelScreen(level.LevelName);
            }
        }
    }
}