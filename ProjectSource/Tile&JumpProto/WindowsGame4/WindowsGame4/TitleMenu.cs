﻿using System;
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

        KeyboardState keyState;
        KeyboardState prevKeyState;

        public TitleMenu(GameLoop game, Texture2D _background, SpriteFont _menuFont) : base(game)
        {
            background = _background;
            gameLoop = game;
            menuFont = _menuFont;
            selectedIndex = 0;
            keyState = Keyboard.GetState();
            prevKeyState = Keyboard.GetState();
        }

        public void Update()
        {
            keyState = Keyboard.GetState();

            if (prevKeyState == null)
            {
                prevKeyState = keyState;
            }

            // hitting the S key selects the next item in the list
            if (keyState.IsKeyDown(Keys.S) && prevKeyState.IsKeyUp(Keys.S))
            {
                selectedIndex++;
            }
            // hitting the w key selects the previous item in the list
            if (keyState.IsKeyDown(Keys.W) && prevKeyState.IsKeyUp(Keys.W))
            {
                selectedIndex--;
            }

            // doubly link the selectable items on the screen (hitting up at the top moves the cursor to the bottom)
            if (selectedIndex < 0) { selectedIndex = 2; }
            if (selectedIndex > 2) { selectedIndex = 0; }

            if (keyState.IsKeyDown(Keys.Enter) && prevKeyState.IsKeyUp(Keys.Enter))
            {
                if (selectedIndex == 0)
                {
                    gameLoop.SetGameState(GameLoop.GameState.level);
                    gameLoop.SetGameState(GameLoop.GameState.levelIntro);
                }
                else if (selectedIndex == 1)
                {
                    gameLoop.SetGameState(GameLoop.GameState.tutorial);
                    gameLoop.SetGameState(GameLoop.GameState.levelIntro);
                }
                else if (selectedIndex == 2)
                {
                    gameLoop.Exit();
                }
            }

            prevKeyState = keyState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 newGameSize = menuFont.MeasureString("New Game");
            Vector2 tutorialSize = menuFont.MeasureString("Tutorial");
            Vector2 quitSize = menuFont.MeasureString("Quit");

            spriteBatch.Draw(background, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);
            if (selectedIndex == 0)
            {
                spriteBatch.DrawString(menuFont, "New Game", new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2), Color.Yellow, 0f, newGameSize / 2, 1.0f, SpriteEffects.None, 1.0f);
            }
            else
            {
                spriteBatch.DrawString(menuFont, "New Game", new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2), Color.White, 0f, newGameSize / 2, 1.0f, SpriteEffects.None, 1.0f);
            }

            if (selectedIndex == 1)
            {
                spriteBatch.DrawString(menuFont, "Tutorial", new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2 + (3/2 * newGameSize.Y)), Color.Yellow, 0f, tutorialSize / 2, 1.0f, SpriteEffects.None, 1.0f);
            }
            else
            {
                spriteBatch.DrawString(menuFont, "Tutorial", new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2 + (3/2 * newGameSize.Y)), Color.White, 0f, tutorialSize / 2, 1.0f, SpriteEffects.None, 1.0f);
            }

            if (selectedIndex == 2)
            {
                spriteBatch.DrawString(menuFont, "Quit", new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2 + (5 / 2 * newGameSize.Y)), Color.Yellow, 0f, quitSize / 2, 1.0f, SpriteEffects.None, 1.0f);
            }
            else
            {
                spriteBatch.DrawString(menuFont, "Quit", new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2 + (5 / 2 * newGameSize.Y)), Color.White, 0f, quitSize / 2, 1.0f, SpriteEffects.None, 1.0f);
            }
        }
    }
}