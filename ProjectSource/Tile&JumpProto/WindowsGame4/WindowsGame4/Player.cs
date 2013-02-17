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
        protected const float maxInitialJumpPower = 7;
        protected const float minInitialJumpPower = 3f;
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
                deltaY = 1;
                jumpPower = startFalling;
            }

            // perform jump's update
            position.Y -= deltaY;
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
        public void HandleCollision(IList<ITile> tiles)
        {
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

                        // no longer jumping so reset the jump charge meter
                        if (isJumping && startFalling >= jumpPower)
                        {
                            jumpPower = 0;
                            isJumping = false;
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
                        position.X = t.getPosition().Right;
                        break;
                    case CollisionDirection.right:
                        position.X = t.getPosition().Left - position.Width;
                        break;
                }
            }

            // if the player is not jumping and has no tiles under their feet, they start falling
            if (!isJumping && !footCollision)
            {
                isJumping = true;
            }
        }

        protected CollisionDirection determineCollisionType(Rectangle r)
        {
            // default to a bottom collision - this will get overwritten if there is a collision
            CollisionDirection direction = CollisionDirection.none;

            // players top left corner is in collision with r's bottom right corner
            if ((r.Bottom > position.Top && position.Top > r.Top) && (r.Right > position.Left && r.Left < position.Left))
            {

                // the line segment made up of the change in x, y from the players top left corner
                Point p1 = new Point(position.Left - deltaX, position.Top + deltaY);
                Point p2 = new Point(position.Left, position.Top);

                Point intersectionPoint = getLineCollisionLocation(p1, p2, new Point(r.Left, r.Bottom), new Point(r.Right, r.Bottom));

                // if there was a collision with the player's top left corner path and the tile's bottom
                if (intersectionPoint.X != -1 && intersectionPoint.Y != -1)
                {
                    direction = CollisionDirection.top;
                }
                else
                {
                    direction = CollisionDirection.left;
                }
            }
            // player's top right is in collision with r's bottom left corner
            else if ((r.Bottom > position.Top && position.Top > r.Top) && (r.Right > position.Right && r.Left < position.Right))
            {
                Point p1 = new Point(position.Right - deltaX, position.Top + deltaY);
                Point p2 = new Point(position.Right, position.Top);

                Point intersectionPoint = getLineCollisionLocation(p1, p2, new Point(r.Left, r.Bottom), new Point(r.Right, r.Bottom));

                // if there was a collision with the player's top right corner path and the tile's bottom
                if (intersectionPoint.X != -1 && intersectionPoint.Y != -1)
                {
                    direction = CollisionDirection.top;
                }
                else
                {
                    direction = CollisionDirection.right;
                }
            }
            // player's bottom left is in collision with r's top right corner
            else if ((r.Top < position.Bottom && r.Bottom > position.Bottom) && (r.Right > position.Left && r.Left < position.Left))
            {
                Point p1 = new Point(position.Left - deltaX, position.Bottom + deltaY);
                Point p2 = new Point(position.Left, position.Bottom);

                Point intersectionPoint = getLineCollisionLocation(p1, p2, new Point(r.Right, r.Top), new Point(r.Left, r.Top));

                if (intersectionPoint.X != -1 && intersectionPoint.Y != -1)
                {
                    direction = CollisionDirection.bottom;
                }
                else
                {
                    direction = CollisionDirection.left;
                }
            }
            // player's bottom right is in collision with r's top left corner
            else if ((r.Top < position.Bottom && r.Bottom > position.Bottom) && (r.Right > position.Right && r.Left < position.Right))
            {
                Point p1 = new Point(position.Right - deltaX, position.Bottom + deltaY);
                Point p2 = new Point(position.Right, position.Bottom);

                Point intersectionPoint = getLineCollisionLocation(p1, p2, new Point(r.Right, r.Top), new Point(r.Left, r.Top));

                if (intersectionPoint.X != -1 && intersectionPoint.Y != -1)
                {
                    direction = CollisionDirection.bottom;
                }
                else
                {
                    direction = CollisionDirection.right;
                }
            }
            // if player's feet are touching the ground - this handles pure bottom collisions
            else if (r.Top <= position.Bottom && r.Top > position.Bottom + deltaY)// && ((r.Right > position.Right && r.Left < position.Right) || (r.Right > position.Left && r.Left < position.Left)))
            {
                // this is wrong and doesn't handle all of the possilbe cases! yay!
                direction = CollisionDirection.bottom;
            }
            // inner body collision - will ony get here if none of the corner collision cases have ocurred 
            else if (r.Intersects(position))
            {
                // pure top collision
                if (position.Top > r.Top && position.Top < r.Bottom)
                {
                    direction = CollisionDirection.top;
                }
                // pure left collision
                else if (position.Left < r.Right && position.Left > r.Left)
                {
                    direction = CollisionDirection.left;
                }
                // pure right collision
                else if (position.Right < r.Right && position.Right > r.Left)
                {
                    direction = CollisionDirection.right;
                }
            }

            return direction;
        }

        protected Point getLineCollisionLocation(Point p1, Point p2, Point q1, Point q2)
        {
            /*
             * Line segment intersection
             *(Px, Py) = (
             *             ((x1y2 - y1x2)(x3 - x4) - (x1-x2)(x3y4 - y3x4))
             *             -----------------------------------------------
             *               ((x1 - x2)(y3 - y4) - (y1 - y2)(x3 - x4))
             *           ,
             *             ((x1y2 - y1x2)(y3 - y4) - (y1 - y2)(x3y4 - y3x4))
             *             -------------------------------------------------
             *                  ((x1 - x2)(y3 - y4) - (y1 - y2)(x3 - x4))
             *           )
             */

            Point intersectionPoint = new Point();

            int denominator = (p1.X - p2.X) * (q1.Y - q2.Y) - (p1.Y - p2.Y) * (q1.X - q2.X);

            if (denominator != 0)
            {
                intersectionPoint.X = (((p1.X * p2.Y) - (p1.Y * p2.X)) * (q1.X - q2.X) - (p1.X - p2.X) * ((q1.X * q2.Y) - (q1.Y * q2.X))) / denominator;
                intersectionPoint.Y = (((p1.X * p2.Y) - (p1.Y * p2.X)) * (q1.Y - q2.Y) - (p1.Y - p2.Y) * ((q1.X * q2.Y) - (q1.Y * q2.X))) / denominator;

                if (!pointIsOnLine(intersectionPoint, p1, p2) || !pointIsOnLine(intersectionPoint, q1, q2))
                {
                    intersectionPoint.X = -1;
                    intersectionPoint.Y = -1;
                }
            }
            else
            {
                // there is no position -1,-1 on the screen, so this indicates that the player did not move since the last check, so the collision would already have been handled
                intersectionPoint.X = -1;
                intersectionPoint.Y = -1;
            }

            return intersectionPoint;
        }

        // check to see if a is on the line segment between p1 and p2
        protected bool pointIsOnLine(Point a, Point p1, Point p2)
        {
            bool result = false;

            // a.x is between p1.x, and p2.x
            if ((a.X <= p1.X && a.X >= p2.X) || (a.X <= p2.X && a.X >= p1.X))
            {
                if ((a.Y <= p1.Y && a.Y >= p2.Y) || (a.Y <= p2.Y && a.Y >= p1.Y))
                {
                    result = true;
                }
            }

            return result;

        }

        public Rectangle GetPosition()
        {
            /* do not want to give this player's rectangle to another class that might change it... */
            return new Rectangle(position.X, position.Y, position.Width, position.Height);
        }

        public void Update(Action direction, int velocity)
        {
            if (isJumping)
            {
                Jump();
            }

            // for now - eventually get player to be handling their own run speed
            deltaX = velocity;

            switch (direction)
            {
                case Action.right:
                    facingDirection = direction;
                    position.X += deltaX;
                    break;
                case Action.left:
                    facingDirection = direction;
                    position.X -= deltaX;
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
