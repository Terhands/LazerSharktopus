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
        protected Guard guard;

        protected const int playerIndex = 0;
        protected const int guardIndex = 3;
        protected Rectangle playerRange;


        public Level(GameLoop game, ArrayList textures) : base(game)
        {

            int screenWidth = Game.GraphicsDevice.Viewport.Width;
            int screenHeight = Game.GraphicsDevice.Viewport.Height;

            levelMap = new Map(game, "test.txt", textures);
            player = new Player(game, (Texture2D)textures[playerIndex], 50, screenHeight - 52 - (screenHeight / 32));
            guard = new Guard(game, (Texture2D)textures[guardIndex], 80, screenHeight - 52 - (screenHeight / 32), Action.right, 100);
            playerRange = new Rectangle((screenWidth * 2)/5, 0, screenWidth/5, screenHeight); 
        }

        /* procedure responsible for updating this level given an action (velocity should eventually be determined by player)*/
        public void Update(Action action, int velocity)
        {
            // would like to find a way to just call foreach i, i.Update(a, v) instead of having to explicitly deal with the map...
            if (shouldShiftScreen(action))
            {
                // update the map position when the background screen needs to be updated
                levelMap.Update(action, velocity);
                velocity = 0;
            }

            // update the player position when the player needs to change position on screen
            player.Update(action, velocity);
            player.HandleCollision(levelMap.GetNearbyTiles(player.GetPosition()));
            guard.Update(action, -velocity);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            levelMap.Draw(spriteBatch);
            player.Draw(spriteBatch);
            guard.Draw(spriteBatch);
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
