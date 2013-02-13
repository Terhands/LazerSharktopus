using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame4
{
    class Player : IPlayer
    {
        protected Texture2D sprite;
        protected Rectangle source;
        protected Rectangle position;
        protected Direction facingDirection;

        protected bool isHidden;
        protected bool isJumping;
        protected bool isStopped;

        // may replace this with a jump meter object later on
        protected float jumpPower;
        protected const int maxJumpPower = 10;

        protected int deltaY;
        protected int deltaX;

        public Player(Texture2D texture, int xStart, int yStart)
        {
            facingDirection = Direction.right;
            source = new Rectangle(251, 142, 746 - 251, 805 - 142);
            position = new Rectangle(xStart, yStart, 36, 52);
            sprite = texture;

            isHidden = false;
            isJumping = false;
            isStopped = true; // will we need to know this?? Maybe for a funny animation if you take too long...

            jumpPower = 0;

            deltaX = 0;
            deltaY = 0;
        }

        // need to fix this & handle when player lands
        public void Jump()
        {
            if (jumpPower > 0.00001 || jumpPower < 0)
            {
                deltaY = 3 * (int)jumpPower;
                jumpPower = jumpPower - 0.2f;
            }
            else
            {
                deltaY = -1;
                jumpPower = -0.25f;
            }

            isJumping = true;
        }

        public void ChargeJumpPower()
        {
            jumpPower += 0.1f;
        }

        public void ThrowBolt()
        {
            
        }

        public void Hide()
        {

        }

        public void HandleCollision(IGameObject obj)
        {
            
        }

        public void Update(Direction direction, int velocity)
        {
            if (isJumping)
            {
                position.Y -= deltaY;
                Jump();
            }

            switch (direction)
            {
                case Direction.right:
                    facingDirection = direction;
                    break;
                case Direction.left:
                    facingDirection = direction;
                    break;
                // have to decide if we will implement ladder mechanics or rely on jumps
                case Direction.up:
                    break;

                /* crouch to try and avoid detection */
                case Direction.down:
                    Hide();
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, source, Color.White);
        }
    }
}
