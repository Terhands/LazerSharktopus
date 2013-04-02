using System;
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

        private static string[] credits = { "Lathraia",
                                            " ",
                                            "Developed By", 
                                            " ",
                                            " ",
                                            " ",
                                            "Lazer Sharktopus Games",
                                            "Programming Team",
                                            " ",
                                            " ",
                                            " ",
                                            "Teresa Hume",
                                            "Mike Fulton",
                                            "Mike Greve",
                                            "Alexa Elliot",
                                            "Alicia Spencer",
                                            "Music By DST",
                                            " ",
                                            " ",
                                            " ",
                                            "http://www.nosoapradio.us/",
                                            "Songs",
                                            " ",
                                            " ",
                                            " ",
                                            "Blue Mist - Intro Screen",
                                            "Aronara - Level Music",
                                            "Angry Robot - Level Music",
                                            "Starry Book - End Credits",
                                            "Sound Effects From",
                                            " ",
                                            " ",
                                            " ",
                                            "http://soundbible.com",
                                            " ",
                                            " ",
                                            " ",
                                            "A Sharktopus Production",
                                          };

        private static int[] creditFonts = { 3, 4, 
                                             3, 4, 4, 4, 4, 
                                             3, 4, 4, 4, 4, 4, 4, 4, 4, 
                                             3, 4, 4, 4, 4, 
                                             3, 4, 4, 4, 4, 4, 4, 4, 
                                             3, 4, 4, 4, 4, 4, 4, 4, 4 };

        protected const string thankYou = "Thank You For Playing!";

        protected int thanksWidth;

        private static Justification _justification = Justification.left;
        private static int _songIndex = 3;

        private const int maxFadeValue = 255;

        public Credits(GameLoop _game, Texture2D _background, MusicManager _musicPlayer, ArrayList _fonts)
            : base(_game, _background, _musicPlayer, _songIndex, credits, _fonts, creditFonts, _justification)
        {
            thanksWidth = (int)((SpriteFont)fonts[3]).MeasureString(thankYou).X;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Color color = new Color(currFadeValue, currFadeValue, currFadeValue, 255);

            spriteBatch.Draw(background, bgPosition, bgSource, color, 0, new Vector2(0, 0), SpriteEffects.None, 0.01f);

            for (int i = 0; i < scrollingText.Length; i++)
            {
                spriteBatch.DrawString(((SpriteFont)fonts[fontIndices[i]]), scrollingText[i], textPosition[i], Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0.99f);
            }

            if (currFadeValue < 255)
            {
                int colorChannel = maxFadeValue - currFadeValue;
                Color fadeIn = new Color(colorChannel, colorChannel, colorChannel, colorChannel);
                spriteBatch.DrawString((SpriteFont)fonts[3], thankYou, new Vector2(screenCenter - thanksWidth/2, screenHeight/2) , fadeIn, 0, new Vector2(0,0), 1, SpriteEffects.None, 0.99f); 
            }
        }
    }
}
