using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame4
{
    class GameOver : Microsoft.Xna.Framework.GameComponent
    {
        SpriteFont menuFont;
        Texture2D background;

        int selectedIndex;

        GameLoop gameLoop;

        InputHandler inputHandler;
        public GameOver(GameLoop game, Texture2D _background, SpriteFont _menuFont, InputHandler _inputHandler) : base(game)
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

            if (inputHandler.isNewlyPressed(InputHandler.InputTypes.jump) ||
                inputHandler.isNewlyPressed(InputHandler.InputTypes.start))
            {
                if (selectedIndex == 0)
                    gameLoop.SetGameState(GameLoop.GameState.levelIntro);
                else if (selectedIndex == 1)
                    gameLoop.State = GameLoop.GameState.titleMenu;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);
            if (selectedIndex == 0)
                spriteBatch.DrawString(menuFont, "Retry Level", new Vector2(650, 10), Color.Yellow, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 1.0f);
            else
                spriteBatch.DrawString(menuFont, "Retry Level", new Vector2(650, 10), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 1.0f);
            
            if (selectedIndex == 1)
                spriteBatch.DrawString(menuFont, "Main Menu", new Vector2(650, 90), Color.Yellow, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 1.0f);
            else
                spriteBatch.DrawString(menuFont, "Main Menu", new Vector2(650, 90), Color.White, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 1.0f);
        }
    }
}