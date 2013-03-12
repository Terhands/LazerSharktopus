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

        protected Rectangle position;
        protected Rectangle source;

        protected const int spriteWidth = 7;
        protected const int spriteHeight = 31;

        Texture2D flameTexture;

        protected const int flamesSwitchTime = 15;
        protected int flamesAnimation;

        protected int maxSprite = 4;
        protected int minSprite = 0;
        protected int currSprite;

        protected int[] xPosition = { 1, 9, 17, 25, 34 };

        protected bool loopForward;

        public Torch(Game game, Texture2D texture, int xStart, int yStart) : base(game)
        {
            loopForward = true;
            currSprite = minSprite;

            flameTexture = texture;
            position = new Rectangle(xStart, yStart, 15, 25);
            source = new Rectangle(xPosition[currSprite], 1, spriteWidth, spriteHeight);
        }

        public void Update(Action action, int velocity)
        {
            position.X -= velocity;
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

                source.X = xPosition[currSprite];
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(flameTexture, position, source, Color.White, 0, new Vector2(0,0), SpriteEffects.None, 0.9f);
        }

    }
}
