﻿using System;
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
        protected Action facingDirection;

        protected bool isHidden;
        protected bool isJumping;
        protected bool isStopped;
        protected bool isDead;
        protected bool hasReachedGoal;

        protected const float spriteDepth = 0.5f;

        // may replace this with a jump meter object later on
        protected JumpMeter jumpMeter;
        protected int playerPadding = 8;

        // the speed that player starts falling
        protected const float startFalling = -0.25f;

        public Player(Game game, Texture2D texture, ArrayList _sounds, int xStart, int yStart)
            : base(game)
        {
            facingDirection = Action.right;
            source = new Rectangle(251, 142, 746 - 251, 805 - 142);
            position = new Rectangle(xStart, yStart, 36, 52);
            sprite = texture;

            int xCenter = xStart + (position.Width / 2);
            int yCenter = yStart - playerPadding;
            this.jumpMeter = new JumpMeter(game, xCenter, yCenter, spriteDepth);

            isHidden = false;
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

        public void Jump()
        {
            // scale the jump so you can go high fast, but fall a bit slower - less sudden
            if (jumpMeter.JumpPower > 0.00001 || jumpMeter.JumpPower < 0)
            {
                deltaY = (int)(5 / 3 * jumpMeter.JumpPower);
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

            // when jumping the player has less control of their direction until they land
            position.Y -= deltaY;
            position.X += deltaX;
            isJumping = true;
        }

        public void ChargeJumpPower()
        {
            if (!isJumping)
            {
                jumpMeter.chargeJumpPower();
            }
        }

        public void ThrowBolt()
        {

        }

        public void Hide()
        {

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
                tilePos.Width += 2;

                Direction direction = determineCollisionType(tilePos);

                if (direction != Direction.none && t.getCollisionBehaviour() == CollisionType.goal)
                {
                    hasReachedGoal = true;
                }

                if (direction != Direction.none && t.getCollisionBehaviour() == CollisionType.spike)
                {
                    isDead = true;
                    ((SoundEffect)sounds[0]).Play();
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
        }

        public override void Update(Action direction, int velocity)
        {
            switch (direction)
            {
                case Action.right:
                    facingDirection = direction;

                    if (!isJumping)
                    {
                        deltaX = velocity;
                        position.X += deltaX;
                    }

                    break;
                case Action.left:
                    facingDirection = direction;

                    if (!isJumping)
                    {
                        deltaX = velocity;
                        position.X += deltaX;
                    }
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
                case Action.none:
                    if (isJumping)
                    {
                        Jump();
                    }
                    else
                    {
                        // once the player has landed the user regains control of movement
                        deltaX = 0;
                    }
                    break;
            }

            jumpMeter.setMeterPosition(position.X + (position.Width / 2), position.Y - playerPadding);
            jumpMeter.Update(Action.none, 0);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isDead)
            {
                spriteBatch.Draw(sprite, position, source, Color.Crimson, 0, new Vector2(0, 0), SpriteEffects.None, spriteDepth);
            }
            else
            {
                spriteBatch.Draw(sprite, position, source, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, spriteDepth);
                jumpMeter.Draw(spriteBatch);
            }
        }
    }
}
