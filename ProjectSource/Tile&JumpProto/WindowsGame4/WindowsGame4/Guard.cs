using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame4
{
    class Guard : ADynamicGameObject, IDynamicGameObject
    {
        
        protected int patrolLength;
        protected int patrolBoundaryLeft;
        protected int patrolBoundaryRight;

        protected Texture2D sprite;
        protected Rectangle source;
        protected Action facingDirection;

        protected int LOSRadius;
        protected int hearingRadius;
        protected int velocity = 1;
        

        Vector2 eyePos;

        public Guard(Game game, Texture2D texture, int xStart, int yStart, Action FacingDirectionStart, int patrolLength) : base(game)
        {
            facingDirection = FacingDirectionStart;
            this.patrolLength = patrolLength;
            if (facingDirection == Action.left)
            {
                patrolBoundaryRight = xStart;
                patrolBoundaryLeft = xStart - patrolLength;

            }
            else if (facingDirection == Action.right)
            {
                patrolBoundaryLeft = xStart;
                patrolBoundaryRight = xStart + patrolLength;
            }

            source = new Rectangle(10, 13, 84, 109);
            position = new Rectangle(xStart, yStart, 36, 52);
            sprite = texture;

            deltaX = 0;
            deltaY = 0;
        }

        public override Rectangle GetPosition()
        {
            return new Rectangle(position.X, position.Y, position.Width, position.Height);
        }

        public override void Update(Action direction, int velocity)
        {
            int extraVelocity = 0;
            switch (direction)
            {
                case Action.right:
                    extraVelocity += velocity;
                    break;
                case Action.left:
                    extraVelocity -= velocity;
                    break;
            }


            if (facingDirection == Action.right)
            {
                if (position.X < patrolBoundaryRight)
                {
                    position.X += this.velocity;
                    position.X += extraVelocity;
                }
                else if (position.X == patrolBoundaryRight)
                {
                    facingDirection = Action.left;
                    position.X += extraVelocity;

                }

            }
            else if (facingDirection == Action.left)
            {
                if (patrolBoundaryLeft < position.X)
                {
                    position.X -= this.velocity;
                    position.X += extraVelocity;
                }
                else if (patrolBoundaryLeft == position.X)
                {
                    facingDirection = Action.right;
                    position.X += extraVelocity;
                }
            }

        }

        public override void HandleCollision(IList<ITile> obj)
        {

        }

        protected CollisionDirection determineRadialCollision(Rectangle r, float radius)
        {
            CollisionDirection direction = CollisionDirection.none;

            // hopefully close enough & easier than having to handle circle/rectangle collisions
            // let recRadius be half the average of the width & height of the rectangle
            float recRadius = 0.25f * ((float)(r.Width + r.Height));

            float distance = (float)Math.Sqrt(Math.Pow(eyePos.X - (r.X + (r.Width/2)), 2) + Math.Pow(eyePos.Y - (r.Y + (r.Height/2)), 2));

            // if the distance is less than the two radii, then the rectange is in collision with this Guard's collision radius
            if (distance <= recRadius + radius)
            {
                // now to get the collision direction l\u/r u=up, b=bottom, r=right, l=left
                //                                    l/b\r
                Vector2 destination = new Vector2((r.X + r.Width)/2, (r.Y + r.Height)/2);
                Vector2 vecDir = destination - eyePos;
                vecDir.Normalize();
                float angle = VectorToAngle(vecDir);

                // may have to tweak the angle values it depends on how xna stores angles againsts world coords
                if ((angle <= 45 && angle >= 0) || (angle >= 315))
                {
                    direction = CollisionDirection.right;
                }
                else if (angle >= 45 && angle <= 135)
                {
                    direction = CollisionDirection.top;
                }
                else if (angle >= 135 && angle <= 225)
                {
                    direction = CollisionDirection.left;
                }
                else
                {
                    direction = CollisionDirection.bottom;
                }
            }

            return direction;
        }

        protected float VectorToAngle(Vector2 v)
        {
            return (float)Math.Atan2(v.Y, v.X);
        }

        // get the normalized vector from the given angle
        protected Vector2 AngleToVector(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public override void Draw(SpriteBatch spriteBatch){
            spriteBatch.Draw(sprite, position, source, Color.Cyan);
        }


    }
}
