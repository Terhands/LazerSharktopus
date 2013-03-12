using System;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace WindowsGame4
{
    class Map : Microsoft.Xna.Framework.GameComponent, IMap
    {
        protected ITile[,] tiles;
        protected Texture2D background;
        protected Rectangle bgPosition;

        protected int pixelOffset;
        protected int maxPixelOffset;
        protected int minPixelOffset;

        protected int screenWidth;
        protected int screenHeight;

        protected const int rowsPerScreen = 32;
        protected const int colsPerScreen = 64;

        // our test map has 100 columns
        //protected int mapCols = 100;

        public Map(Game game, int[,] mapLayout, ArrayList tileTextures) : base(game)
        {
            Initialize(mapLayout, tileTextures);
        }

        /* load the level map from file - for now just load a dummy level with random tile types */
        protected void Initialize(int[,] mapLayout, ArrayList tileTextures)
        {
            tiles = new Tile[mapLayout.GetLength(0), mapLayout.GetLength(1)];

            screenWidth = Game.GraphicsDevice.Viewport.Width;
            screenHeight = Game.GraphicsDevice.Viewport.Height;

            // hardcoded for now until we set up the file read
            background = (Texture2D)tileTextures[2];
            bgPosition.X = 0;
            bgPosition.Y = 0;
            bgPosition.Width = screenWidth;
            bgPosition.Height = screenHeight;

            pixelOffset = 0;
            maxPixelOffset = (mapLayout.GetLength(1) * (screenWidth / colsPerScreen)) - screenWidth;
            minPixelOffset = 0;

            for (int i = 0; i < mapLayout.GetLength(0); i++)
            {
                for (int j = 0; j < mapLayout.GetLength(1); j++)
                {
                    tiles[i, j] = new Tile(Game, (Texture2D)tileTextures[1], (CollisionType)mapLayout[i,j], j, i, screenWidth, screenHeight, rowsPerScreen, colsPerScreen);
                }
            }

            /*
            Random rand = new Random();

            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    CollisionType colType = CollisionType.platform;

                    if ((29 == i && 51 < j))
                    {
                        colType = CollisionType.hideable;
                    }
                    else if (31 == i || (30 == i && j > 50) || (i == 18 && j > 30 && j < 40) || (i == 23 && j > 42 && j < 50))
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
                    tiles[i, j] = new Tile(Game, (Texture2D)tileTextures[1], colType, j, i, screenWidth, screenHeight);
                }
            }
            */ 
        }

        /* reset the level offset */
        public void Reset()
        {
            pixelOffset = 0;
        }

        /* determine if the screen has hit the left edge of the map */
        public bool atLeftEdge()
        {
            return (pixelOffset == minPixelOffset);
        }

        /* determine if the screen has hit the right edge of the map */
        public bool atRightEdge()
        {
            return (pixelOffset == maxPixelOffset);
        }

        /* get the tiles that are either intersecting position on the screen */
        public IList<ITile> GetNearbyTiles(Rectangle position)
        {
            // the x and y indices in tiles of the tile being intersected by the top-left of position
            int xIndex = (position.X + pixelOffset) / (screenWidth / colsPerScreen);
            int yIndex = position.Y / (screenHeight / rowsPerScreen);

            // need to grab the surrounding tiles as well (1 extra in each direction) - in case object is intersecting multiple tiles
            int xMin = xIndex - 1;
            // the leftMost tile cannot be before the left-most tile of the level
            if (xMin < 0) { xMin = 0; }

            // get the right-most tile position that we will care about for collisions
            int xMax = xMin;
            while ((xMax < 100 - 1) && tiles[0, xMax].getPosition().Right < position.Right)
            {
                xMax += 1;
            }


            int yMin = yIndex - 1;
            // the topMost tile cannot be before the top-most tile of the level
            if (yMin < 0) { yMin = 0; }

            // get the bottom-most tile position that we will case about for collisions
            int yMax = yMin;
            while (yMax < (rowsPerScreen - 1) && tiles[yMax, 0].getPosition().Bottom <= position.Bottom)
            {
                yMax += 1;
            }

            IList<ITile> nearbyTiles = new List<ITile>();

            // add all of the relevant tiles for potential collision handling
            for (int i = xMin; i <= xMax && i < tiles.GetLength(1); i++)
            {
                for (int j = yMin; j <= yMax && j < rowsPerScreen; j++)
                {
                    //only pass back platform tiles & impassable tiles (don't care if no collision could result)
                    if (tiles[j, i].getCollisionBehaviour() != CollisionType.passable)
                    {
                        nearbyTiles.Add(tiles[j, i]);
                    }
                }
            }

            return nearbyTiles;
        }

        /* update the level position */
        public void Update(Action direction, int velocity)
        {
            switch (direction)
            {
                case Action.left:
                    if (pixelOffset + velocity > minPixelOffset)
                    {
                        pixelOffset += velocity;
                    }
                    else
                    {
                        velocity = minPixelOffset - pixelOffset;
                        pixelOffset = minPixelOffset;
                    }
                    break;
                case Action.right:
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
                case Action.up:
                    //have to decide if we will ever shift up/down
                    break;
                case Action.down:
                    //have to decide if we will ever shift up/down
                    break;
            }

            /* update the tile set for this level */
            foreach (ITile t in tiles)
            {
                t.Update(direction, velocity);
            }
        }

        /* draw the level map */
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, bgPosition, Color.White);
            foreach (ITile t in tiles)
            {
                t.Draw(spriteBatch);
            }
        }
    }
}
