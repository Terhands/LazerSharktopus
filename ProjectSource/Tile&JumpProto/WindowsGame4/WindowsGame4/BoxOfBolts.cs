using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace WindowsGame4
{
    class BoxOfBolts : IDynamicGameObject
    {
        private Rectangle position;

        private int spriteWidth = 200;
        private int spriteHeight = 200;

        protected bool gathered;

        Texture2D boxOfBoltsTexture;

        public BoxOfBolts(Game game, int xPos, int yPos, Texture2D texture)
        {
            position = new Rectangle(xPos, yPos, spriteWidth / 4, spriteHeight / 5);
            boxOfBoltsTexture = texture;

            gathered = false;
        }

        public void HandleCollision(IList<ITile> tiles)
        {
            bool collided = false;

            foreach (ITile t in tiles)
            {
                if (t.isInCollision(this))
                {
                    collided = true;
                    break;
                }
            }

            if (collided)
            {
                gatherBox();
            }
        }

        public bool getGathered()
        {
            return gathered;
        }

        public void Update(GameTime gameTime)
        {
            // Nothing actually needs to be updated
        }

        public void Update(Action action, int myInt)
        {
        }

        public void gatherBox()
        {
            gathered = true;
        }

        public Rectangle GetPosition()
        {
            return position;
        }

        public void reposition(int deltaX)
        {
            position.X -= deltaX;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!gathered)
                spriteBatch.Draw(boxOfBoltsTexture, position, new Rectangle(0, 0, spriteWidth, spriteHeight), Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.4f);
        }
    }
}
