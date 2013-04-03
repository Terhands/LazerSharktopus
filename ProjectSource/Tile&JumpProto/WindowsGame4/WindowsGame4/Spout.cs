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

        private const int spriteWidth = 32;
        private const int spriteHeight = 32;
        protected const int rowsPerScreen = 32;
        protected const int colsPerScreen = 64;

        Texture2D spoutTexture;

        public Spout(Game game, int xPos, int yPos, int screenWidth, int screenHeight, Texture2D _spoutTexture)
        {
            position = new Rectangle();
            position.Width = screenWidth / colsPerScreen;
            position.Height = screenHeight / rowsPerScreen;
            spoutTexture = _spoutTexture;
            position.X = xPos * position.Width;
            position.Y = yPos * position.Height;

        }
        
        public void Update(GameTime gameTime)
        {

        }

        public void Update(Action action, int myInt)
        {
        }

        public void activate()
        {
            /* Spawn a new bolt */
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
            spriteBatch.Draw(spoutTexture, position, null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.15f);
        }
    }
}