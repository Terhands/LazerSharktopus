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
                    textPosition[i].X = screenCenter / 4;
                }
                else
                {
                    textPosition[i].X = screenCenter * 7 / 4 - textPosition[i].X;
                }

                
                currTextHeight += (int)textPosition[i].Y + heightPadding;
                textPosition[i].Y = currTextHeight + (int)textPosition[i].Y;
            }
        
        }
    }
}
