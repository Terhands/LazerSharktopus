
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;

namespace WindowsGame4
{
    class PlotScreen
    {

        protected int currPlotScreen;

        private ArrayList storyLine = new ArrayList 
                                        { 
                                            new string[] 
                                                { "Act 1",
                                                  " ",
                                                  " ",
                                                  "You wake up from a deep and troubled sleep.", 
                                                  "A dark castle took shape in your dreams.",
                                                  "It is time to rescue the princess."}, 
                                            new string[]
                                                { "Act 2",
                                                  " ",
                                                  " ",
                                                  "After days of hard travel you have arrived at the castle.",
                                                  "Instinctively you know that the princess is trapped inside,", 
                                                  "waiting for you to free her."}, 
                                            new string[]    
                                                { "Act 3",
                                                  " ",
                                                  " ",
                                                  "Princess: What??",
                                                  "Princess: Who are you and how did you get past my guards??",
                                                  " ",
                                                  "Robro: ... ... ... Beep ...",
                                                  " ",
                                                  "Princess: *muffled cry*",
                                                  " ",
                                                  "After safely securing the princess in a burlap sac,",
                                                  "you begin the long trek back to your master."} 
                                        };

        private ArrayList fontSelection = new ArrayList
                                            {
                                                new int[] {3, 3, 5, 5, 5, 5}, 
                                                new int[] {3, 3, 5, 5, 5, 5}, 
                                                new int[] {3, 3, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5}
                                            };

        private int[] textureSelection = { 22, 23, 17 };
        private int[] songSelection = { 7, 6, 8};

        protected ScrollingTextScreen displayScreen;
        protected GameLoop game;
        protected MusicManager musicPlayer;
        protected ArrayList textures;
        protected ArrayList fonts;

        public PlotScreen(GameLoop _game, MusicManager _musicPlayer, ArrayList _textures, ArrayList _fonts)
        {
            currPlotScreen = 0;

            game = _game;
            musicPlayer = _musicPlayer;
            textures = _textures;
            fonts = _fonts;
        }

        public void initPlotScreen()
        {
            displayScreen = new ScrollingTextScreen(game, (Texture2D)textures[textureSelection[currPlotScreen]], musicPlayer, songSelection[currPlotScreen], (string[])storyLine[currPlotScreen], fonts, (int[])fontSelection[currPlotScreen], ScrollingTextScreen.Justification.center);
            currPlotScreen += 1;
        }

        public bool IsEnding
        {
            get { return currPlotScreen >= songSelection.Length; }
        }

        public void Reset()
        {
            currPlotScreen = 0;
            initPlotScreen();
        }

        public void Update()
        {
            displayScreen.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            displayScreen.Draw(spriteBatch);
        }
    }
}
