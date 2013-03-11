using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame4
{
    abstract class ADynamicGameObject : DrawableGameComponent, IDynamicGameObject
    {
        // dynamic objects current position
        protected Rectangle position;

        // dynamic object's change in position
        protected int deltaY;
        protected int deltaX;

        public ADynamicGameObject(Game game) : base(game)
        {

        }

        public abstract Rectangle GetPosition();

        public abstract void Update(Action action, int velocity);

        public abstract void Draw(SpriteBatch spriteBatch);

        public abstract void HandleCollision(IList<ITile> obj);

        // procedure to detect which direction a collision is taking place with a rectangular object relative to a rectangular game object
        protected Direction determineCollisionType(Rectangle r)
        {
            // default to a bottom collision - this will get overwritten if there is a collision
            Direction direction = Direction.none;

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
                    direction = Direction.top;
                }
                else
                {
                    direction = Direction.left;
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
                    direction = Direction.top;
                }
                else
                {
                    direction = Direction.right;
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
                    direction = Direction.bottom;
                }
                else
                {
                    direction = Direction.left;
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
                    direction = Direction.bottom;
                }
                else
                {
                    direction = Direction.right;
                }
            }
            // if player's feet are touching the ground - this handles pure bottom collisions
            else if ((r.Top <= position.Bottom && r.Top > position.Bottom + deltaY) && ((r.Right >= position.Right && r.Left <= position.Right) || (r.Right >= position.Left && r.Left <= position.Left)))
            {
                // this is wrong and doesn't handle all of the possilbe cases! yay!
                direction = Direction.bottom;
            }
            // inner body collision - will ony get here if none of the corner collision cases have ocurred 
            else if (r.Intersects(position))
            {
                // pure top collision
                if (position.Top > r.Top && position.Top < r.Bottom)
                {
                    direction = Direction.top;
                }
                // pure left collision
                else if (position.Left < r.Right && position.Left > r.Left)
                {
                    direction = Direction.left;
                }
                // pure right collision
                else if (position.Right < r.Right && position.Right > r.Left)
                {
                    direction = Direction.right;
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
    }
}
