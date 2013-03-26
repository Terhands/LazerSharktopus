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

        int[,] mapLayout;

        Vector2[] guardLayout;
        Vector2[] torchLayout;
        Vector2[] leverLayout;

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
            int numMapCols = NextInt(tokenizedData);
            timeLimit = NextInt(tokenizedData);
            musicIndex = NextInt(tokenizedData);

            if (numMapRows * numMapCols > tokenizedData.Length)
            {
                Console.WriteLine("Error: Invalid Map Parameters");
                throw (new System.IO.IOException());
            }

            mapLayout = new int[numMapRows, numMapCols];

            // load the rows and columns value of the map into the mapLayout variable
            for (int i = 0; i < numMapRows; i++)
            {
                for (int j = 0; j < numMapCols; j++)
                {
                    mapLayout[i, j] = NextInt(tokenizedData);
                }
            }

            int numTorches = NextInt(tokenizedData);
            torchLayout = new Vector2[numTorches];

            // load the row/column indices for torch placement
            for (int i = 0; i < numTorches; i++)
            {
                torchLayout[i].X = NextInt(tokenizedData);
                torchLayout[i].Y = NextInt(tokenizedData);
            }

            int numGuards = NextInt(tokenizedData);
            guardLayout = new Vector2[numGuards];

            for(int i = 0; i < numGuards; i++)
            {
                guardLayout[i].X = NextInt(tokenizedData);
                guardLayout[i].Y = NextInt(tokenizedData);
            }

            int numLevers = NextInt(tokenizedData);
            leverLayout = new Vector2[numLevers];
            for (int i = 0; i < numLevers; i++)
            {
                leverLayout[i].X = NextInt(tokenizedData);
                leverLayout[i].Y = NextInt(tokenizedData);
                int numGates = NextInt(tokenizedData);

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

        public Vector2[] Guards
        {
            get { return guardLayout; }
        }

        public Vector2[] Levers
        {
            get { return leverLayout; }
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
