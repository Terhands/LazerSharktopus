using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame4
{
    class LevelIntroScreen : DrawableGameComponent
    {

        string levelName;
        
        protected Rectangle position;
        protected Rectangle source;

        protected Rectangle verticalLine;

        protected int countdown;
        protected const int startCountdown = 70;

        Texture2D texture;
        SpriteFont font;

        GameLoop game;

        Vector2 textPosition;

        public LevelIntroScreen(GameLoop _game, SpriteFont _font) : base(_game)
        {
            position = new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
            verticalLine = new Rectangle(60, 0, 2, Game.GraphicsDevice.Viewport.Height);

            source = new Rectangle(0, 0, 0, 0);
            textPosition = new Vector2(70, Game.GraphicsDevice.Viewport.Width/2.5f);

            game = _game;
            font = _font;

            texture = new Texture2D(game.GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.White });
        }

        public string LevelName
        {
            get { return levelName;  }
            set { levelName = value; }
        }

        public void InitLevelScreen(string _levelName)
        {
            levelName = _levelName;
            countdown = startCountdown;
        }

        public void Update()
        {
            countdown -= 1;
            if (0 == countdown)
            {
                game.SetGameState(game.PrevGameState);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, source, Color.Black, 0, new Vector2(0, 0), SpriteEffects.None, 0.8f);
            spriteBatch.Draw(texture, verticalLine, source, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0.9f);
            spriteBatch.DrawString(font, levelName, textPosition, Color.White, 0, new Vector2(0,0), 1, SpriteEffects.None, 1.0f);
        }
    }
}
