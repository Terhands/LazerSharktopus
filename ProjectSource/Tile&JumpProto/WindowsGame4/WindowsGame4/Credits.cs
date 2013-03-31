﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections;

namespace WindowsGame4
{
    // specialized class to handle displaying the credites at the end of the game
    class Credits : ScrollingTextScreen
    {

        public Credits(GameLoop _game, Texture2D _background, MusicManager _musicPlayer, int _songIndex, string[] _scrollingText, ArrayList _fonts, int[] _fontIndices, Justification _justification)
            : base(_game, _background, _musicPlayer, _songIndex, _scrollingText, _fonts, _fontIndices, _justification)
        { }

        public void Update()
        {
            if (musicPlayer.CurrSong != songIndex)
            {
                musicPlayer.Play(songIndex);
            }

            if (textPosition[textPosition.Length - 1].Y == 0)
            {
                fadeCounter = countdown;
            }
            else if (textPosition[textPosition.Length - 1].Y < 0)
            {
                fadeCounter -= 1;
            }
            else if (textPosition[textPosition.Length - 1].Y < screenHeight / 2)
            {
                currFadeValue += fadeIncrement;
            }

            if (fadeCounter == 0 && textPosition[textPosition.Length - 1].Y < 0)
            {
                game.SetGameState(GameLoop.GameState.titleScreen);
            }


            for (int i = 0; i < textPosition.Length; i++)
            {
                textPosition[i].Y -= scrollSpeed;
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color color = new Color(currFadeValue, currFadeValue, currFadeValue, 255);

            spriteBatch.Draw(background, bgPosition, bgSource, color, 0, new Vector2(0, 0), SpriteEffects.None, 0.01f);

            for (int i = 0; i < scrollingText.Length; i++)
            {
                spriteBatch.DrawString(((SpriteFont)fonts[fontIndices[i]]), scrollingText[i], textPosition[i], Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0.99f);
            }
        }

    }
}
