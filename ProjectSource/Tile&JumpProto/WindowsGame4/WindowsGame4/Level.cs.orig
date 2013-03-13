using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame4
{
    class Level : Microsoft.Xna.Framework.GameComponent
    {
        protected IPlayer player;
        protected IMap levelMap;
        protected Guard guard;
        protected GameTimer gameTimer;

        LevelLoader levelLoader;
        int currentLevel;

        protected const int playerIndex = 0;
        protected const int guardIndex = 5;
        protected Rectangle playerRange;

        int deathCounter = 0;
        int maxDeathCounter = 100;

        ArrayList textures;
        ArrayList sounds;
        ArrayList fonts;
        ArrayList songFiles;

        MusicManager musicPlayer;

	    Texture2D boltTexture;
        GameLoop game;
        KeyboardState keyState;
        KeyboardState prevKeyState;

        protected List<Bolt> bolts;
        protected List<Torch> torches;

        public Level(GameLoop game, ArrayList _textures, ArrayList _fonts, ArrayList _sounds, ArrayList _songs, LevelLoader loader) : base(game)
        {
            int screenWidth = Game.GraphicsDevice.Viewport.Width;
            int screenHeight = Game.GraphicsDevice.Viewport.Height;

            bolts = new List<Bolt>();
            torches = new List<Torch>();

            playerRange = new Rectangle((screenWidth * 2)/5, 0, screenWidth/5, screenHeight);
            
            levelLoader = loader;
            textures = _textures;
            sounds = _sounds;
            fonts = _fonts;

            musicPlayer = new MusicManager(_songs);

            currentLevel = 0;
            deathCounter = 0;

            screenWidth = Game.GraphicsDevice.Viewport.Width;
            screenHeight = Game.GraphicsDevice.Viewport.Height;

            this.game = game;
            InitLevel();
        }

        public void InitLevel()
        {
            levelLoader.LoadLevel(currentLevel);
            if (levelLoader.Map == null)
            {
                // If there's no level, then we've passed the last level and the player has won the game
                game.State = GameLoop.States.victory;
                return;
            }
            levelMap = new Map(Game, levelLoader.Map, textures);

            int screenWidth = Game.GraphicsDevice.Viewport.Width;
            int screenHeight = Game.GraphicsDevice.Viewport.Height;

            player = new Player(Game, (Texture2D)textures[playerIndex], sounds, 50, screenHeight - 52 - (screenHeight / 32));
            guard = new Guard(Game, (Texture2D)textures[guardIndex], 700, screenHeight - 52 - (screenHeight / 32), Direction.right, 100);
            
            boltTexture = (Texture2D)textures[4];

            // load torches from the level files
            foreach (Vector2 v in levelLoader.Torches)
            {
                int x = ((int)v.X) * (screenWidth / 64) - (15/2);
                int y = (((int)v.Y) * (screenHeight / 32)) - 25;
                torches.Add(new Torch(Game, (Texture2D)textures[6], x, y));
            }

            gameTimer = new GameTimer(levelLoader.TimeLimit, (SpriteFont)fonts[0]);

            musicPlayer.Play(levelLoader.LevelMusic);

            keyState = Keyboard.GetState();
            prevKeyState = keyState;
        }

        /* procedure responsible for updating this level given an action (velocity should eventually be determined by player)*/
        public void Update(GameTime gameTime)
        {   
            /* Timer update logic */
            gameTimer.Update();
            if (gameTimer.isFinished()) player.IsDead = true;


            // no need to perform update if the player died - get ready for some serious death-screen action
            if (!player.IsDead)
            {
                keyState = Keyboard.GetState();
                /* Control jumping state of player */
                if (keyState.IsKeyDown(Keys.Space))
                {
                    player.ChargeJumpPower();
                }
                if (keyState.IsKeyUp(Keys.Space) && prevKeyState.IsKeyDown(Keys.Space))
                {
                    player.Jump();
                }

                /* Control hiding state of player */
                if (keyState.IsKeyDown(Keys.S))
                {
                    player.Hide(levelMap.GetNearbyTiles(player.GetPosition()));
                }
                if (keyState.IsKeyUp(Keys.S) && prevKeyState.IsKeyDown(Keys.S))
                {
                    player.StopHiding();
                }

                Action playerAction = Action.none;
                int velocity = 0;


                if (keyState.IsKeyDown(Keys.D))
                {
                    playerAction = Action.right;
                    velocity = 2;
                }
                else if (keyState.IsKeyDown(Keys.A))
                {
                    playerAction = Action.left;
                    velocity = -2;
                }

                // update the player position when the player needs to change position on screen
                player.Update(playerAction, velocity);
                player.HandleCollision(levelMap.GetNearbyTiles(player.GetPosition()));



                // would like to find a way to just call foreach i, i.Update(a, v) instead of having to explicitly deal with the map...
                if (shouldShiftScreen(playerAction))
                {
                    // update the map position when the background screen needs to be updated
                    int deltaX = player.DeltaX;
                    levelMap.Update(playerAction, deltaX);
                    player.reposition();

                    foreach (Torch t in torches)
                    {
                        t.Update(playerAction, deltaX);
                    }

                    //update guards
                    guard.Update(playerAction, deltaX);
                   

                    foreach (Bolt bolt in bolts)
                    {
                        bolt.reposition(deltaX);
                    }
                }


                /* Below this are Bolt actions */
                if (keyState.IsKeyDown(Keys.E) && prevKeyState.IsKeyUp(Keys.E))
                {
                    if (bolts.Count < 5)
                    {
                        ArrayList boltSounds = new ArrayList();
                        boltSounds.Add(sounds[1]);
                        boltSounds.Add(sounds[2]);
                        bolts.Add(new Bolt(Game, player.GetFacingDirection(), player.GetPosition().X, player.GetPosition().Y, boltTexture, boltSounds));
                    }
                }

                foreach (Torch t in torches)
                {
                    t.Update(gameTime);
                }

                guard.Update(gameTime);
                guard.HandleCollision(player);

                foreach (Bolt bolt in bolts)
                {
                    bolt.Update(Action.none, 0);
                    if (!bolt.hasCollided)
                    {
                        bolt.HandleCollision(levelMap.GetNearbyTiles(bolt.GetPosition()));
                    }

                    if (bolt.expiryTime <= 0)
                    {
                        bolts.Remove(bolt);
                        break;
                    }
                }

                if (player.DoneLevel)
                {
                    // do some intermediate next level screen...
                    currentLevel += 1;
                    bolts.Clear();
                    torches.Clear();
                    InitLevel();
                }
            }
            else
            {
                deathCounter += 1;
                if (deathCounter > maxDeathCounter)
                {
                    game.State = GameLoop.States.gameOver;
                }
            }
            prevKeyState = keyState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            levelMap.Draw(spriteBatch);
            player.Draw(spriteBatch);
            guard.Draw(spriteBatch);
            gameTimer.Draw(spriteBatch);
            foreach (Bolt bolt in bolts)
            {
                bolt.Draw(spriteBatch);
            }
            foreach (Torch t in torches)
            {
                t.Draw(spriteBatch);
            }
        }

        /* figure out if the screen needs to shift to reflect the given action */
        protected bool shouldShiftScreen(Action action)
        {
            Rectangle playerPos = player.GetPosition();

            bool result = false;

            if((Action.right == action && levelMap.atRightEdge()) || (Action.left == action && levelMap.atLeftEdge()))
            {
                result = false;
            }
            else if (Action.right == action && playerPos.X <= (playerRange.X + playerPos.Width))
            {
                result = false;
            }
            else if (Action.left == action && playerPos.X >= (playerRange.X - playerPos.Width))
            {
                result = false;
            }
            else if (playerRange.Contains(playerPos))
            {
                result = false;
            }
            else
            {
                result = true;
            }

            return result;
        }
    }
}
