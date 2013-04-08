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
        
        public enum GameState { titleMenu, titleScreen, level, tutorial, gameOver, victory, levelIntro, credits, plotScreen };
        GameState gameState;
        GameState prevGameState;

        GameOver gameOver;
        TitleMenu titleMenu;
        TitleScreen titleScreen;
        LevelIntroScreen levelIntroScreen;
        Credits credits;
        PlotScreen plotScreen;

        PlayerAnimation playerAnimation;
        bool animatePlayer;

        Level level;
        Level tutorial;

        int mainMenuIndex = 2;
        int creditsIndex = 17;

        InputHandler inputHandler;

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
            base.Initialize();
            gameState = GameState.titleScreen;
            prevGameState = gameState;
            inputHandler = new InputHandler();

            plotScreen = new PlotScreen(this, musicPlayer, textures, fonts);
            level = new Level(this, textures, fonts, sounds, musicPlayer, plotScreen, new LevelLoader(config.LevelFiles), inputHandler);
            tutorial = new Level(this, textures, fonts, sounds, musicPlayer, null, new LevelLoader(config.TutorialFiles), inputHandler);
            gameOver = new GameOver(this, (Texture2D)textures[3], (SpriteFont)fonts[2], inputHandler);
            titleScreen = new TitleScreen(this, (Texture2D)textures[7], (Texture2D)textures[16], (SpriteFont)fonts[2], musicPlayer, inputHandler);
            titleMenu = new TitleMenu(this, (Texture2D)textures[7], (SpriteFont)fonts[2], inputHandler);
            levelIntroScreen = new LevelIntroScreen(this, (SpriteFont)fonts[1]);
            levelIntroScreen.InitLevelScreen(level.LevelName);

            credits = new Credits(this, (Texture2D)textures[creditsIndex], musicPlayer, fonts);

            playerAnimation = new PlayerAnimation(this, new Player(this, (Texture2D)textures[0], sounds, -100, 400));
            animatePlayer = false;

            int screenWidth = graphics.GraphicsDevice.Viewport.Width;
            int screenHeight = graphics.GraphicsDevice.Viewport.Height;

            //initializing virtual screen resolution to be mapped to the actual screen
            Resolution.Init(ref graphics);
            Resolution.SetVirtualResolution(screenWidth, screenHeight);
            Resolution.SetResolution(800, 600, false);
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
            inputHandler.Update();
            if (gameState == GameState.level)
            {
                level.Update(gameTime);
            }
            else if (gameState == GameState.tutorial)
            {
                tutorial.Update(gameTime);
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
                AnimatePlayer = true;
                if (musicPlayer.isStopped)
                {
                    musicPlayer.Play(mainMenuIndex);
                }
                titleMenu.Update();
            }
            else if (gameState == GameState.victory)
            {
                if (prevGameState == GameState.tutorial)
                {
                    SetGameState(GameState.titleMenu);
                }
                else
                {
                    SetGameState(GameState.credits);
                }
            }
            else if (gameState == GameState.titleScreen)
            {
                level.CurrentLevel = 0;
                titleScreen.Update();
            }
            else if (gameState == GameState.credits)
            {
                credits.Update();
                if (inputHandler.isNewlyPressed(InputHandler.InputTypes.start) || inputHandler.isNewlyPressed(InputHandler.InputTypes.jump))
                {
                    SetGameState(GameState.titleScreen);
                }
            }
            else if (gameState == GameState.plotScreen)
            {
                plotScreen.Update();
                if (inputHandler.isNewlyPressed(InputHandler.InputTypes.start) || inputHandler.isNewlyPressed(InputHandler.InputTypes.jump))
                {
                    if (plotScreen.IsEnding)
                    {
                        SetGameState(GameState.credits);
                    }
                    else
                    {
                        SetGameState(GameState.levelIntro);
                    }
                }
            }

            if (inputHandler.isPressed(InputHandler.InputTypes.quit))
            {
                this.Exit();
            }

            if (animatePlayer)
            {
                playerAnimation.Update(Action.none, 0);
            }

            base.Update(gameTime);
        }

        public bool AnimatePlayer
        {
            set { animatePlayer = value; }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Resolution.BeginDraw();

            //this.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, Resolution.getTransformationMatrix());

            if (gameState == GameState.level)
            {
                level.Draw(spriteBatch);
            }
            else if (gameState == GameState.tutorial)
            {
                tutorial.Draw(spriteBatch);
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
            else if (gameState == GameState.credits)
            {
                credits.Draw(spriteBatch);
            }
            else if (gameState == GameState.plotScreen)
            {
                plotScreen.Draw(spriteBatch);
            }

            if (animatePlayer)
            {
                playerAnimation.Draw(spriteBatch);
            }

            this.spriteBatch.End();

            base.Draw(gameTime);
        }

        public GameState PrevGameState
        {
            get { return prevGameState; }
        }

        // reset the plot screen so the correct ones play when restarting the game
        public void ResetScreens()
        {
            plotScreen.Reset();
        }

        // when level state is set the new map needs to be built
        public void SetGameState(GameState _gameState)
        {
            // need to know if the user is running actual levels or the tutorials
            if (gameState == GameState.level || gameState == GameState.tutorial)
            {
                prevGameState = gameState;
            }
            gameState = _gameState;

            if (GameState.level == gameState)
            {
                level.InitLevel();
            }
            else if (GameState.tutorial == gameState)
            {
                tutorial.InitLevel();
            }
            else if (GameState.levelIntro == gameState)
            {
                if (prevGameState == GameState.level)
                {
                    levelIntroScreen.InitLevelScreen(level.LevelName);
                }
                else if (prevGameState == GameState.tutorial)
                {
                    levelIntroScreen.InitLevelScreen(tutorial.LevelName);
                }
            }
            else if (GameState.credits == gameState)
            {
                credits.initScrollingTextScreen();
            }
        }
    }
}