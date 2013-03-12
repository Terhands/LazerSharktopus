using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame4
{
    class Bolt : ADynamicGameObject, IGameObject
    {
        Texture2D boltTexture;
        Rectangle position;

        float deltaX = 3;
        float deltaY = -3;
        float k_looks_gravity = 0.15f;
        public bool hasCollided = false;
        public int expiryTime;
        float spriteDepth;

        private Rectangle boltArea = new Rectangle(0, 0, 64, 64);
        private Vector2 spriteOrigin = new Vector2(32, 32);
        private float rotation;

        public Bolt(Game game, Action direction, int xStart, int yStart, Texture2D texture) : base(game)
        {
            position = new Rectangle(xStart, yStart, 25, 25);
            if (direction == Action.right)
                position.X += 35;
            boltTexture = texture;
            if (direction == Action.left)
                deltaX = deltaX * -1;
            expiryTime = 30;
            spriteDepth = 0.5f;
            rotation = 0;
        }
        
        /* Determine if a bolt has hit something */
        public override void HandleCollision(IList<ITile> tiles)
        {
            if (position.Y > 440) hasCollided = true;
            // handle foot to floor collisions after intersection collisions have been resolved
            IList<ITile> tilesBelowBolt = new List<ITile>();
            IList<ITile> tilesAboveBolt = new List<ITile>();

            foreach (ITile t in tiles)
            {
                if (t.getPosition().Top < position.Bottom)
                {
                    tilesBelowBolt.Add(t);
                }
                else if (t.getPosition().Top >= position.Bottom && t.getCollisionBehaviour() != CollisionType.hideable)
                {
                    tilesAboveBolt.Add(t);
                }
                HandleIntersectionCollisions(tiles);
            }

            

           // HandleFootCollisions(tilesAboveBolt);
        }

        protected void HandleIntersectionCollisions(IList<ITile> tiles)
        {
            // check that any intersections are only on passable tiles
            foreach (ITile t in tiles)
            {
                // padding the tile with a pixel on either side so the player cannot climb the walls
                Rectangle tilePos = t.getPosition();

                Direction direction = determineCollisionType(tilePos);
                if (direction == Direction.none)
                {
                    this.hasCollided = true;
                }
            }
        }

        protected void HandleFootCollisions(IList<ITile> tiles)
        {
            bool footCollision = false;

            foreach (ITile t in tiles)
            {
                Direction direction = determineCollisionType(t.getPosition());

                if (Direction.bottom == direction)
                {
                    footCollision = true;
                    /*
                    if (isJumping)
                    {
                        isJumping = false;
                        jumpMeter.reset();
                    }*/
                }
            }
            /*
            // if the player is not jumping and has no tiles under their feet, they start falling
            if (!isJumping && !footCollision)
            {
                isJumping = true;
                jumpMeter.JumpPower = startFalling;
            }*/
        }
        
        public override Rectangle GetPosition()
        {
            return position;
        }

        public void reposition(int deltaX)
        {
            position.X -= deltaX;
        }

        public override void Update(Action action, int velocity)
        {
            if (hasCollided)
            {
                expiryTime--;
            }
            else
            {
                position.X += (int)deltaX;
                position.Y += (int)deltaY;

                deltaY += k_looks_gravity;
            }

            if (!hasCollided)
                rotation += 0.2f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(boltTexture, position, boltArea, Color.White * 1, rotation, spriteOrigin, SpriteEffects.None, spriteDepth);
        }

    }
}
