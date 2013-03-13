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
        protected Direction facingDirection;

        protected int LOSRadius;
        protected int hearingRadius;
        protected int velocity = 1;


        protected Color debugColor;

        //took this from the player class, may need a different value
        protected const float spriteDepth = 0.5f;


        Vector2 eyePos = new Vector2(46.0f, 18.0f);
        

        public Guard(Game game, Texture2D texture, int xStart, int yStart, Direction FacingDirectionStart, int patrolLength) : base(game)
        {
            debugColor = Color.White;
            facingDirection = FacingDirectionStart;
            this.patrolLength = patrolLength;
            if (facingDirection == Direction.left)
            {
                patrolBoundaryRight = xStart;
                patrolBoundaryLeft = xStart - patrolLength;

            }
            else if (facingDirection == Direction.right)
            {
                patrolBoundaryLeft = xStart;
                patrolBoundaryRight = xStart + patrolLength;
            }

            source = new Rectangle(0, 0, 83, 108);
            position = new Rectangle(xStart, yStart, 36, 52);
            sprite = texture;

            //deltaX = 0;
            //deltaY = 0;
        }

        public override Rectangle GetPosition()
        {
            return new Rectangle(position.X, position.Y, position.Width, position.Height);
        }

        public override void Update(Action direction, int velocity)
        {
            /*move guard and boundaries with the map*/
            position.X -= velocity;
            patrolBoundaryLeft -= velocity;
            patrolBoundaryRight -= velocity;
        }

        public override void Update(GameTime gameTime)
        {
            //move right until you reach your patrol, then turn around
            if (facingDirection == Direction.right)
            {
                if (position.X < patrolBoundaryRight)
                {
                    position.X += this.velocity;
                }
                else if (position.X == patrolBoundaryRight)
                {
                    facingDirection = Direction.left;

                }

            }
                //move left until you reach your patrol, then turn around
            else if (facingDirection == Direction.left)
            {
                if (patrolBoundaryLeft < position.X)
                {
                    position.X -= this.velocity;

                }
                else if (patrolBoundaryLeft == position.X)
                {
                    facingDirection = Direction.right;

                }
            }
        }

       

        public override void HandleCollision(IList<ITile> tile)
        {
          //  determineRadialCollision(
        }

        public void HandleCollision(IList<Bolt> bolt)
        {
            
        }

        public void HandleCollision(IPlayer player)
        {

            Direction sightDirection = determineRadialCollision(player.GetPosition(), 100.0f);

            
            if (sightDirection == Direction.left)
            {
                debugColor = Color.Cyan;
            }

            if (sightDirection == Direction.right)
            {

                debugColor = Color.DarkGoldenrod;
            }

            if (sightDirection == Direction.top)
            {
                debugColor = Color.Crimson;
            }

            if (sightDirection == Direction.bottom)
            {
                debugColor = Color.ForestGreen;
            }
        }


       
        protected Direction determineRadialCollision(Rectangle r, float radius)
        {
            Direction direction = Direction.none;

           Vector2  mapEyePos = new Vector2(this.position.X + eyePos.X, this.position.Y + eyePos.Y);

            // hopefully close enough & easier than having to handle circle/rectangle collisions
            // let recRadius be half the average of the width & height of the rectangle
            float recRadius = 0.25f * ((float)(r.Width + r.Height));

            //distance between the x and y position of the guards eyes and the middle of the player
            float deltaX = mapEyePos.X - (r.X + (r.Width / 2));
            float deltaY = mapEyePos.Y - (r.X + (r.Height / 2));
            float distance = (float)Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));

            // if the distance is less than the two radii, then the rectange is in collision with this Guard's collision radius
            if (distance <= recRadius + radius)
            {
                // now to get the collision direction l\u/r u=up, b=bottom, r=right, l=left
                //                                    l/b\r
                Vector2 destination = new Vector2(deltaX, deltaY);
                Vector2 vecDir = destination - eyePos;
                vecDir.Normalize();
                float angle = VectorToAngle(vecDir);
                

                // may have to tweak the angle values it depends on how xna stores angles againsts world coords
                if ((angle <= 45 && angle >= 0) || (angle >= 315))
                {
                    direction = Direction.top;
                   
                }
                else if (angle >= 45 && angle <= 135)
                {
                    direction = Direction.right;
                 
                }
                else if (angle >= 135 && angle <= 225)
                {
                    direction = Direction.bottom;
                    System.Console.WriteLine("dir = right");
                }
                else
                {
                    System.Console.WriteLine("dir = bottom");
                    direction = Direction.left;
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
            spriteBatch.Draw(sprite, position, source, debugColor, 0, new Vector2(0, 0), SpriteEffects.None, spriteDepth);
            
        }
        


    }
}
