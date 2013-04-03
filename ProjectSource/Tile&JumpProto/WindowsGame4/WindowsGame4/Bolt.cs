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

        /* Constructor when the bolt is thrown by the player */
        public Bolt(Game game, Action direction, int xStart, int yStart, Texture2D texture, ArrayList _sounds) : base(game)
        {
            position = new Rectangle(xStart, yStart, 15, 15);
            
            /* Set the starting information for the bolt based on where it's going */
            if (direction == Action.right)
                position.X += 35;
            if (direction == Action.left)
                deltaX = deltaX * -1;

            /* Set basic always the same values for the bolt */
            boltTexture = texture;
            expiryTime = 30;
            spriteDepth = 0.5f;
            rotation = 0;
            sounds = _sounds;
        }

        /* Constructor when the bolt is generated from a spout */
        /* Orientations: down=0, left=1, up=2, right=3 */
        public Bolt(Game game, int orientation, int xStart, int yStart, Texture2D texture, ArrayList _sounds)
            : base(game)
        {
            position = new Rectangle(xStart, yStart, 15, 15);

            /* Set the starting information for the bolt based on where it's going */
            if (orientation == 0 || orientation == 2)
            {
                deltaX = 0; // Throw the bolt straight up
                if (orientation == 0)
                {
                    deltaY = deltaY * -1; // Throw it down
                }
                else if (orientation == 2)
                {
                    deltaY = deltaY * 2; //More force!
                }
            }
            if (orientation == 1 || orientation == 3)
            {
                deltaY = 0; // Throw it horiztonally
                if (orientation == 1)
                    deltaX = deltaX * -1; // Throw it left instead of right
            }

            /* Set basic always the same values for the bolt */
            boltTexture = texture;
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
                    case CollisionType.magnet:
                    case CollisionType.invisible:
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
