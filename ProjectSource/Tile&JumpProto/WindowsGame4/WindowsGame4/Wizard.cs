using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame4
{
    class Wizard : ADynamicGameObject, IGuard
    {
        protected int invisibleY; 
        //distraction
        protected int distractionCount;
        protected int maxDistractedCount = 40;

        //keep track of how long the guard stands at the end of his patrol
        protected int guardCounter;
        protected int guardStartCount = 20;

        protected int getBackCounter;
        //patrol settings
        protected int patrolLength;
        protected int patrolBoundaryLeft;
        protected int patrolBoundaryRight;
        protected int patrolY;
       
        //how long guard is distracted
        protected int distractionX;

        //sprite thingees 
        protected Texture2D sprite;
        protected Rectangle source;
        protected Direction facingDirection;

        //detection settings
        protected const int LOSRadius = 150;
        protected const int hearingRadius = 150;
        protected const float LOSSlope = 0.268f;

        //gravity and movement and whatever oh my!
        protected bool isFalling;
        protected int velocity = 1;
        protected const int minFallingSpeed = -1;

        //guard states
        protected enum Behaviour { patrol, guard, distracted, goCheckThatShitOut, gotoPatrol, wander, teleport };
        protected Behaviour currentBehaviour;


        //TODO: remember to get rid of this once they work
        //actually just change it because right now it makes the guard invisible
        protected Color debugColor;

        //took this from the player class, may need a different value
        protected const float spriteDepth = 0.95f;

        protected const int spriteY = 1;
        protected const int spriteHeight = 28;

        protected int[] spriteX = { 1, 16, 32, 48, 64, 80, 96, 111, 131, 153, 173, 196, 214, 238 };
        protected int[] spriteWidth = { 13, 14, 14, 14, 14, 14, 14, 19, 21, 19, 22, 17, 23, 12 };

        protected int standingIndex = 0;
        protected int walkingIndex = 1;
        protected int teleportIndex = 6;

        protected int walkCounter;
        protected int currWalkingIndex;
        //position of the guards eyes relative to his source rectangle
        Vector2 eyePos;
        

        public Wizard(Game game, Texture2D texture, int xStart, int yStart, Direction FacingDirectionStart, int patrolLength) : base(game)
        {
            //assumes that the level is never going to be ridiculously huge
            invisibleY = yStart - 1000;
            //guard should be normal 
            debugColor = Color.White;

            //direction guard starts walking in 
            facingDirection = FacingDirectionStart;

            //distance the guards control should cover
            this.patrolLength = patrolLength;

            patrolY = yStart;
            /*patrol path extends from starting position in the direction 
             * the guard is initially facing in to the length of the path*/
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

            // default to the basic standing sprite
            source = new Rectangle(spriteX[standingIndex], spriteY, spriteWidth[standingIndex], spriteHeight);
            position = new Rectangle(xStart, yStart, source.Width*2, source.Height*2);
            eyePos = new Vector2(position.Width/2, position.Height/2);
            sprite = texture;

            //guard is initially patrolling
            currentBehaviour = Behaviour.patrol;
            
            isFalling = false;

            //guard count inactive when not at the patrol boundaries
            guardCounter = -1;
            walkCounter = 0;

            distractionCount = 0;

            getBackCounter = -1;

            deltaX = 0;
            deltaY = 0;
        }

        public override Rectangle GetPosition()
        {
            return new Rectangle(position.X, position.Y, position.Width, position.Height);
        }

        //updates position of guard relative to map
        public override void Update(Action direction, int velocity)
        {
            /*move guard and boundaries with the map*/
            position.X -= velocity;
            patrolBoundaryLeft -= velocity;
            patrolBoundaryRight -= velocity;
        }

        //regular update for stationary map
        public override void Update(GameTime gameTime)
        {
            //perform the appropriate action for the current behaviour
            if (currentBehaviour == Behaviour.patrol)
            {
                //default behaviour, walking back and forth
                Patrol();
                Walk();
            }
            else if (currentBehaviour == Behaviour.guard)
            {
                // chill out for a few and guard some shit
                StandWatch();
                Stand();
            }
            else if (currentBehaviour == Behaviour.goCheckThatShitOut)
            {
                // WTF WAS THAT?!?!?
                GoCheckThatShitOut();
                Walk();
            }
            else if (currentBehaviour == Behaviour.distracted)
            {
                // hmmm.... maybe I'm just going insane... and hearing things... like bolts
                BeDistracted();
                Stand();
            }
            else if (currentBehaviour == Behaviour.gotoPatrol)
            {
                // use the ultimate power of the goto to get back to your starting route!
                GotoPatrol();
            }
            else if (currentBehaviour == Behaviour.teleport)
            {
                Teleport();
            }

            // guards fall straight down
            if (!isFalling)
            {
                position.X += deltaX;
            }
            else
            {
                Fall();
                Stand();
                position.Y -= deltaY;
            }
        }


        //the numbers will have to be tweaked to fit the animation probably
        //right now it's like 100-71 is going in a door, 70 - 31 he disappears, 
        //30-0 he comes back out of the door
        private void Teleport()
        {
            if (getBackCounter < 0)
            {
                deltaX = 0;
                getBackCounter = 100;
                
                //door stuff goes here
                
                
                
            }
            else if (0 < getBackCounter)
            {
          
                //disappear after awhile, because you went through the door
                if (70 == getBackCounter)
                {
                    //now he is off screen and can't collide with anything
                    position.Y = invisibleY;
                    
                }
                //you're now going out the other door!
                if (30 == getBackCounter)
                {
                    deltaX = 0;
                    position.X = patrolBoundaryLeft + velocity;
                    position.Y = patrolY;
                    facingDirection = Direction.right;
                    
                }

                getBackCounter--;
            }
            else if (0 == getBackCounter)
            {

                getBackCounter = -1;
                currentBehaviour = Behaviour.patrol;
            }
        }

        /*
         * guard falls when not in contact with the ground (durrr)
         */
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

        // handles the walking animation
        protected void Walk()
        {
            walkCounter += 1;
            // if the guard was not walking before or finished the walking animation (re-)start the walking animation
            if (currWalkingIndex < walkingIndex || currWalkingIndex == teleportIndex)
            {
                walkCounter = 0;
                currWalkingIndex = walkingIndex;
            }
            else if(walkCounter % 10 == 0)
            {
                // continue the walking animation
                currWalkingIndex += 1;
                walkCounter = 0;
            }

            source.X = spriteX[currWalkingIndex];
            source.Width = spriteWidth[currWalkingIndex];
            position.Width = source.Width * 2;
        }

        // handle the stand animation
        protected void Stand()
        {
            // set the correct sprite for standing watch
            source.X = spriteX[standingIndex];
            source.Width = spriteWidth[standingIndex];

            // scale the sprite to match the size of the previous sprite
            position.Width = source.Width * 2;
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
                // GTF BACK TO YOUR POST SOLDIER!!!
                else if (position.X > patrolBoundaryRight)
                {
                    facingDirection = Direction.left;
                    deltaX = -1 * velocity;
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
                else if (patrolBoundaryLeft > position.X)
                {
                    facingDirection = Direction.right;
                    deltaX = velocity;
                }
                else if (patrolBoundaryLeft == position.X)
                {
                    currentBehaviour = Behaviour.guard;
                }
            }
        }

        protected void GotoPatrol()
        {
            // if the soldier fell - he can't jump he's wearing heavy armor
            if (position.Y != patrolY || patrolBoundaryLeft > position.X || patrolBoundaryRight < position.X)
            {
                currentBehaviour = Behaviour.teleport;
            }
            else
            {
                // patrolling makes the guard turn back if he's out of his patrol range
                currentBehaviour = Behaviour.patrol;
            }
        }

        /**
         * stay at the boundary patrol for the preset amount of time 
         * to watch for guards
         */
        protected void StandWatch()
        {

            deltaX = 0;
            //start the counter if you were previously not counting
            if (guardCounter < 0)
            {
                // the guard will stand watch for guardCounter frames
                guardCounter = guardStartCount;
            }

            //decrease counter while standing watch
            else if (guardCounter > 0)
            {
                guardCounter -= 1;
            }
            //reset the counter when the watch is done and resume patrol in the other direction
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


        //move towards the bolt that you sensed and be distracted for a bit
        protected void GoCheckThatShitOut()
        {
            //i dont know what this math means
            if (Math.Abs(position.X + (position.Width / 2) - distractionX) > 38)
            {
                // I dare say there is a distraction to your posterior good sir!
                if (facingDirection == Direction.right && distractionX < position.X)
                {
                    facingDirection = Direction.left;
                    deltaX = -1 * velocity;
                }
                // the bolt is behind you I say!
                else if (facingDirection == Direction.left && distractionX > position.X)
                {
                    facingDirection = Direction.right;
                    deltaX = velocity;
                }
            }
            else
            {
                currentBehaviour = Behaviour.distracted;
                guardCounter = 70;
            }
        }

        protected void BeDistracted()
        {
            deltaX = 0;
            if (distractionCount < 0)
            {
                // the guard will stand watch for guardCounter frames
                distractionCount = maxDistractedCount;
            }
            else if (distractionCount > 0)
            {
                distractionCount -= 1;
            }
            else
            {
                distractionCount = -1;
                currentBehaviour = Behaviour.gotoPatrol;
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

                tilePos.Y += 2;
                tilePos.Height -= 2;

                Direction direction = determineCollisionType(tilePos);

                // check for left-right collisions
                switch (direction)
                {
                    case Direction.left:
                        if (t.getCollisionBehaviour() == CollisionType.impassable)
                        {
                            // for some weird reason with only 1 pixel of padding this breaks guards fall
                            position.X = t.getPosition().Right;
                            deltaX = 0;
                        }
                        break;

                    case Direction.right:
                        if (t.getCollisionBehaviour() == CollisionType.impassable)
                        {
                            position.X = t.getPosition().Left - position.Width;
                            deltaX = 0;
                        }
                        break;
                }
            }

            // check for foot collisions
            foreach (ITile t in tiles)
            {
                // padding the tile with a pixel on either side so the player cannot climb the walls
                Rectangle tilePos = t.getPosition();

                Direction direction = determineCollisionType(tilePos);

                if (direction == Direction.bottom)
                {
                        position.Y = t.getPosition().Top - position.Height;
                        footCollision = true;
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

        //collision handling in case of sound
        public void HandleHearing(IList<Bolt> bolts)
        {
            Direction hearingDirection;
            Bolt distractingBolt = null;
            
            //the first bolt you actually hear will distract you
            foreach (Bolt bolt in bolts)
            {
               hearingDirection = determineRadialCollision(bolt.GetPosition(), hearingRadius);
               if (hearingDirection != Direction.none)
               {
                   distractingBolt = bolt;
                   debugColor = Color.Blue;
                   break;
               }
            }

            if (distractingBolt != null)
            {
                //set up an x coordinate to head towards to check out the sound
                distractionX = distractingBolt.GetPosition().X;
                //check out the sound
                currentBehaviour = Behaviour.goCheckThatShitOut;
            }
        }

        //if he sees the player, the player should die.
        public void HandleVision(IPlayer player, IList<ITile> surroundingTiles)
        {
            //grab the position of the eyes(relative to guard) and make them relative to the map)
            Vector2  mapEyePos = new Vector2(this.position.X + eyePos.X, this.position.Y + eyePos.Y);

            //distance between the x and y position of the guards eyes and the middle of the player
            float dX = mapEyePos.X - (player.GetPosition().X + (player.GetPosition().Width / 2));
            float dY = mapEyePos.Y - (player.GetPosition().Y + (player.GetPosition().Height / 2));
            float distance = (float)Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));

            float visibility = distance - ((1 - player.HiddenPercent) * LOSRadius);

            debugColor = Color.White;

            // if the player is behind the guard we don't care
            if ((facingDirection == Direction.left && player.GetPosition().X <= mapEyePos.X) || (facingDirection == Direction.right && player.GetPosition().Right >= mapEyePos.X))
            {
                // is the player visible enough/close enough for the guard to be able to see
                if (visibility <= 0 && isVisible(player.GetPosition(), surroundingTiles))
                {
                    debugColor = Color.Red;
                    //player.Kill();
                }
                else
                {
                    debugColor = Color.White;
                }
            }
        }


        //detects whether or not you can actually see something in your field of vision 
        //aka, is there a wall in the way or am i golden?
        protected bool isVisible(Rectangle r, IList<ITile> tiles)
        {
            float m1 = -1 * LOSSlope;
            float m2 = LOSSlope;
            float xLOS = LOSRadius;

            // b = the y location that the guard is looking from
            float b = position.Y + eyePos.Y;
            // x = the x position that the guard is looking from
            int x1 = position.X + (int)eyePos.X;

            int yTop = (int)((m1 * xLOS) + b);    // y = -mx + b
            int yBottom = (int)((m2 * xLOS) + b);    // y = mx + b

            // y axis is inverted in world coords
            if (yTop > yBottom)
            {
                int ySwap = yBottom;
                yBottom = yTop;
                yTop = ySwap;
            }


            IList<ITile> cleanedTiles = new List<ITile>();
            // weed out irrelevant tiles
            foreach (ITile t in tiles)
            {
                // if the tile is completely behind the player relative to the guard's perspective we don't care about it
                if (!((facingDirection == Direction.right && t.getPosition().Left > r.Right) || (facingDirection == Direction.left && t.getPosition().Right < r.Left)))
                {
                    if (t.getCollisionBehaviour() == CollisionType.impassable || (t.getCollisionBehaviour() == CollisionType.platform && r.Bottom <= t.getPosition().Top))
                    {
                        cleanedTiles.Add(t);
                    }
                }
            }

            bool isInLOS = false;

            // points to use for line collision interesection - only want to make these once
            Point p1 = new Point(x1, (int)b);
            Point p2 = new Point();

            while (yTop <= yBottom && !isInLOS)
            {
                // there are two possible values that this could retrieve - for this it will ALWAYS get the x as though the guard were facing right
                p2.X = (int)Math.Sqrt((double)(Math.Abs((LOSRadius * LOSRadius) - (yTop * yTop))));
                p2.Y = yTop;

                if (facingDirection == Direction.left && p2.X > x1)
                {
                    p2.X = x1 - (p2.X - x1);
                }

                // check that the player hits this sweep line otherwise we dont care if tiles are in the way
                isInLOS = isSweepLineCollision(p1, p2, r);

                foreach (ITile t in cleanedTiles)
                {
                    // if the player is already being blocked - we don't care anymore
                    if (isInLOS)
                    {
                        if (isSweepLineCollision(p1, p2, t.getPosition()))
                        {
                            isInLOS = false;
                        }
                    }
                }
                yTop += 1;
            }

            return isInLOS;
        }

        // test to see if r intersects the sweep line from p1 -> p2
        protected bool isSweepLineCollision(Point p1, Point p2, Rectangle r)
        {

            Point q1 = new Point();
            Point q2 = new Point();
            Point collisionPoint;

            if (facingDirection == Direction.right)
            {
                // q1 -> q2 is the left side of the rectangle
                q1.X = r.Left;
                q1.Y = r.Top;
                q2.X = r.Left;
                q2.Y = r.Bottom;
            }
            else
            {
                // q1 -> q2 is the right side of the rectangle
                q1.X = r.Right;
                q1.Y = r.Top;
                q2.X = r.Right;
                q2.Y = r.Bottom;
            }

            collisionPoint = getLineCollisionLocation(p1, p2, q1, q2);
            if (collisionPoint.X != -1 || collisionPoint.Y != -1)
            {
                return true;
            }

            // q1 - > q2 is the top of the rectangle
            q1.X = r.Left;
            q1.Y = r.Top;
            q2.X = r.Right;
            q2.Y = r.Top;

            collisionPoint = getLineCollisionLocation(p1, p2, q1, q2);
            if (collisionPoint.X != -1 || collisionPoint.Y != -1)
            {
                return true;
            }

            // checking the bottom of the rectangle
            q1.X = r.Left;
            q1.Y = r.Bottom;
            q2.X = r.Right;
            q2.Y = r.Bottom;

            collisionPoint = getLineCollisionLocation(p1, p2, q1, q2);
            if (collisionPoint.X != -1 || collisionPoint.Y != -1)
            {
                return true;
            }

            return false;
        }

        // get the bounding rectangle for the guard's line of sight
        public Rectangle GetLOSRectangle()
        {
            int x=0 , y=0, height=0;

            float b = position.Y + eyePos.Y;

            if (facingDirection == Direction.right)
            {
                float mUp = -1 * LOSSlope;
                float mDown = LOSSlope;
                float xLOS = LOSRadius;

                x = position.X + (int)eyePos.X;
                y = (int)((mUp * xLOS) + b);
                height = (int)((mDown * xLOS) + b) - y;
            }
            else
            {
                float mUp = LOSSlope;
                float mDown = -1 * LOSSlope;
                float xLOS = LOSRadius;

                x = position.X + (int)eyePos.X - LOSRadius;
                y = (int)((mUp * xLOS) + b);
                height = (int)(y - ((mDown * xLOS) + b));
            }

            return new Rectangle(x, y, LOSRadius, height);
        }
       
        //the function for use with hearing( and viewing?) collision
        protected Direction determineRadialCollision(Rectangle r, float radius)
        {
            Direction direction = Direction.none;

            Vector2  mapEyePos = new Vector2(this.position.X + eyePos.X, this.position.Y + eyePos.Y);

            // hopefully close enough & easier than having to handle circle/rectangle collisions
            // let recRadius be half the average of the width & height of the rectangle
            float recRadius = 0.25f * ((float)(r.Width + r.Height));

            //distance between the x and y position of the guards eyes and the middle of the player
            float dX = mapEyePos.X - (r.X + (r.Width / 2));
            float dY = mapEyePos.Y - (r.Y + (r.Height / 2));
            float distance = (float)Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));

            debugColor = Color.White;

            // if the distance is less than the two radii, then the rectange is in collision with this Guard's collision radius
            if (distance <= recRadius + radius)
            {
                // now to get the collision direction l\u/r u=up, b=bottom, r=right, l=left
                //                                    l/b\r
                Vector2 destination = new Vector2(dX, dY);
                Vector2 vecDir = destination;// -mapEyePos;
                vecDir.Normalize();
                float angle = VectorToAngle(vecDir);

                while (angle < 0)
                    angle += 360;
                

                // may have to tweak the angle values it depends on how xna stores angles against world coords
                if (angle >= 15 && angle <= 165)
                {
                    direction = Direction.top;
                }
                else if (angle >= 165 && angle <= 195)
                {
                    direction = Direction.right;
                }
                else if (angle >= 195 && angle <= 345)
                {
                    direction = Direction.bottom;
                }
                else
                {
                    direction = Direction.left;
                }
            }

            return direction;
        }

        //converts a vector to an angle
        protected float VectorToAngle(Vector2 v)
        {
            return (float)(Math.Atan2(v.Y, v.X) * (180 / Math.PI));
        }

        // get the normalized vector from the given angle - NOTE angle is probably required to be in radians /cry
        protected Vector2 AngleToVector(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (facingDirection == Direction.right)
            {
                spriteBatch.Draw(sprite, position, source, debugColor, 0, new Vector2(0, 0), SpriteEffects.None, spriteDepth);
            }
            else
            {
                spriteBatch.Draw(sprite, position, source, debugColor, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, spriteDepth);
            }
        }
        


    }
}
