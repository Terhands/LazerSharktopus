using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace WindowsGame4
{
    class Level : Microsoft.Xna.Framework.GameComponent, IGameObject
    {
        protected IPlayer player;
        protected IMap levelMap;
        protected GameTimer gameTimer;

        LevelLoader levelLoader;
        int currentLevel;

        protected const int playerIndex = 0;
        protected Rectangle playerRange;

        int deathCounter = 0;
        int maxDeathCounter = 100;

        ArrayList textures;
        ArrayList sounds;
	    Texture2D boltTexture;

        protected List<Bolt> bolts;

<<<<<<< HEAD
        public Level(GameLoop game, ArrayList _textures, ArrayList _fonts, LevelLoader loader) : base(game)
=======
        public Level(GameLoop game, ArrayList _textures, ArrayList _sounds, LevelLoader loader) : base(game)
>>>>>>> 154f9e36a71c70401042ec434d50d118fac2e877
        {
            int screenWidth = Game.GraphicsDevice.Viewport.Width;
            int screenHeight = Game.GraphicsDevice.Viewport.Height;
            bolts = new List<Bolt>();
            playerRange = new Rectangle((screenWidth * 2)/5, 0, screenWidth/5, screenHeight);

            levelLoader = loader;
            textures = _textures;
            sounds = _sounds;

            currentLevel = 0;
            deathCounter = 0;

            screenWidth = Game.GraphicsDevice.Viewport.Width;
            screenHeight = Game.GraphicsDevice.Viewport.Height;

            gameTimer = new GameTimer(360, (SpriteFont)_fonts[0]);
            InitLevel();
        }

        public void InitLevel()
        {
            levelLoader.LoadLevel(currentLevel);
            levelMap = new Map(Game, levelLoader.Map, textures);

            int screenWidth = Game.GraphicsDevice.Viewport.Width;
            int screenHeight = Game.GraphicsDevice.Viewport.Height;
            player = new Player(Game, (Texture2D)textures[playerIndex], sounds, 50, screenHeight - 52 - (screenHeight / 32));
            boltTexture = (Texture2D)textures[4];
        }

        /* procedure responsible for updating this level given an action (velocity should eventually be determined by player)*/
        public void Update(Action action, int velocity)
        {
<<<<<<< HEAD
            // would like to find a way to just call foreach i, i.Update(a, v) instead of having to explicitly deal with the map...
            if (shouldShiftScreen(action))
            {
                // update the map position when the background screen needs to be updated
                levelMap.Update(action, velocity);
                velocity = 0;
            }

            if (action == Action.throwBolt)
            {
                player.ThrowBolt();
                bolts.Add(new Bolt(Game, player.GetFacingDirection(), player.GetPosition().X, player.GetPosition().Y, boltTexture));
            }

            // update the player position when the player needs to change position on screen
            player.Update(action, velocity);
            player.HandleCollision(levelMap.GetNearbyTiles(player.GetPosition()));
            foreach (Bolt bolt in bolts)
            {
                bolt.Update(action, velocity);
                bolt.HandleCollision(levelMap.GetNearbyTiles(bolt.GetPosition()));
                if (bolt.expiryTime <= 0)
                {
                    bolts.Remove(bolt);
                    break;
                }
            }
            gameTimer.Update();
            
            if (player.DoneLevel)
            {
                // do some intermediate next level screen...
                currentLevel += 1;
                InitLevel();
=======
            // no need to perform update if the player died - get ready for some serious death-screen action
            if (!player.IsDead)
            {
                // would like to find a way to just call foreach i, i.Update(a, v) instead of having to explicitly deal with the map...
                if (shouldShiftScreen(action))
                {
                    // update the map position when the background screen needs to be updated
                    levelMap.Update(action, velocity);
                    velocity = 0;
                }

            	if (action == Action.throwBolt)
            	{
            	    player.ThrowBolt();
            	    bolts.Add(new Bolt(Game, Direction.right, player.GetPosition().X, player.GetPosition().Y, boltTexture));
            	}
	
	        // update the player position when the player needs to change position on screen
	        player.Update(action, velocity);
	        player.HandleCollision(levelMap.GetNearbyTiles(player.GetPosition()));
	
	        if (action == Action.boltUpdates)
	        {
	            foreach (Bolt bolt in bolts)
	            {
	                bolt.Update(action, velocity);
	            }
	        }
	    	
            	if (player.DoneLevel)
            	{
            	    // do some intermediate next level screen...
            	    currentLevel += 1;
            	    InitLevel();
            	}
>>>>>>> 154f9e36a71c70401042ec434d50d118fac2e877
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
<<<<<<< HEAD
            levelMap.Draw(spriteBatch);
            player.Draw(spriteBatch);
        
            foreach (Bolt bolt in bolts)
            {
                bolt.Draw(spriteBatch);
            }
            gameTimer.Draw(spriteBatch);

=======
            if (deathCounter < maxDeathCounter)
            {
		foreach (Bolt bolt in bolts)            
            	{
            	    bolt.Draw(spriteBatch);
            	}
            	 
                levelMap.Draw(spriteBatch);
                player.Draw(spriteBatch);
                if (player.IsDead)
                {
                    deathCounter += 1;
                }
            }
            else
            {
                spriteBatch.Draw((Texture2D)textures[3], new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);
            }
>>>>>>> 154f9e36a71c70401042ec434d50d118fac2e877
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
