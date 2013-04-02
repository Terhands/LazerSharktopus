using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame4
{
    class Soldier : Wizard, IGuard
    {

        private int startY;

        protected int[] spriteX = { 1, 15, 31, 47, 63, 79, 95, 111, 131, 153, 173, 196, 214, 238 };
        protected int[] spriteWidth = { 13, 14, 15, 15, 15, 15, 15, 19, 21, 19, 22, 17, 23, 12 };

        protected bool isStuck;

        public Soldier(Game game, Texture2D texture, Texture2D LOStexture, int xStart, int yStart, Direction FacingDirectionStart, int patrolLength)
            : base(game, texture, LOStexture, xStart, yStart, FacingDirectionStart, patrolLength)
        {
            startY = yStart;
            isStuck = false;

            base.spriteX = this.spriteX;
            base.spriteWidth = this.spriteWidth;
        }

        public override void Update(GameTime gameTime)
        {
            if (currentBehaviour == Behaviour.patrol)
            {
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
            else if (currentBehaviour == Behaviour.wander)
            {
                // turn around whenever you hit a wall & just walk on
                Wander();
                Walk();
            }

            // when a soldier is stuck to a magnet they can no longer move
            if (!isStuck)
            {
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
        }

        protected void GotoPatrol()
        {
            // if the soldier fell - he can't jump he's wearing heavy armor
            if (position.Y != startY)
            {
                currentBehaviour = Behaviour.wander;
            }
            else
            {
                // patrolling makes the guard turn back if he's out of his patrol range
                currentBehaviour = Behaviour.patrol;
            }
        }

        protected void Wander()
        {
            if (!isFalling && deltaX == 0)
            {
                if (facingDirection == Direction.right)
                {
                    facingDirection = Direction.left;
                    deltaX = -1 * velocity;
                }
                else
                {
                    facingDirection = Direction.right;
                    deltaX = velocity;
                }
            }
        }

        // soldiers differ from guards - magnets hold them in place
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

                    // when a guard's feet hit a magnet he gets stuck to it & can no longer move freely
                    if (CollisionType.magnet == t.getCollisionBehaviour())
                    {
                        isStuck = true;
                        position.X = t.getPosition().X + t.getPosition().Width / 2 - position.Width / 2;
                    }
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

    }
}
