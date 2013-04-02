using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections;

namespace WindowsGame4
{
    // class to manage text scrolling from the bottom of a screen to the top of a screen
    class ScrollingTextScreen
    {
        protected const float scrollSpeed = 0.5f;
        protected int songIndex;
        protected int screenCenter;
        protected int screenHeight;

        protected int fadeCounter;
        protected const int countdown = 100;

        protected const int fadeIncrement = -1;
        protected int currFadeValue;

        protected GameLoop game;
        protected MusicManager musicPlayer;

        protected Texture2D background;
        protected Rectangle bgPosition;
        protected Rectangle bgSource;

        protected int[] fontIndices;
        protected ArrayList fonts;
        protected String[] scrollingText;
        protected Vector2[] textPosition;
        protected Justification justification;

        public enum Justification { right, left, center };

        public ScrollingTextScreen(GameLoop _game, Texture2D _background, MusicManager _musicPlayer, int _songIndex, string[] _scrollingText, ArrayList _fonts, int[] _fontIndices, Justification _justification)
        {
            game = _game;

            background = _background;
            musicPlayer = _musicPlayer;
            songIndex = _songIndex;

            scrollingText = _scrollingText;
            fonts = _fonts;
            fontIndices = _fontIndices;
            justification = _justification;

            screenCenter = game.GraphicsDevice.Viewport.Width / 2;
            screenHeight = game.GraphicsDevice.Viewport.Height;

            bgPosition = new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, screenHeight);
            bgSource = new Rectangle(0, 0, background.Width, background.Height);

            textPosition = new Vector2[scrollingText.Length];
            initScrollingTextScreen();
        }

        public void initScrollingTextScreen()
        {
            int currTextHeight = game.GraphicsDevice.Viewport.Height;
            int heightPadding = -2;

            // 255 is the maximum colour channel value - going to fade from white to black
            currFadeValue = 255;

            for (int i = 0; i < scrollingText.Length; i++)
            {
                textPosition[i] = ((SpriteFont)fonts[fontIndices[i]]).MeasureString(scrollingText[i]);
                if (justification == Justification.center)
                {
                    textPosition[i].X = screenCenter - textPosition[i].X / 2;
                }

                else if (justification == Justification.left)
                {
                    textPosition[i].X = screenCenter / 6;
                }
                else
                {
                    textPosition[i].X = screenCenter * 7 / 4 - textPosition[i].X;
                }

                
                currTextHeight += (int)textPosition[i].Y + heightPadding;
                textPosition[i].Y = currTextHeight + (int)textPosition[i].Y;
            }
        
        }

        public virtual void Update()
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
            else if (textPosition[textPosition.Length - 1].Y < screenHeight / 3)
            {
                currFadeValue += fadeIncrement;
            }

            for (int i = 0; i < textPosition.Length; i++)
            {
                textPosition[i].Y -= scrollSpeed;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Color color = new Color(currFadeValue, currFadeValue, currFadeValue, 255);

            spriteBatch.Draw(background, bgPosition, bgSource, color, 0, new Vector2(0, 0), SpriteEffects.None, 0.01f);

            for (int i = 0; i < scrollingText.Length; i++)
            {
                spriteBatch.DrawString(((SpriteFont)fonts[fontIndices[i]]), scrollingText[i], textPosition[i], Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0.99f);
            }
        }

        public bool IsDone
        {
            get { return textPosition[textPosition.Length - 1].Y < 0 && currFadeValue <= 0; }
        }
    }
}
