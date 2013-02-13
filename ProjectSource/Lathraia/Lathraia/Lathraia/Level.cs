using System;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame4
{
    class Level : Microsoft.Xna.Framework.GameComponent, ILevel
    {
        protected ITile[,] background;
        protected int pixelOffset;
        protected int maxPixelOffset;
        protected int minPixelOffset;

        public Level(Game game, string filename, ArrayList tileTextures) : base(game)
        {
            Initialize(filename, tileTextures);
        }

        /* load the level map from file - for now just load a dummy level with random tile types */
        protected void Initialize(string filename, ArrayList tileTextures)
        {
            background = new Tile[32, 100];

            int screenWidth = Game.GraphicsDevice.Viewport.Width;
            int screenHeight = Game.GraphicsDevice.Viewport.Height;

            pixelOffset = 0;
            maxPixelOffset = (100 * (screenWidth / 64)) - screenWidth;
            minPixelOffset = 0;

            Random rand = new Random();

            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    CollisionType colType = CollisionType.platform;
                    /*
                    switch (rand.Next(3))
                    {
                        case 0:
                            colType = CollisionType.passable;
                            break;
                        case 1:
                            colType = CollisionType.impassable;
                            break;
                        case 2:
                            colType = CollisionType.platform;
                            break;
                    }
                    */

                    if (31 == i || (29 == i && 51 < j) || (30 == i && j > 50))
                    {
                        colType = CollisionType.platform;
                    }
                    else if (99 == j || 0 == j)
                    {
                        colType = CollisionType.impassable;
                    }
                    else
                    {
                        colType = CollisionType.passable;
                    }
                    background[i, j] = new Tile((Texture2D)tileTextures[0], colType, j, i, screenWidth, screenHeight);
                }
            }
        }

        /* reset the level offset */
        public void Reset()
        {
            pixelOffset = 0;
        }

        /* update the level position */
        public void Update(Direction direction, int velocity)
        {
            switch (direction)
            {
                case Direction.right:
                    if (pixelOffset - velocity > minPixelOffset)
                    {
                        pixelOffset -= velocity;
                    }
                    else
                    {
                        velocity = pixelOffset - minPixelOffset;
                        pixelOffset = minPixelOffset;
                    }
                    break;
                case Direction.left:
                    if (pixelOffset + velocity < maxPixelOffset)
                    {
                        pixelOffset += velocity;
                    }
                    else
                    {
                        velocity = maxPixelOffset - pixelOffset;
                        pixelOffset = maxPixelOffset;
                    }
                    break;
                case Direction.up:
                    //have to decide if we will ever shit up/down
                    break;
                case Direction.down:
                    //have to decide if we will ever shift up/down
                    break;
            }

            /* update the tile set for this level */
            foreach (ITile t in background)
            {
                t.Update(direction, velocity);
            }
        }

        /* draw the level map */
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (ITile t in background)
            {
                t.Draw(spriteBatch);
            }
        }
    }
}
