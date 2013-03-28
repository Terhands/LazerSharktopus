using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame4
{
    class Soldier : Guard, IGuard
    {

        private int startY;

        public Soldier(Game game, Texture2D texture, int xStart, int yStart, Direction FacingDirectionStart, int patrolLength)
            : base(game, texture, xStart, yStart, FacingDirectionStart, patrolLength)
        {
            startY = yStart;
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
    }
}
