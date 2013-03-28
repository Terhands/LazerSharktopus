using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame4
{
    class Torch : DrawableGameComponent, IGameObject
    {

        protected Rectangle positionBase;
        protected Rectangle sourceBase;

        protected Rectangle positionNimbus;
        protected Rectangle sourceNimbus;

        protected const int spriteWidth = 7;
        protected const int spriteHeight = 31;

        protected const int torchWidth = 14;
        protected const int torchHeight = 25;

        Texture2D[] flameTextures;

        protected const int flamesSwitchTime = 15;
        protected int flamesAnimation;

        protected const int nimbusRadius = 30;
        protected const int flameOffset = 3;

        protected int maxSprite = 4;
        protected int minSprite = 0;
        protected int currSprite;

        protected int[] xPosition = { 1, 9, 17, 25, 34 };

        protected bool loopForward;

        public Torch(Game game, Texture2D[] texture, int xStart, int yStart) : base(game)
        {
            loopForward = true;
            currSprite = minSprite;

            flameTextures = texture;
            positionBase = new Rectangle(xStart, yStart, torchWidth, torchHeight);
            sourceBase = new Rectangle(xPosition[currSprite], 1, spriteWidth, spriteHeight);

            positionNimbus = new Rectangle(xStart + (torchWidth / 2) - (nimbusRadius), yStart - flameOffset - nimbusRadius, nimbusRadius * 2, nimbusRadius * 2); 
            sourceNimbus = new Rectangle(0, 0, 32, 32);
        }

        public void Update(Action action, int velocity)
        {
            positionBase.X -= velocity;
            positionNimbus.X -= velocity;
        }

        public void HandleCollision(IPlayer player)
        {
            int playerRadius = (player.GetPosition().Width) / 2;

            // the center of the player's bounding circle
            int x1 = player.GetPosition().X + (player.GetPosition().Width/2);
            int y1 = player.GetPosition().Y + (player.GetPosition().Height/2);

            // the center of the torch's bounding circle for the nimbus
            int x2 = positionNimbus.X + (positionNimbus.Width/2);
            int y2 = positionNimbus.Y + (positionNimbus.Height/2);

            int dx = x1 - x2;
            int dy = y1 - y2;

            int distance = (int)Math.Sqrt((dx*dx) + (dy*dy));

            if (distance < playerRadius + nimbusRadius && player.HiddenPercent > 0)
            {
                // if the player is hitting a torch his hidden percent drops
                player.HiddenPercent -= 0.49f;
            }
            
        }

        public void Update(GameTime gameTime)
        {
            flamesAnimation += 1;

            if (flamesAnimation % flamesSwitchTime == 0)
            {
                flamesAnimation = 0;

                if (loopForward)
                {
                    currSprite += 1;
                    if (currSprite == maxSprite)
                    {
                        loopForward = false;
                    }
                }
                else
                {
                    currSprite -= 1;
                    if (currSprite == minSprite)
                    {
                        loopForward = true;
                    }
                }

                sourceBase.X = xPosition[currSprite];
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(flameTextures[0], positionBase, sourceBase, Color.White, 0, new Vector2(0,0), SpriteEffects.None, 0.7f);
            spriteBatch.Draw(flameTextures[1], positionNimbus, sourceNimbus, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0.99f);
        }

    }
}
