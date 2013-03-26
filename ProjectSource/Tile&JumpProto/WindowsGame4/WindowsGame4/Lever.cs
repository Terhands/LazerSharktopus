using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame4
{
    class Lever : IDynamicGameObject
    {
        /* Each lever is a switch or a button. Switchers can be turned off and on, buttons are pressed multiple times */
        public enum LeverType { switcher, button };
        /* Only matters if it's a switch - off has the handle facing left or down, on has it facing right or up */
        public enum Status { on, off };

        private Rectangle location;
        private int gridX;
        private int gridY;
        private LeverType type;
        private Status status;

        private const int spriteWidth = 48;
        private const int spriteHeight = 48;


        
        Texture2D leverTexture;

        public Lever(Game game, int xPos, int yPos, int _gridX, int _gridY, LeverType leverType, Texture2D texture)
        {
            location = new Rectangle(xPos, yPos, spriteWidth/2, spriteHeight/2);
            type = leverType;
            status = Status.off;
            leverTexture = texture;

            gridX = _gridX;
            gridY = _gridY;
        }

        public void Update(GameTime gameTime)
        {
            /* The lever object itself doesn't actually do anything each round.
             * Any changes to it are called from the level controller.
             * Any changes in things that the lever operates happen in those objects (gates, etc).
             */
        }

        public void Update(Action action, int myInt)
        {
        }

        public void flip()
        {
            if (status == Status.off) status = Status.on;
            else status = Status.off;

            // Now cycle through each object this acts on and activate it
        }

        public void reposition(int deltaX)
        {
            location.X -= deltaX;
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

            if (collided) flip();
        }

        public Rectangle GetPosition()
        {
            return location;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (status == Status.off)
                spriteBatch.Draw(leverTexture, location, new Rectangle(0, 0, spriteWidth, spriteHeight), Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.2f);
            else
                spriteBatch.Draw(leverTexture, location, new Rectangle(0, 0, spriteWidth, spriteHeight), Color.White, 0f, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0.2f);
        }
    }
}
