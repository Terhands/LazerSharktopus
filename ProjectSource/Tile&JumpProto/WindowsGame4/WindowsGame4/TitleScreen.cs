using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame4
{
    class TitleScreen : Microsoft.Xna.Framework.GameComponent
    {
        SpriteFont menuFont;
        Texture2D background;

        GameLoop gameLoop;

        KeyboardState keyState;

        public TitleScreen(GameLoop game, Texture2D _background, SpriteFont _menuFont) : base(game)
        {
            background = _background;
            gameLoop = game;
            menuFont = _menuFont;
            keyState = Keyboard.GetState();
        }

        public void Update()
        {
            keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Enter))
                gameLoop.State = GameLoop.States.titleMenu;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 item0size = menuFont.MeasureString("Press Enter to Start");

            spriteBatch.Draw(background, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);

            spriteBatch.DrawString(menuFont, "Press Enter to Start", new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2), Color.White, 0f, item0size / 2, 1.0f, SpriteEffects.None, 1.0f);
        }
    }
}