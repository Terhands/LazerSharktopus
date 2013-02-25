using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame4
{
    class Player : ADynamicGameObject, IPlayer
    {
        protected Texture2D sprite;
        protected Rectangle source;
        protected Action facingDirection;

        protected bool isHidden;
        protected bool isJumping;
        protected bool isStopped;

        // may replace this with a jump meter object later on
        protected float jumpPower;
        protected const float maxInitialJumpPower = 7;
        protected const float minInitialJumpPower = 3f;
        protected const float startFalling = -0.25f;

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

        public void Jump()
        {
            // scale the jump so you can go high fast, but fall a bit slower - less sudden
            if (jumpPower > 0.00001 || jumpPower < 0)
            {
                deltaY = (int)(5 / 3 * jumpPower);
                if (jumpPower > 0)
                {
                    jumpPower = jumpPower - 0.25f;
                }
                else
                {
                    jumpPower = jumpPower - 0.1f;
                }
            }
            else
            {
                deltaY = 0;
                jumpPower = startFalling;
            }

            // when jumping the player has no more control of their direction until they land
            position.Y -= deltaY;
            position.X += deltaX;
            isJumping = true;
        }

        public void ChargeJumpPower()
        {
            if (jumpPower < maxInitialJumpPower && !isJumping)
            {
                if (jumpPower < minInitialJumpPower)
                {
                    jumpPower = minInitialJumpPower;
                }

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
        public override void HandleCollision(IList<ITile> tiles)
        {

            IList<Rectangle> rectangles = new List<Rectangle>();
            bool footCollision = false;
            // check that any intersections are only on passable tiles
            foreach (ITile t in tiles)
            {

                CollisionDirection direction = determineCollisionType(t.getPosition());

                switch (direction)
                {
                    case CollisionDirection.bottom:
                        position.Y = t.getPosition().Top - position.Height;
                        footCollision = true;

                        if (isJumping)
                        {
                            isJumping = false;
                            jumpPower = 0;
                        }

                        break;
                    case CollisionDirection.top:
                        position.Y = t.getPosition().Bottom;
                        if (isJumping && startFalling < jumpPower)
                        {
                            jumpPower = startFalling;
                        }
                        break;
                    case CollisionDirection.left:
                        position.X = t.getPosition().Right + 2;
                        deltaX = 0;
                        break;
                    case CollisionDirection.right:
                        position.X = t.getPosition().Left - position.Width;
                        deltaX = 0;
                        break;
                }
            }

            
            // if the player is not jumping and has no tiles under their feet, they start falling
            if (!isJumping && !footCollision)
            {
                isJumping = true;
                jumpPower = startFalling;
            }
 
        }

        public override Rectangle GetPosition()
        {
            /* do not want to give this player's rectangle to another class that might change it... */
            return new Rectangle(position.X, position.Y, position.Width, position.Height);
        }

        public override void Update(Action direction, int velocity)
        {
            switch (direction)
            {
                case Action.right:
                    facingDirection = direction;

                    if (!isJumping)
                    {
                        deltaX = velocity;
                        position.X += deltaX;
                    }
                    break;
                case Action.left:
                    facingDirection = direction;

                    if (!isJumping)
                    {
                        deltaX = velocity;
                        position.X += deltaX;
                    }
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
                case Action.none:
                    if (isJumping)
                    {
                        Jump();
                    }
                    else
                    {
                        deltaX = 0;
                    }
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, source, Color.White);
        }
    }
}
