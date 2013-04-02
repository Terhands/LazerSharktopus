using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame4
{
    class TitleMenu : Microsoft.Xna.Framework.GameComponent
    {
        SpriteFont menuFont;
        Texture2D background;

        int selectedIndex;

        GameLoop gameLoop;

        InputHandler inputHandler;

        public TitleMenu(GameLoop game, Texture2D _background, SpriteFont _menuFont, InputHandler _inputHandler) : base(game)
        {
            background = _background;
            gameLoop = game;
            menuFont = _menuFont;
            selectedIndex = 0;
            inputHandler = _inputHandler;
        }

        public void Update()
        {
         
            if (inputHandler.isNewlyPressed(InputHandler.InputTypes.down))
                selectedIndex++;
            if (inputHandler.isNewlyPressed(InputHandler.InputTypes.up))
                selectedIndex--;

            if (selectedIndex < 0) selectedIndex = 1;
            if (selectedIndex > 1) selectedIndex = 0;

            if (inputHandler.isNewlyPressed(InputHandler.InputTypes.start) ||
                inputHandler.isNewlyPressed(InputHandler.InputTypes.jump))
            {
                if (selectedIndex == 0)
                    gameLoop.SetGameState(GameLoop.GameState.levelIntro);
                else if (selectedIndex == 1)
                    gameLoop.Exit();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 item0size = menuFont.MeasureString("New Game");
            Vector2 item1size = menuFont.MeasureString("Quit");

            spriteBatch.Draw(background, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);
            if (selectedIndex == 0)
                spriteBatch.DrawString(menuFont, "New Game", new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2), Color.Yellow, 0f, item0size / 2, 1.0f, SpriteEffects.None, 1.0f);
            else
                spriteBatch.DrawString(menuFont, "New Game", new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2), Color.White, 0f, item0size / 2, 1.0f, SpriteEffects.None, 1.0f);

            if (selectedIndex == 1)
                spriteBatch.DrawString(menuFont, "Quit", new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height * 2 / 3), Color.Yellow, 0f, item1size / 2, 1.0f, SpriteEffects.None, 1.0f);
            else
                spriteBatch.DrawString(menuFont, "Quit", new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height * 2 / 3), Color.White, 0f, item1size / 2, 1.0f, SpriteEffects.None, 1.0f);
        }
    }
}