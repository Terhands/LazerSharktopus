using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame4
{
    class PlayerAnimation : IGameObject
    {
        IPlayer player;
        IList<ITile> tiles;
        int screenWidth;
        int AnimationVelocity;
        Action action;
        Random random;

        int hideCount;

        public PlayerAnimation(GameLoop game, IPlayer _player)
        {
            screenWidth = game.GraphicsDevice.Viewport.Width;

            AnimationVelocity = 1;
            action = Action.right;

            random = new Random();
            tiles = new List<ITile>();

            player = _player;
            player.ToggleHealthBar();
        }

        // the player will just run back and forth across the screen
        public void Update(Action _action, int _velocity)
        {
            if (player.GetPosition().Right < -10)
            {
                action = Action.right;
                AnimationVelocity = 1;
            }
            else if (player.GetPosition().Left > screenWidth + 15)
            {
                action = Action.left;
                AnimationVelocity = -1;
            }

            int hide = random.Next(0, 100);

            if (player.GetPosition().X + (player.GetPosition().Width /2) == screenWidth/2 && hideCount < 0)
            {
                hideCount = 100;
            }

            if (hideCount < 0)
            {
                player.Update(action, AnimationVelocity);
            }
            else if (hideCount == 0)
            {
                hideCount = -1;
                player.StopHiding();
                player.Update(action, AnimationVelocity);
            }
            else
            {
                player.Hide(tiles);
                hideCount -= 1;
            }
            
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            player.Draw(spriteBatch);
        }
    }
}
