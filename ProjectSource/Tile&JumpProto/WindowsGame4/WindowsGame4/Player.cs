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
        protected Action facingDirection;

        protected bool isHidden;
        protected bool isJumping;
        protected bool isStopped;

        // may replace this with a jump meter object later on
        protected float jumpPower;
        protected const int maxJumpPower = 5;
        protected const float startFalling = -0.25f;

        protected int deltaY;
        protected int deltaX;

        public Player(Texture2D texture, int xStart, int yStart)
        {
            facingDirection = Action.right;
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
                jumpPower = startFalling;
            }

            // perform jump's update
            position.Y -= deltaY;
            isJumping = true;
        }

        public void ChargeJumpPower()
        {
            if (jumpPower < maxJumpPower)
            {
                jumpPower += 0.1f;
            }
        }

        public void ThrowBolt()
        {
            
        }

        public void Hide()
        {

        }

        /* make sure player isn't falling through platforms/walking through walls */
        public void HandleCollision(IList<ITile> tiles)
        {
            bool footCollision = false;
            // check that any intersections are only on passable tiles
            foreach (ITile t in tiles)
            {
                if(t.isInCollision(this))
                {
                    // have to break this up into cases - what type of intersection? top/bottom/right/left
                    // tile's top is between player's bottom and top
                    if (isJumping && t.getPosition().Top >= position.Bottom - Math.Abs(deltaY))
                    {
                        if(startFalling >= jumpPower)
                        {
                            position.Y = t.getPosition().Top - position.Height;

                            isJumping = false;
                            footCollision = true;
                            jumpPower = 0;
                        }
                    }
                }
                else if (t.getPosition().Top == position.Bottom)
                {
                    // no longer jumping so reset the jump charge meter
                    if (isJumping && startFalling >= jumpPower)
                    {
                        jumpPower = 0;
                        isJumping = false;
                    }

                    footCollision = true;
                }
            }

            // if the only tiles hitting the player's feet are passable, then they are falling - set jump power to initial falling speed
            if (!isJumping && !footCollision)
            {
                isJumping = true;
            }
        }

        public Rectangle GetPosition()
        {
            /* do not want to give this player's rectangle to another class that might change it... */
            Rectangle rec = new Rectangle(position.X, position.Y, position.Width, position.Height);
            return rec;
        }

        public void Update(Action direction, int velocity)
        {
            if (isJumping)
            {
                Jump();
            }

            switch (direction)
            {
                case Action.right:
                    facingDirection = direction;
                    position.X += velocity;
                    break;
                case Action.left:
                    facingDirection = direction;
                    position.X -= velocity;
                    break;
                // have to decide if we will implement ladder mechanics or rely on jumps
                case Action.up:
                    break;

                /* crouch to try and avoid detection */
                case Action.down:
                    Hide();
                    break;
                case Action.jump:
                    Jump();
                    break;
                case Action.chargeJump:
                    ChargeJumpPower();
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, source, Color.White);
        }
    }
}
