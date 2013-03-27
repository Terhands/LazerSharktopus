using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame4
{
    class Tile : DrawableGameComponent, ITile
    {
        protected int spriteWidth = 16;
        protected int spriteOffset = 1;
        protected float spriteDepth;
        protected CollisionType collisionBehaviour;
        protected Texture2D tileTexture;
        protected Rectangle position;
        protected Rectangle source;
        protected Color color;

        /* set up the tile -> figure out it's position in the level, it's size relative to the screen & load the sprite+set the collision behaviour */
        public Tile(Game game, Texture2D texture, CollisionType type, int xIndex, int yIndex, int screenWidth, int screenHeight, int rowsPerScreen, int colsPerScreen) : base(game)
        {
            LoadContent(texture, type);
            position = new Rectangle();

            position.Width = screenWidth/colsPerScreen;
            position.Height = screenHeight/rowsPerScreen;

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
                    position.X -= velocity;
                    break;
            }
        }

        /* load the basic Tile properties (the texture/collision properties) */
        public void LoadContent(Texture2D texture, CollisionType cType)
        {
            tileTexture = texture;
            collisionBehaviour = cType;
            color = Color.White;

            Random rand = new Random();
            int textureIndex = rand.Next(1, 4);
            textureIndex = spriteOffset * textureIndex + spriteWidth * (textureIndex - 1);

            switch (collisionBehaviour)
            {
                case CollisionType.impassable:
                    source = new Rectangle(textureIndex, spriteOffset, spriteWidth, spriteWidth);
                    spriteDepth = 0.2f;
                    break;
                case CollisionType.passable:
                    color = Color.Transparent;
                    source = new Rectangle(0, 0, spriteWidth, spriteWidth);
                    spriteDepth = 0;
                    break;
                case CollisionType.platform:
                    source = new Rectangle(textureIndex, spriteOffset * 2 + spriteWidth, spriteWidth, spriteWidth);
                    spriteDepth = 0.2f;
                    break;
                case CollisionType.hideable:
                    source = new Rectangle(textureIndex, spriteOffset * 3 + spriteWidth * 2, spriteWidth, spriteWidth);
                    spriteDepth = 0.75f;
                    break;
                case CollisionType.goal:
                    color = Color.White;
                    source = new Rectangle(spriteOffset, spriteOffset * 4 + spriteWidth * 3, spriteWidth, spriteWidth);
                    spriteDepth = 0.2f;
                    break;
                case CollisionType.spike:
                    source = new Rectangle(spriteOffset, spriteOffset * 5 + spriteWidth * 4, spriteWidth, spriteWidth);
                    spriteDepth = 0.2f;
                    break;
                case CollisionType.invisible:
                    color = Color.Transparent;
                    source = new Rectangle(0, 0, spriteWidth, spriteWidth);
                    spriteDepth = 0;
                    break;
            }
        }

        /* draw this tile to screen with the current spriteBatch */
        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(tileTexture, position, color);
            spriteBatch.Draw(tileTexture, position, source, color, 0, new Vector2(0, 0), SpriteEffects.None, spriteDepth);
        }
    }
}