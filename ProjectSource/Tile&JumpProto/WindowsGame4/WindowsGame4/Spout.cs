using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame4
{
    class Spout : IDynamicGameObject
    {     
        private Rectangle position;
        private Level parentLevel;

        private const int spriteWidth = 32;
        private const int spriteHeight = 32;
        protected const int rowsPerScreen = 32;
        protected const int colsPerScreen = 64;

        /* Orientation is a number from 0 to 4, representing down, left, up, right, respectively */
        private int orientation;
        private Rectangle region;
        Texture2D spoutTexture;

        public Spout(Game game, int xPos, int yPos, int screenWidth, int screenHeight, int _orientation, Texture2D _spoutTexture, Level level)
        {
            position = new Rectangle();
            position.Width = screenWidth / colsPerScreen;
            position.Height = screenHeight / rowsPerScreen;
            spoutTexture = _spoutTexture;
            position.X = xPos * position.Width;
            position.Y = yPos * position.Height;
            orientation = _orientation;
            parentLevel = level;
            region = new Rectangle(orientation * spriteWidth, 0, spriteWidth, spriteHeight);
        }
        
        public void Update(GameTime gameTime)
        {

        }

        public void Update(Action action, int myInt)
        {
        }

        public void activate()
        {
            parentLevel.spoutBolt(position.X + position.Width / 2, position.Y + position.Height / 2, orientation);
        }

        public void reposition(int deltaX)
        {
            position.X -= deltaX;
        }

        public void HandleCollision(IList<ITile> tiles)
        {

        }

        public Rectangle GetPosition()
        {
            return position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spoutTexture, position, region, Color.White, 0f, new Vector2(0,0), SpriteEffects.None, 0.80f);
        }
    }
}