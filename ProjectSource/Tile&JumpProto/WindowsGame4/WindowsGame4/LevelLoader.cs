using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace WindowsGame4
{
    class LevelLoader
    {

        string[] levelFiles;

        string levelName;
        int currentLevel;
        int dataPos;
        int timeLimit;
        int musicIndex;
        int bgroundIndex;

        int[,] mapLayout;

        Vector3[] guardLayout;
        Vector2[] torchLayout;
        Vector2[] leverLayout;
        int[][] leverGateMaps;
        Vector2[] gateLayout;
        Vector2[] boxOfBoltsLayout;
        Vector2[] buttonLayout;
        Vector3[] spoutLayout;
        int[][] buttonSpoutMaps;


        public LevelLoader(string[] _levelFiles)
        {
            levelFiles = _levelFiles;

            currentLevel = 0;
        }

        public int TimeLimit
        {
            get { return timeLimit; }
        }

        public int LevelMusic
        {
            get { return musicIndex; }
        }

        public int LevelBackground
        {
            get { return bgroundIndex; }
        }

        public void LoadLevel(int levelIndex)
        {
            Debug.Assert(levelIndex < levelFiles.Length);

            // read in the entire level configuration file
            string levelFile = System.IO.File.ReadAllText(@"Content\" + levelFiles[levelIndex]);

            // grab the entire file
            Match dataMatch = Regex.Match(levelFile, @"[~(.\s\w\d-)]*");
            string rawData = dataMatch.Groups[0].Value.Trim(); // trim all whitespace
            levelName = rawData.Split('~')[0];
            rawData = Regex.Replace(rawData, levelName + "~", " ");
            rawData = Regex.Replace(rawData, @"\n", " "); // remove newlines
            rawData = Regex.Replace(rawData, @"[\s\t]{1,}", " "); // only have one space between elements
            rawData = rawData.Trim(); // trim the start
            string[] tokenizedData = rawData.Split(' '); // split up the file elements by spaces

            // reset the data position in the file tokens to the first element
            dataPos = 0;

            int numMapRows = NextInt(tokenizedData);
            System.Console.WriteLine("YA 1");
            int numMapCols = NextInt(tokenizedData);
            System.Console.WriteLine("YA 2");
            timeLimit = NextInt(tokenizedData);
            System.Console.WriteLine("YA 3");
            musicIndex = NextInt(tokenizedData);
            System.Console.WriteLine("YA 4");
            bgroundIndex = NextInt(tokenizedData);
            System.Console.WriteLine("YA 5");

            if (numMapRows * numMapCols > tokenizedData.Length)
            {
                Console.WriteLine("Error: Invalid Map Parameters");
                throw (new System.IO.IOException());
            }

            mapLayout = new int[numMapRows, numMapCols];
            System.Console.WriteLine("YA 6");
            // load the rows and columns value of the map into the mapLayout variable
            for (int i = 0; i < numMapRows; i++)
            {
                for (int j = 0; j < numMapCols; j++)
                {
                    mapLayout[i, j] = NextInt(tokenizedData);
                   
                }
            }
            System.Console.WriteLine("YA 7");
            int numTorches = NextInt(tokenizedData);
            System.Console.WriteLine("YA 8");
            torchLayout = new Vector2[numTorches];

            // load the row/column indices for torch placement
            for (int i = 0; i < numTorches; i++)
            {
                torchLayout[i].X = NextInt(tokenizedData);
                torchLayout[i].Y = NextInt(tokenizedData);
            }
            System.Console.WriteLine("YA 9");
            int numGuards = NextInt(tokenizedData);
            System.Console.WriteLine("YA 10");
            guardLayout = new Vector3[numGuards];

            for(int i = 0; i < numGuards; i++)
            {
                guardLayout[i].X = NextInt(tokenizedData);
                guardLayout[i].Y = NextInt(tokenizedData);
                guardLayout[i].Z = NextInt(tokenizedData);
            }

            int numGates = NextInt(tokenizedData);
            System.Console.WriteLine("YA 11");
            gateLayout = new Vector2[numGates];
            for (int i = 0; i < numGates; i++)
            {
                gateLayout[i].X = NextInt(tokenizedData);
                gateLayout[i].Y = NextInt(tokenizedData);
            }
            System.Console.WriteLine("YA 12");
            int numLevers = NextInt(tokenizedData);
            System.Console.WriteLine("YA 12.1");
            leverLayout = new Vector2[numLevers];
            leverGateMaps = new int[numLevers][];
            for (int i = 0; i < numLevers; i++)
            {
                leverLayout[i].X = NextInt(tokenizedData);
                System.Console.WriteLine("YA 13");
                leverLayout[i].Y = NextInt(tokenizedData);
                System.Console.WriteLine("YA 14");
                int numGatesForLever = NextInt(tokenizedData);
                System.Console.WriteLine("YA 15");
                // And then we read in the indexes of the gates we'll be using for this lever
                leverGateMaps[i] = new int[numGatesForLever];
                for (int j = 0; j < numGatesForLever; j++)
                {
                    leverGateMaps[i][j] = NextInt(tokenizedData);
                }
            }

            // Loading in the boxes of bolts for the player to gather
            int numBoxes = NextInt(tokenizedData); // Next int is number of boxes in level
            boxOfBoltsLayout = new Vector2[numBoxes];
            System.Console.WriteLine("YA 16");
            for (int i = 0; i < numBoxes; i++)
            {
                boxOfBoltsLayout[i].X = NextInt(tokenizedData); // Next int is the X poistion for the box
                boxOfBoltsLayout[i].Y = NextInt(tokenizedData); // Next int is the Y position for the box
            }

            int numSpouts = NextInt(tokenizedData);
            spoutLayout = new Vector3[numSpouts];
            for (int i = 0; i < numSpouts; i++)
            {
                spoutLayout[i].X = NextInt(tokenizedData);
                spoutLayout[i].Y = NextInt(tokenizedData);
                spoutLayout[i].Z = NextInt(tokenizedData);
            }

            int numButtons = NextInt(tokenizedData);
            buttonLayout = new Vector2[numButtons];
            buttonSpoutMaps = new int[numSpouts][];
            for (int i = 0; i < numButtons; i++)
            {
                buttonLayout[i].X = NextInt(tokenizedData);
                buttonLayout[i].Y = NextInt(tokenizedData);
                int numSpoutsForButton = NextInt(tokenizedData);
                buttonSpoutMaps[i] = new int[numSpoutsForButton];
                for (int j = 0; j < numSpoutsForButton; j++)
                {
                    buttonSpoutMaps[i][j] = NextInt(tokenizedData);
                }
            }
        }

        // read the next integer from data
        public int NextInt(string[] data)
        {
            dataPos += 1;
            return Convert.ToInt32(data[dataPos - 1]);
        }

        public int[,] Map
        {
            get { return mapLayout; }
        }

        public Vector2[] Torches
        {
            get { return torchLayout; }
        }

        public Vector3[] Guards
        {
            get { return guardLayout; }
        }

        public Vector2[] Levers
        {
            get { return leverLayout; }
        }

        /* Return the layout for the boxes of bolts */
        public Vector2[] BoxesOfBolts
        {
            get { return boxOfBoltsLayout; }
        }

        public Vector2[] Buttons
        {
            get { return buttonLayout; }
        }

        public Vector3[] Spouts
        {
            get { return spoutLayout; }
        }

        public int[][] ButtonSpoutMaps
        {
            get { return buttonSpoutMaps; }
        }

        public int[][] levelGateMaps
        {
            get { return leverGateMaps; }
        }

        public Vector2[] Gates
        {
            get { return gateLayout; }
        }

        public int NumLevels
        {
            get { return levelFiles.Length; }
        }

        public string LevelName
        {
            get { return levelName; }
        }

    }
}
