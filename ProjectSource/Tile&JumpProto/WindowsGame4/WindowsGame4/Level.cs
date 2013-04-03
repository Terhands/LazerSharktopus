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
        protected GameTimer gameTimer;

        LevelLoader levelLoader;
        int currentLevel;

        protected const int playerIndex = 0;
        protected const int wizardIndex = 5;
        protected const int soldierIndex = 15;
        protected const int LOSIndex = 18;
        protected const int leverIndex = 9;
        protected const int gateIndex = 10;
        protected const int boxOfBoltsIndex = 19;

        protected Rectangle playerRange;

        int deathCounter = 0;
        int maxDeathCounter = 100;

        ArrayList textures;
        ArrayList sounds;
        ArrayList fonts;

        MusicManager musicPlayer;
        GuardFactory guardFactory;
        PlotScreen plotScreen;

        Texture2D boxTexture;
	    Texture2D boltTexture;
        GameLoop game;
        InputHandler inputHandler;
        
        protected List<Bolt> bolts;
        protected List<Torch> torches;
        protected List<IGuard> guards;
        protected List<Lever> levers;
        protected List<Gate> gates;
        protected List<BoxOfBolts> boxBolts;
        protected List<Button> buttons;
        protected List<Spout> spouts;

        public Level(GameLoop game, ArrayList _textures, ArrayList _fonts, ArrayList _sounds, MusicManager _musicPlayer, PlotScreen _plotScreen, LevelLoader loader, InputHandler _inputHandler) : base(game)
        {
            int screenWidth = Game.GraphicsDevice.Viewport.Width;
            int screenHeight = Game.GraphicsDevice.Viewport.Height;

            bolts = new List<Bolt>();
            torches = new List<Torch>();
            guards = new List<IGuard>();
            levers = new List<Lever>();
            gates = new List<Gate>();
            boxBolts = new List<BoxOfBolts>();
            buttons = new List<Button>();
            spouts = new List<Spout>();

            playerRange = new Rectangle((screenWidth * 2)/5, 0, screenWidth/5, screenHeight);
            
            levelLoader = loader;
            textures = _textures;
            sounds = _sounds;
            fonts = _fonts;

            musicPlayer = _musicPlayer;
            plotScreen = _plotScreen;
            guardFactory = new GuardFactory((Texture2D)textures[wizardIndex], (Texture2D)textures[soldierIndex], (Texture2D)textures[LOSIndex]);

            currentLevel = 0;
            deathCounter = 0;

            screenWidth = Game.GraphicsDevice.Viewport.Width;
            screenHeight = Game.GraphicsDevice.Viewport.Height;

            this.game = game;
            inputHandler = _inputHandler;
            levelLoader.LoadLevel(currentLevel);
        }

        public void InitLevel()
        {
            bolts.Clear();
            torches.Clear();
            guards.Clear();
            levers.Clear();
            gates.Clear();
            musicPlayer.Stop();
            spouts.Clear();
            buttons.Clear();

            levelLoader.LoadLevel(currentLevel);
            if (levelLoader.Map == null)
            {
                // If there's no level, then we've passed the last level and the player has won the game
                game.SetGameState(GameLoop.GameState.victory);
                return;
            }
            levelMap = new Map(Game, levelLoader.Map, textures, levelLoader.LevelBackground);

            int screenWidth = Game.GraphicsDevice.Viewport.Width;
            int screenHeight = Game.GraphicsDevice.Viewport.Height;

            player = new Player(Game, (Texture2D)textures[playerIndex], sounds, 50, screenHeight - 52 - (screenHeight / 32));
            
            boltTexture = (Texture2D)textures[4];
            boxTexture = (Texture2D)textures[boxOfBoltsIndex];
            Texture2D[] torchTextures = new Texture2D[2];
            torchTextures[0] = (Texture2D)textures[6];
            torchTextures[1] = (Texture2D)textures[8];

            // load torches from the level files
            foreach (Vector2 v in levelLoader.Torches)
            {
                int x = ((int)v.X) * (screenWidth / 64) - (15/2);
                int y = (((int)v.Y) * (screenHeight / 32)) - 25;
                torches.Add(new Torch(Game, torchTextures, x, y));
            }

            foreach (Vector3 v in levelLoader.Guards)
            {
                int x = ((int)v.X) * (screenWidth / 64) - (36/2);
                int y = ((int)v.Y) * (screenHeight / 32) - (28*2);
                int type = (int)v.Z;
                guards.Add(guardFactory.createGuard(Game, x, y, Direction.right, 100, type));
            }

            foreach (Vector2 v in levelLoader.Gates)
            {
                /* These offsets will be wrong right now */
                int x = ((int)v.X);
                int y = ((int)v.Y);
                gates.Add(new Gate(Game, x, y, screenWidth, screenHeight, (Texture2D)textures[gateIndex], this));
            }

            int[][] gateMaps = levelLoader.levelGateMaps;
            int i = 0;
            foreach (Vector2 v in levelLoader.Levers)
            {
                List<Gate> leverGates = new List<Gate>();
                for (int j = 0; j < gateMaps[i].Length; j++)
                {
                    leverGates.Add(gates[j]);
                }
                int x = ((int)v.X) * (screenWidth / 64) - (24 / 2);
                int y = ((int)v.Y) * (screenHeight / 32) - 22;
                levers.Add(new Lever(Game, x, y, screenWidth, screenHeight, Lever.LeverType.switcher, leverGates, (Texture2D)textures[leverIndex]));
                i++;
            }

            foreach (Vector2 v in levelLoader.BoxesOfBolts)
            {
                int x = ((int)v.X) * (screenWidth / 64) - (15 / 2);
                int y = ((int)v.Y) * (screenHeight / 32) - 22;
                boxBolts.Add(new BoxOfBolts(Game, x, y, boxTexture));
            }

            foreach (Vector2 v in levelLoader.Spouts)
            {
                /* These offsets will be wrong right now */
                int x = ((int)v.X);
                int y = ((int)v.Y);
                spouts.Add(new Spout(Game, x, y, screenWidth, screenHeight, (Texture2D)textures[21]));
            }

            int[][] buttonMaps = levelLoader.ButtonSpoutMaps;
            i = 0;
            foreach (Vector2 v in levelLoader.Buttons)
            {
                List<Spout> buttonSpouts = new List<Spout>();
                for (int j = 0; j < buttonMaps[i].Length; j++)
                {
                    buttonSpouts.Add(spouts[j]);
                }
                int x = ((int)v.X) * (screenWidth / 64) - (24 / 2);
                int y = ((int)v.Y) * (screenHeight / 32) - 22;
                buttons.Add(new Button(Game, x, y, screenWidth, screenHeight, buttonSpouts, (Texture2D)textures[20]));
                i++;
            }

    
            gameTimer = new GameTimer(levelLoader.TimeLimit, (SpriteFont)fonts[0]);

            // don't keep restarting the song if it is already playing
            if (musicPlayer.CurrSong != levelLoader.LevelMusic || musicPlayer.isStopped)
            {
                musicPlayer.Play(levelLoader.LevelMusic);
            }

        }

        /* procedure responsible for updating this level given an action (velocity should eventually be determined by player)*/
        public override void Update(GameTime gameTime)
        {   
            /* Timer update logic */
            gameTimer.Update();
            if (gameTimer.isFinished() && !player.IsDead)
            {
                player.Kill();
            }


            // no need to perform update if the player died - get ready for some serious death-screen action
            if (!player.IsDead)
            {
                /* Control jumping state of player */
                if (inputHandler.isPressed(InputHandler.InputTypes.jump))
                {
                    player.ChargeJumpPower();
                }
                if (inputHandler.isNewlyReleased(InputHandler.InputTypes.jump))
                {
                    player.Jump();
                }

                /* Control hiding state of player */
                if (inputHandler.isPressed(InputHandler.InputTypes.down))
                {
                    player.Hide(levelMap.GetNearbyTiles(player.GetPosition()));
                }
                if (inputHandler.isNewlyReleased(InputHandler.InputTypes.down))
                {
                    player.StopHiding();
                }

                Action playerAction = Action.none;
                int velocity = 0;


                if (inputHandler.isPressed(InputHandler.InputTypes.right))
                {
                    playerAction = Action.right;
                    velocity = 2;
                }
                else if (inputHandler.isPressed(InputHandler.InputTypes.left))
                {
                    playerAction = Action.left;
                    velocity = -2;
                }

                if (inputHandler.isNewlyPressed(InputHandler.InputTypes.pull))
                {
                    foreach (Lever lever in levers)
                    {
                        lever.HandleCollision(levelMap.GetNearbyTiles(player.GetPosition()));
                    }
                }

                if (inputHandler.isNewlyPressed(InputHandler.InputTypes.pull))
                {
                    foreach (Button button in buttons)
                    {
                        button.HandleCollision(levelMap.GetNearbyTiles(player.GetPosition()));
                    }
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

                    foreach (Wizard guard in guards)
                    {
                        guard.Update(playerAction, deltaX);
                        if (guard.IsDead)
                        {
                            player.Kill();
                        }
                    }

                    foreach (Bolt bolt in bolts)
                    {
                        bolt.reposition(deltaX);
                    }

                    foreach (Lever lever in levers)
                    {
                        lever.reposition(deltaX);
                    }

                    foreach (Gate gate in gates)
                    {
                        gate.reposition(deltaX);
                    }

                    foreach (Button button in buttons)
                    {
                        button.reposition(deltaX);
                    }

                    foreach (Spout spout in spouts)
                    {
                        spout.reposition(deltaX);
                    }

                    // Update position
                    foreach (BoxOfBolts boxBolt in boxBolts)
                    {
                        boxBolt.reposition(deltaX);
                    }
                }

                // Will gather bolts to reset players health
                if (inputHandler.isNewlyPressed(InputHandler.InputTypes.gather))
                {
                    foreach (BoxOfBolts box in boxBolts)
                    {
                        box.HandleCollision(levelMap.GetNearbyTiles(player.GetPosition()));
                        if (box.getGathered())
                        {
                            player.healDamage();
                        }
                    }
                }

                /* Below this are Bolt actions */
                if (inputHandler.isNewlyPressed(InputHandler.InputTypes.bolt))
                {
                    if (bolts.Count < 5)
                    {
                        ArrayList boltSounds = new ArrayList();
                        boltSounds.Add(sounds[1]);
                        boltSounds.Add(sounds[2]);
                        bolts.Add(new Bolt(Game, player.GetFacingDirection(), player.GetPosition().X, player.GetPosition().Y, boltTexture, boltSounds));
                        player.throwBolt();
                    }
                }

                foreach (Torch t in torches)
                {
                    t.Update(gameTime);
                    t.HandleCollision(player);
                }

                IList<Bolt> collidedBolts = new List<Bolt>();

                foreach (Bolt bolt in bolts)
                {
                    bolt.Update(Action.none, 0);
                    if (!bolt.hasCollided)
                    {
                        bolt.HandleCollision(levelMap.GetNearbyTiles(bolt.GetPosition()));
                    }
                    else
                    {
                        collidedBolts.Add(bolt);
                    }

                    if (bolt.expiryTime <= 0)
                    {
                        bolts.Remove(bolt);
                        break;
                    }
                }

                foreach (Wizard guard in guards)
                {
                    guard.Update(gameTime);
                   
                        guard.HandleCollision(levelMap.GetNearbyTiles(guard.GetPosition()));
                        guard.HandleVision(player, levelMap.GetNearbyTiles(guard.GetLOSRectangle()));
                        // need a way to get back all bolts that have collided - have to actually hear it, not see it with my 360 degree camera strapped to the inside of the guard's visor
                        guard.HandleHearing(collidedBolts);
                }

                foreach (Gate gate in gates)
                {
                    gate.Update(gameTime);
                }

                foreach (Button button in buttons)
                {
                    button.Update(gameTime);
                }

                if (player.DoneLevel)
                {
                    // do some intermediate next level screen...
                    currentLevel += 1;
                    bolts.Clear();
                    torches.Clear();
                    guards.Clear();
                    levers.Clear();
                    gates.Clear();
                    boxBolts.Clear();
                    musicPlayer.Stop();
                    buttons.Clear();
                    spouts.Clear();

                    // if there is a next level get the map loaded
                    if (levelLoader.NumLevels > currentLevel)
                    {
                        levelLoader.LoadLevel(currentLevel);
                    }

                    if (currentLevel % 3 == 0 && plotScreen != null)
                    {
                        plotScreen.initPlotScreen();
                        game.SetGameState(GameLoop.GameState.plotScreen);
                    }
                    else if (levelLoader.NumLevels > currentLevel)
                    {
                        game.SetGameState(GameLoop.GameState.levelIntro);
                    }
                    else
                    {
                        game.SetGameState(GameLoop.GameState.victory);
                    }
                }
            }
            else
            {
                deathCounter += 1;
                if (deathCounter > maxDeathCounter)
                {
                    torches.Clear();
                    bolts.Clear();
                    guards.Clear();
                    levers.Clear();
                    gates.Clear();
                    boxBolts.Clear();
                    musicPlayer.Stop();
                    game.SetGameState(GameLoop.GameState.gameOver);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            levelMap.Draw(spriteBatch);
            player.Draw(spriteBatch);
            gameTimer.Draw(spriteBatch);

            foreach (Wizard guard in guards)
            {
                guard.Draw(spriteBatch);
            }
            foreach (Bolt bolt in bolts)
            {
                bolt.Draw(spriteBatch);
            }
            foreach (Torch t in torches)
            {
                t.Draw(spriteBatch);
            }
            foreach (Lever lever in levers)
            {
                lever.Draw(spriteBatch);
            }
            foreach (Gate gate in gates)
            {
                gate.Draw(spriteBatch);
            }
            foreach (BoxOfBolts box in boxBolts)
            {
                box.Draw(spriteBatch);
            }
            foreach (Spout spout in spouts)
            {
                spout.Draw(spriteBatch);
            }
            foreach (Button button in buttons)
            {
                button.Draw(spriteBatch);
            }
        }

        public string LevelName
        {
            get { return levelLoader.LevelName; }
        }

        public int CurrentLevel
        {
            get { return currentLevel; }
            set { currentLevel = value; }
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

        public void modifyTiles(Rectangle changeRect, CollisionType newType)
        {
            for (int i = changeRect.X; i < changeRect.X + changeRect.Width; i++)
            {
                for (int j = changeRect.Y; j < changeRect.Y + changeRect.Height; j++)
                {
                    levelMap.changeTile(i, j, newType);
                }
            }
        }

    }
}
