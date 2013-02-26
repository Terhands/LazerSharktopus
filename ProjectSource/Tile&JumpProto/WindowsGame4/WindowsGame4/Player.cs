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
        protected JumpMeter jumpMeter;
        protected int playerPadding = 8;

        // the speed that player starts falling
        protected const float startFalling = -0.25f;

        public Player(Game game, Texture2D texture, int xStart, int yStart)
            : base(game)
        {
            facingDirection = Action.right;
            source = new Rectangle(251, 142, 746 - 251, 805 - 142);
            position = new Rectangle(xStart, yStart, 36, 52);
            sprite = texture;

            int xCenter = xStart + (position.Width / 2);
            int yCenter = yStart - playerPadding;
            this.jumpMeter = new JumpMeter(game, xCenter, yCenter);

            isHidden = false;
            isJumping = false;
            isStopped = true; // will we need to know this?? Maybe for a funny animation if you take too long...

            deltaX = 0;
            deltaY = 0;
        }

        public void Jump()
        {
            // scale the jump so you can go high fast, but fall a bit slower - less sudden
            if (jumpMeter.JumpPower > 0.00001 || jumpMeter.JumpPower < 0)
            {
                deltaY = (int)(5 / 3 * jumpMeter.JumpPower);
                if (jumpMeter.JumpPower > 0)
                {
                    jumpMeter.drainJumpPower(0.25f);
                }
                else
                {
                    jumpMeter.drainJumpPower(0.1f);
                }
            }
            else
            {
                deltaY = 0;
                jumpMeter.JumpPower = startFalling;
            }

            // when jumping the player has no more control of their direction until they land
            position.Y -= deltaY;
            position.X += deltaX;
            isJumping = true;
        }

        public void ChargeJumpPower()
        {
            if (!isJumping)
            {
                jumpMeter.chargeJumpPower();
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
                            jumpMeter.reset();
                        }

                        break;
                    case CollisionDirection.top:
                        if (t.getCollisionBehaviour() == CollisionType.impassable)
                        {
                            position.Y = t.getPosition().Bottom;
                            if (isJumping && startFalling < jumpMeter.JumpPower)
                            {
                                jumpMeter.JumpPower = startFalling;
                            }
                        }
                        break;
                    case CollisionDirection.left:
                        if (t.getCollisionBehaviour() == CollisionType.impassable)
                        {
                            position.X = t.getPosition().Right + 2;
                            deltaX = 0;
                        }
                        break;
                    case CollisionDirection.right:
                        if (t.getCollisionBehaviour() == CollisionType.impassable)
                        {
                            position.X = t.getPosition().Left - position.Width - 2;
                            deltaX = 0;
                        }
                        break;
                }
            }


            // if the player is not jumping and has no tiles under their feet, they start falling
            if (!isJumping && !footCollision)
            {
                isJumping = true;
                jumpMeter.JumpPower = startFalling;
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
                        // once the player has landed the user regains control of movement
                        deltaX = 0;
                    }
                    break;
            }

            jumpMeter.setMeterPosition(position.X + (position.Width / 2), position.Y - playerPadding);
            jumpMeter.Update(Action.none, 0);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, source, Color.White);
            jumpMeter.Draw(spriteBatch);
        }
    }
}
