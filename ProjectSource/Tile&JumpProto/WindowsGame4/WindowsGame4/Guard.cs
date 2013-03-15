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

        protected int distractedTime;
        protected int maxDistractedTime;

        protected int guardCounter;
        protected int guardStartCount = 20;

        protected int patrolLength;
        protected int patrolBoundaryLeft;
        protected int patrolBoundaryRight;

        protected Texture2D sprite;
        protected Rectangle source;
        protected Direction facingDirection;

        protected int LOSRadius;
        protected int hearingRadius;

        protected bool isFalling;
        protected int velocity = 2;
        protected const int minFallingSpeed = -1;

        protected enum Behaviour { patrol, guard, distracted, goCheckThatShitOut, gotoPatrol };

        protected Behaviour currentBehaviour;


        protected Color debugColor;

        //took this from the player class, may need a different value
        protected const float spriteDepth = 0.95f;


        Vector2 eyePos;
        

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
            eyePos = new Vector2(position.Width/2, position.Height/4);
            sprite = texture;

            currentBehaviour = Behaviour.patrol;
            isFalling = false;
            guardCounter = -1;

            deltaX = 0;
            deltaY = 0;
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
            if (currentBehaviour == Behaviour.patrol)
            {
                Patrol();
            }
            else if (currentBehaviour == Behaviour.guard)
            {
                // chill out for a few and guard some shit
                StandWatch();
            }
            else if (currentBehaviour == Behaviour.goCheckThatShitOut)
            {
                // WTF WAS THAT?!?!?
            }
            else if (currentBehaviour == Behaviour.distracted)
            {
                // hmmm.... maybe I'm just going insane... and hearing things... like bolts
            }
            else if (currentBehaviour == Behaviour.gotoPatrol)
            {
                // use the ultimate power of the goto to get back to your starting route!
            }

            // guards fall straight down
            if (!isFalling)
            {
                position.X += deltaX;
            }
            else
            {
                Fall();
                position.Y -= deltaY;
            }
        }

        protected void Fall()
        {
            if (deltaY >= minFallingSpeed)
            {
                deltaY = minFallingSpeed;
            }
            else
            {
                deltaY -= deltaY * 5 / 3;
            }
        }

        protected void Patrol()
        {
            //move right until you reach your patrol, then turn around
            if (facingDirection == Direction.right)
            {
                if (position.X < patrolBoundaryRight)
                {
                    deltaX = velocity;
                }
                else if (position.X == patrolBoundaryRight)
                {
                    currentBehaviour = Behaviour.guard;
                }
            }
            //move left until you reach your patrol, then turn around
            else if (facingDirection == Direction.left)
            {
                if (patrolBoundaryLeft < position.X)
                {
                    deltaX = -1 * velocity;
                }
                else if (patrolBoundaryLeft == position.X)
                {
                    currentBehaviour = Behaviour.guard;
                }
            }
        }

        protected void StandWatch()
        {
            deltaX = 0;
            if (guardCounter < 0)
            {
                guardCounter = guardStartCount;
            }
            else if (guardCounter > 0)
            {
                guardCounter -= 1;
            }
            else
            {
                guardCounter = -1;
                currentBehaviour = Behaviour.patrol;
                if (facingDirection == Direction.right)
                {
                    facingDirection = Direction.left;
                }
                else
                {
                    facingDirection = Direction.right;
                }
            }
        }
       

        public override void HandleCollision(IList<ITile> tiles)
        {
            bool footCollision = false;

            // check that any intersections are only on passable tiles
            foreach (ITile t in tiles)
            {
                // padding the tile with a pixel on either side so the player cannot climb the walls
                Rectangle tilePos = t.getPosition();

                tilePos.X -= 1;
                tilePos.Y += 1;
                tilePos.Height -= 1;
                tilePos.Width += 2;

                Direction direction = determineCollisionType(tilePos);

                switch (direction)
                {
                    case Direction.bottom:
                        position.Y = t.getPosition().Top - position.Height;
                        footCollision = true;
                        break;
                    case Direction.top:
                        // this should never happen, our guards don't jump... yet.
                        break;
                    case Direction.left:
                        if (t.getCollisionBehaviour() == CollisionType.impassable)
                        {
                            // for some wierd reason with only 1 pixel of padding this breaks player's fall
                            position.X = t.getPosition().Right + 2;
                            deltaX = 0;
                        }
                        break;
                    case Direction.right:
                        if (t.getCollisionBehaviour() == CollisionType.impassable)
                        {
                            position.X = t.getPosition().Left - position.Width - 1;
                            deltaX = 0;
                        }
                        break;
                }
            }

            if (!footCollision)
            {
                isFalling = true;
                Fall();
            }
            else
            {
                isFalling = false;
            }
        }

        public void HandleHearing(IList<Bolt> bolts)
        {
            Direction hearingDirection;
            Bolt distractingBolt = null;
            foreach (Bolt bolt in bolts)
            {
               hearingDirection = determineRadialCollision(bolt.GetPosition(), 200);
               if (hearingDirection != Direction.none)
               {
                   distractingBolt = bolt;
                   debugColor = Color.Red;
                   break;
               }
            }

            int GoalDestination = distractingBolt.GetPosition().X;

        }

        //if he sees the player, the player should die.
        public void HandleVision(Player player)
        {

            Direction sightDirection = determineRadialCollision(player.GetPosition(), 100.0f);
          
            if (sightDirection == facingDirection)
            {
                //player.Kill();
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
            float deltaY = mapEyePos.Y - (r.Y + (r.Height / 2));
            float distance = (float)Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));

            debugColor = Color.White;

            // if the distance is less than the two radii, then the rectange is in collision with this Guard's collision radius
            if (distance <= recRadius + radius)
            {
                // now to get the collision direction l\u/r u=up, b=bottom, r=right, l=left
                //                                    l/b\r
                Vector2 destination = new Vector2(deltaX, deltaY);
                Vector2 vecDir = destination;// -mapEyePos;
                vecDir.Normalize();
                float angle = VectorToAngle(vecDir);

                while (angle < 0)
                    angle += 360;

                System.Console.WriteLine(angle);
                

                // may have to tweak the angle values it depends on how xna stores angles against world coords
                if (angle >= 45 && angle <= 135)
                {
                    direction = Direction.top;
                    debugColor = Color.Beige;
                    System.Console.WriteLine("top");
                }
                else if (angle >= 135 && angle <= 225)
                {
                    direction = Direction.right;
                    debugColor = Color.Crimson;
                    System.Console.WriteLine("right");
                 
                }
                else if (angle >= 225 && angle <= 315)
                {
                    direction = Direction.bottom;
                    debugColor = Color.Black;
                    System.Console.WriteLine("bottom");
                }
                else
                {
                    System.Console.WriteLine("left");
                    direction = Direction.left;
                    debugColor = Color.DarkMagenta;
                }
            }

            return direction;
        }

        protected float VectorToAngle(Vector2 v)
        {
            return (float)(Math.Atan2(v.Y, v.X) * (180 / Math.PI));
        }

        // get the normalized vector from the given angle - NOTE angle is probably required to be in radians /cry
        protected Vector2 AngleToVector(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public override void Draw(SpriteBatch spriteBatch){
            spriteBatch.Draw(sprite, position, source, debugColor, 0, new Vector2(0, 0), SpriteEffects.None, spriteDepth);
            
        }
        


    }
}
