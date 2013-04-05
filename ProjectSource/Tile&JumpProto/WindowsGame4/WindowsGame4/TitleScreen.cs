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
        Texture2D logo;

        GameLoop gameLoop;
        InputHandler inputHandler;
        int blinkCount;
        int logoCount;

        protected const int introSong = 2;
        protected const int fadeDelay = 4;
        protected int currFadeStep;
        int channelValue;
        int fadeIncrement = 1;
        int fadeCount = 0;

        MusicManager musicPlayer;

        public TitleScreen(GameLoop game, Texture2D _background, Texture2D _logo, SpriteFont _menuFont, MusicManager _musicPlayer, InputHandler _inputHandler) : base(game)
        {
            background = _background;
            logo = _logo;
            musicPlayer = _musicPlayer;

            gameLoop = game;
            menuFont = _menuFont;
            inputHandler = _inputHandler;
            blinkCount = 0;
            logoCount = 35;
            currFadeStep = fadeDelay;
            channelValue = 1;
        }

        public void Update()
        {
            if (inputHandler.isNewlyPressed(InputHandler.InputTypes.jump) ||
                inputHandler.isNewlyPressed(InputHandler.InputTypes.start))
                gameLoop.SetGameState(GameLoop.GameState.titleMenu);

            if (musicPlayer.isStopped || musicPlayer.CurrSong != introSong)
            {
                musicPlayer.Play(introSong);
            }

            
            if ((inputHandler.isNewlyPressed(InputHandler.InputTypes.jump) || inputHandler.isNewlyPressed(InputHandler.InputTypes.start)) && logoCount == 0)
            {
                gameLoop.SetGameState(GameLoop.GameState.titleMenu);
                logoCount = 35;
            }

            if (channelValue == 0 && logoCount == 0)
            {
                gameLoop.AnimatePlayer = true;
                blinkCount++;
                if (blinkCount > 50)
                {
                    blinkCount = 0;
                }
            }
            else if (channelValue == 0)
            {
                logoCount -= 1;
            }
            else
            {

                if (channelValue == 255 || channelValue == -1)
                {
                    fadeIncrement *= -1;
                }

                if (fadeIncrement > 0)
                {
                    channelValue += fadeIncrement;
                }
                else if (fadeCount % 2 == 0)
                {
                    channelValue += fadeIncrement;
                }
                

                fadeCount += 1;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (logoCount == 0)
            {
                Vector2 item0size = menuFont.MeasureString("Press Start");

                spriteBatch.Draw(background, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);

                if (blinkCount < 30)
                {
                    spriteBatch.DrawString(menuFont, "Press Start", new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2), Color.White, 0f, item0size / 2, 1.0f, SpriteEffects.None, 1.0f);
                }
            }
            else
            {
                // fade to black
                spriteBatch.Draw(logo, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), new Color(channelValue, channelValue, channelValue, 255));
            }
        }
    }
}