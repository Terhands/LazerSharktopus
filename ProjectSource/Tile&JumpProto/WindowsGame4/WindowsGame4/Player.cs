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
        protected Rectangle footCollisionRectangle;

        protected Action facingDirection;

        // hidden goes from 0->1 1 being totallly hidden, 0 being standing yelling & flailing in the middle of a well lit room
        protected float hidden;
        protected bool isJumping;
        protected bool isStopped;
        protected bool isDead;
        protected bool hasReachedGoal;

        protected float spriteDepth = 0.8f;

        // may replace this with a jump meter object later on
        protected JumpMeter jumpMeter;
        protected int playerPadding = 8;

        // the speed that player starts falling
        protected const float startFalling = -0.25f;

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
            this.jumpMeter = new JumpMeter(game, xCenter, yCenter, spriteDepth);

            hidden = 0;
            isJumping = false;
            isStopped = true; // will we need to know this?? Maybe for a funny animation if you take too long...
            isDead = false;
            hasReachedGoal = false;

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

        public float Hidden
        {
            get { return hidden; }
        }

        public void Jump()
        {
            // scale the jump so you can go high fast, but fall a bit slower - less sudden
            if (jumpMeter.JumpPower > 0.00001 || jumpMeter.JumpPower < 0)
            {
                deltaY = (int)(.9 * jumpMeter.JumpPower);
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
            hidden = 0.01f;

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
                this.frameCountCol = 4;
                this.source.X = this.frameStartX + this.frameSkipX * this.frameCountCol;
            }
        }

        public void StopHiding()
        {
            hidden = 0.0f;
            spriteDepth = 0.8f;
            // will also need to swap the texture source rectangle to the standing sprite
            this.frameCountCol = 1;
            this.source.X = this.frameStartX + this.frameSkipX * this.frameCountCol;
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
                if (t.getPosition().Top >= position.Bottom && t.getCollisionBehaviour() != CollisionType.hideable)
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
                tilePos.Y += 1;
                tilePos.Width += 2;

                Direction direction = determineCollisionType(tilePos);

                if (direction != Direction.none && t.getCollisionBehaviour() == CollisionType.goal)
                {
                    hasReachedGoal = true;
                }

                if (direction != Direction.none && t.getCollisionBehaviour() == CollisionType.spike)
                {
                    this.Kill();
                }

                switch (direction)
                {
                    case Direction.bottom:
                        if(t.getCollisionBehaviour() != CollisionType.hideable)
                        {
                            position.Y = t.getPosition().Top - position.Height;

                            if (isJumping)
                            {
                                isJumping = false;
                                landed = true;
                                jumpMeter.reset();
                            }
                        }
                        break;
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
            return new Rectangle(position.X, position.Y, position.Width, position.Height);
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
                    this.frameCountRow = 0;
                    this.animationCount += 1;

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

                // have to decide if we will implement ladder mechanics or rely on jumps
                case Action.up:
                    break;

                case Action.down:
                    break;

                case Action.none:
                    deltaX = 0;
                    break;
            }

            if (isJumping)
            {
                this.frameCountCol = 3;
                Jump();
                
            }

            if (landed)
            {
                landed = false;
                this.frameCountCol = 1;
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
        }

        public void UpdateAnimation()
        {
            if (this.animationCount > this.animationMax)
            {
                this.animationCount = 0;
                if (hidden > 0 || isJumping)
                { }
                else
                    this.frameCountCol += 1;
            }

            if (isJumping)
                this.frameCountCol = 3;
            else if (this.frameCountCol == 3)
            {
                this.frameCountCol = 0;
            }
            else { }
            this.source.X = this.frameStartX + this.frameSkipX * this.frameCountCol;
            this.source.Y = this.frameStartY + this.frameSkipY * this.frameCountRow;
        }
    }
}
