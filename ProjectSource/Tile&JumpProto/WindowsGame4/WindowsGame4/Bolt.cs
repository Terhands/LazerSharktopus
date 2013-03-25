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
        protected ArrayList sounds;

        public Bolt(Game game, Action direction, int xStart, int yStart, Texture2D texture, ArrayList _sounds) : base(game)
        {
            position = new Rectangle(xStart, yStart, 15, 15);
            if (direction == Action.right)
                position.X += 35;
            boltTexture = texture;
            if (direction == Action.left)
                deltaX = deltaX * -1;
            expiryTime = 30;
            spriteDepth = 0.5f;
            rotation = 0;
            sounds = _sounds;
        }
        
        /* Determine if a bolt has hit something */
        public override void HandleCollision(IList<ITile> tiles)
        {
            foreach (ITile t in tiles)
            {
                switch (t.getCollisionBehaviour())
                {
                    //These three types of blocks should never collide with bolts
                    case CollisionType.hideable:
                    case CollisionType.passable:
                    case CollisionType.goal:
                        break;

                    case CollisionType.spike:
                    case CollisionType.impassable:
                    case CollisionType.platform:
                        Rectangle tilePos = t.getPosition();
                        Direction direction = determineCollisionType(tilePos);
                        if (direction == Direction.none)
                        {
                            ((SoundEffect)sounds[0]).Play();
                            this.hasCollided = true;
                        }

                        break;
               }
            }
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
                rotation += 0.2f;
                deltaY += k_looks_gravity;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(boltTexture, position, boltArea, Color.White * 1, rotation, spriteOrigin, SpriteEffects.None, spriteDepth);
        }

    }
}
