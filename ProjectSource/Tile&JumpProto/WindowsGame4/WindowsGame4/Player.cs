using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using Microsoft.Xna.Framework.Audio;

namespace WindowsGame4
{
    class Player : ADynamicGameObject, IPlayer
    {
        protected Texture2D sprite;
        protected ArrayList sounds;
        protected Rectangle source;

        protected Rectangle leftRightCollisionRectangle;

        protected Action facingDirection;

        // hidden goes from 0->1 1 being totallly hidden, 0 being standing yelling & flailing in the middle of a well lit room
        protected float hidden;
        protected bool isJumping;
        protected bool isStopped;
        protected bool isDead;
        protected bool hasReachedGoal;
        protected bool displayHealthBar;

        protected float spriteDepth = 0.8f;

        // may replace this with a jump meter object later on
        protected JumpMeter jumpMeter;
        protected int playerPadding = 8;

        // the speed that player starts falling
        protected const float startFalling = -0.25f;
        protected const int maxPainlessFall = 30;
        protected int fallDistance;


        // robro's change in height from crouching to standing up straight
        protected const int crouchDiff = 30;

        int frameCountCol = 1; // Which frame we are.  Values = {0, 1, 2, 3, 4} this 4 = crouch only in row 0 
        int frameCountRow = 0; //Value = {0,1} 1 = left direction , 0 = right directions
        int frameSkipX = 75; // How much to move the frame in X when we increment a frame--X distance between top left corners.
        int frameSkipY = 127; // how much to move the frame in y when we change directions 
        int frameStartX = 1; // X of top left corner of frame 0. 
        int frameStartY = 1; // Y of top left corner of frame 0.
        int frameWidth = 75; // X of right minus X of left. 
        int frameHeight = 126; // Y of bottom minus Y of top.

        // Keep a counter, to count the number of ticks since the last change of animation frame.
        int animationCount; // How many ticks since the last frame change.
        int animationMax = 8; // How many ticks to change frame after. 
        bool landed = false; //So the update doesnt keep reseting robro to standard land position this bool is used

        protected HealthMeter healthMeter;

        public Player(Game game, Texture2D texture, ArrayList _sounds, int xStart, int yStart)
            : base(game)
        {
            facingDirection = Action.right;
            source = new Rectangle(this.frameStartX + this.frameSkipX * this.frameCountCol, this.frameStartY + this.frameSkipY * this.frameCountRow, this.frameWidth, this.frameHeight);
            position = new Rectangle(xStart, yStart, 36, 52);
            leftRightCollisionRectangle = new Rectangle(position.X, position.Y, position.Width, position.Height - 2);

            sprite = texture;

            int xCenter = xStart + (position.Width / 2);
            int yCenter = yStart - playerPadding;
            jumpMeter = new JumpMeter(game, xCenter, yCenter, spriteDepth);

            healthMeter = new HealthMeter(game, 200, 10, spriteDepth);

            hidden = 0.0f;
            fallDistance = 0;
            isJumping = false;
            isStopped = true; // will we need to know this?? Maybe for a funny animation if you take too long...
            isDead = false;
            hasReachedGoal = false;
            displayHealthBar = true;

            sounds = _sounds;

            deltaX = 0;
            deltaY = 0;
        }

        public bool DoneLevel
        {
            get { return hasReachedGoal; }
        }

        public bool IsDead
        {
            get { return isDead; }
            set { isDead = value; }
        }

        /* Updates Robro's health when a bolt is thrown */
        public void throwBolt()
        {
            healthMeter.lowerHealthMeter();
        }

        public void Jump()
        {
            // scale the jump so you can go high fast, but fall a bit slower - less sudden
            deltaY = (int)(.9 * jumpMeter.JumpPower);
            if (jumpMeter.JumpPower > 0)
            {
                jumpMeter.drainJumpPower(0.25f);
                fallDistance = 0;
            }
            else
            {
                jumpMeter.drainJumpPower(0.2f);
                fallDistance -= deltaY;
            }

            isJumping = true;
        }

        public void ChargeJumpPower()
        {
            if (!isJumping)
            {
                jumpMeter.chargeJumpPower();
            }
        }

        public void Hide(IList<ITile> tiles)
        {
            // attempting to hide in the middle of a well lit room...
            hidden = 0.5f;

            foreach (ITile t in tiles)
            {
                if (t.getCollisionBehaviour() == CollisionType.hideable)
                {
                    if (t.isInCollision(this))
                    {
                        hidden = 1.0f;
                        spriteDepth = 0.25f;
                        // will also need to swap the texture source rectangle to the crouched sprite
                        
                    }
                }
            }

            frameCountCol = 4;
            source.X = frameStartX + frameSkipX * frameCountCol;
        }

        public void StopHiding()
        {
            hidden = 0.0f;
            spriteDepth = 0.8f;
            // will also need to swap the texture source rectangle to the standing sprite
            frameCountCol = 1;
            source.X = frameStartX + frameSkipX * frameCountCol;
        }

        /* make sure player isn't falling through platforms/walking through walls */
        public override void HandleCollision(IList<ITile> tiles)
        {
            // handle foot to floor collisions after intersection collisions have been resolved
            IList<ITile> tilesBelowPlayer = new List<ITile>();
            IList<ITile> tilesAtPlayerLevel = new List<ITile>();

            foreach (ITile t in tiles)
            {
                if (t.getPosition().Top < position.Bottom)
                {
                    tilesAtPlayerLevel.Add(t);
                }
            }

            HandleIntersectionCollisions(tilesAtPlayerLevel);

            foreach (ITile t in tiles)
            {
                if (t.getPosition().Top >= position.Top && t.getCollisionBehaviour() != CollisionType.hideable)
                {
                    tilesBelowPlayer.Add(t);
                }
            }

            HandleFootCollisions(tilesBelowPlayer);
        }

        protected void HandleIntersectionCollisions(IList<ITile> tiles)
        {
            // check that any intersections are only on passable tiles
            foreach (ITile t in tiles)
            {
                // padding the tile with a pixel on either side so the player cannot climb the walls
                Rectangle tilePos = t.getPosition();
                
                tilePos.X -= 1;
                tilePos.Y += 2;
                tilePos.Height -= 2;
                tilePos.Width += 1;
                
                Direction direction = determineCollisionType(tilePos);

                if (direction != Direction.none && t.getCollisionBehaviour() == CollisionType.goal)
                {
                    hasReachedGoal = true;
                }

                if (direction != Direction.none && t.getCollisionBehaviour() == CollisionType.spike)
                {
                    Kill();
                }

                switch (direction)
                {
                    case Direction.top:
                        if (t.getCollisionBehaviour() == CollisionType.impassable)
                        {
                            position.Y = t.getPosition().Bottom;
                            if (isJumping && startFalling < jumpMeter.JumpPower)
                            {
                                jumpMeter.JumpPower = startFalling;
                            }
                        }
                        break;
                    case Direction.left:
                        if (t.getCollisionBehaviour() == CollisionType.impassable)
                        {
                            // for some wierd reason with only 1 pixel of padding this breaks player's fall
                            position.X = t.getPosition().Right + 3;
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
        }

        protected void HandleFootCollisions(IList<ITile> tiles)
        {
            bool footCollision = false;

            foreach (ITile t in tiles)
            {
                Direction direction = determineCollisionType(t.getPosition());

                if (Direction.bottom == direction && t.getCollisionBehaviour() != CollisionType.spike)
                {
                    footCollision = true;

                    position.Y = t.getPosition().Top - position.Height;

                    if (isJumping)
                    {
                        isJumping = false;
                        landed = true;
                        jumpMeter.reset();
                    }
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
            if (hidden > 0.0f)
            {
                // robro is shorter when he is crouching
                return new Rectangle(position.X, position.Y + crouchDiff, position.Width, position.Height - crouchDiff);
            }
            else
            {
                return new Rectangle(position.X, position.Y, position.Width, position.Height);
            }
        }

        public Action GetFacingDirection()
        {
            return facingDirection;
        }

        public void Kill()
        {
            isDead = true;
            ((SoundEffect)sounds[0]).Play();
        }

        public int DeltaX
        {
            get { return deltaX; }
        }

        public float HiddenPercent
        {
            get { return hidden; }
            set { hidden = value; }
        }

        public void reposition()
        {
            position.X -= deltaX;
        }

        public override void Update(Action direction, int velocity)
        {
            switch (direction)
            {
                case Action.right:
                    facingDirection = direction;
                    if (isJumping && deltaX <= 0)
                    {
                        deltaX = velocity / 2;
                    }
                    else if (!isJumping)
                    {
                        if (hidden <= 0)
                        {
                            deltaX = velocity;
                        }
                        else
                        {
                            deltaX = velocity / 2;
                        }
                    }
                    frameCountRow = 0;
                    animationCount += 1;

                    break;
                case Action.left:
                    facingDirection = direction;

                    if (isJumping && deltaX >= 0)
                    {
                        deltaX = velocity / 2;
                    }
                    else if (!isJumping)
                    {
                        if (hidden <= 0)
                        {
                            deltaX = velocity;
                        }
                        else
                        {
                            deltaX = velocity / 2;
                        }
                    }
                    this.frameCountRow = 1;
                    this.animationCount += 1;
                    break;

                case Action.none:
                    deltaX = 0;
                    break;
            }

            if (isJumping)
            {
                frameCountCol = 3;
                Jump();
            }

            if (landed)
            {
                landed = false;
                frameCountCol = 1;

                if (fallDistance > maxPainlessFall)
                {
                    healthMeter.lowerHealthMeter();
                    fallDistance = 0;
                }
            }

            healthMeter.Update(Action.none, 0);
            if (healthMeter.Health <= 0) // If health is less than or equal to zero, Robro dies
            {
                Kill();
            }

            position.X += deltaX;
            position.Y -= deltaY;

            jumpMeter.setMeterPosition(position.X + (position.Width / 2), position.Y - playerPadding);
            jumpMeter.Update(Action.none, 0);

            this.UpdateAnimation();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isDead)
            {
                spriteBatch.Draw(sprite, position, source, Color.Crimson, 0, new Vector2(0, 0), SpriteEffects.None, spriteDepth);
            }
            else
            {
                spriteBatch.Draw(sprite, position, source, Color.White * 1f, 0, new Vector2(0, 0), SpriteEffects.None, spriteDepth);
                jumpMeter.Draw(spriteBatch);
            }

            if (displayHealthBar)
            {
                healthMeter.Draw(spriteBatch);
            }
        }

        public void ToggleHealthBar()
        {
            displayHealthBar = !displayHealthBar;
        }

        public void UpdateAnimation()
        {
            if (animationCount > animationMax)
            {
                animationCount = 0;
                if (hidden > 0 || isJumping)
                { }
                else
                {
                    frameCountCol += 1;
                }
            }

            if (isJumping)
            {
                frameCountCol = 3;
            }
            else if (frameCountCol == 3)
            {
                frameCountCol = 0;
            }
            else { }
            source.X = frameStartX + frameSkipX * frameCountCol;
            source.Y = frameStartY + frameSkipY * frameCountRow;
        }
    }
}
