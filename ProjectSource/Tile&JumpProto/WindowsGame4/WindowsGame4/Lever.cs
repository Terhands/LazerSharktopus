using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;

namespace WindowsGame4
{
    class Lever : IDynamicGameObject
    {
        /* Each lever is a switch or a button. Switchers can be turned off and on, buttons are pressed multiple times */
        public enum LeverType { switcher, button };
        /* Only matters if it's a switch - off has the handle facing left or down, on has it facing right or up */
        public enum Status { on, off };

        private Rectangle position;

        private LeverType type;
        private Status status;

        private int spriteWidth = 48;
        private int spriteHeight = 48;

        private List<Gate> gatesControlled;
        
        Texture2D leverTexture;

        public Lever(Game game, int xPos, int yPos, int screenWidth, int screenHeight, LeverType leverType, List<Gate> gates, Texture2D texture)
        {
            position = new Rectangle(xPos, yPos, spriteWidth/2, spriteHeight/2);
            type = leverType;
            status = Status.off;
            leverTexture = texture;

            gatesControlled = gates;
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

            foreach (Gate gate in gatesControlled)
            {
                gate.flip();
            }
        }

        public void reposition(int deltaX)
        {
            position.X -= deltaX;
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
                flip();
            }
        }

        public Rectangle GetPosition()
        {
            return position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (status == Status.off)
                spriteBatch.Draw(leverTexture, position, new Rectangle(0, 0, spriteWidth, spriteHeight), Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.2f);
            else
                spriteBatch.Draw(leverTexture, position, new Rectangle(0, 0, spriteWidth, spriteHeight), Color.White, 0f, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0.2f);
        }
    }
}
