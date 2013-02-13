using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame4
{
    class Tile : ITile
    {
        protected CollisionType collisionBehaviour;
        protected Texture2D tileTexture;
        protected Rectangle position;
        protected Color color;

        /* set up the tile -> figure out it's position in the level, it's size relative to the screen & load the sprite+set the collision behaviour */
        public Tile(Texture2D texture, CollisionType type, int xIndex, int yIndex, int screenWidth, int screenHeight)
        {
            LoadContent(texture, type);
            position = new Rectangle();

            position.Width = screenWidth/64;
            position.Height = screenHeight/32;

            position.X = xIndex * position.Width;
            position.Y = yIndex * position.Height;
        }

        /* determine whether or not component is in collision with this tile */
        public bool isInCollision(IDynamicGameObject obj)
        {
            bool result = false;
            if (position.Intersects(obj.GetPosition()))
            {
                result = true;
            }

            return result;
        }

        /* get the screen position of this tile */
        public Rectangle getPosition()
        {
            return new Rectangle(position.X, position.Y, position.Width, position.Height);
        }

        /* get the collision behaviour property for this tile */
        public CollisionType getCollisionBehaviour()
        {
            return collisionBehaviour;
        }

        /* shift the draw position of this tile to the direction */
        public void Update(Action direction, int velocity)
        {
            switch (direction)
            {
                /*
                case Direction.up:
                    position.Y -= velocity;
                    break;
                case Direction.down:
                    position.Y += velocity;
                    break
                */
                case Action.right:
                    position.X -= velocity;
                    break;
                case Action.left:
                    position.X += velocity;
                    break;
            }
        }

        /* load the basic Tile properties (the texture/collision properties) */
        public void LoadContent(Texture2D texture, CollisionType cType)
        {
            tileTexture = texture;
            collisionBehaviour = cType;

            switch (collisionBehaviour)
            {
                case CollisionType.impassable:
                    color = Color.Silver;
                    break;
                case CollisionType.passable:
                    color = Color.Transparent;
                    break;
                case CollisionType.platform:
                    color = Color.SteelBlue;
                    break;
            }
        }

        /* draw this tile to screen with the current spriteBatch */
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tileTexture, position, color);
        }
    }
}
