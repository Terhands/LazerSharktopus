    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;

namespace WindowsGame4
{
    class GameLoader
    {

        protected string[] levelFiles;
        protected string[] tutorialFiles;
        protected string[] textureFiles;
        protected string[] soundFiles;
        protected string[] songFiles;
        protected string[] fontFiles;

        // method to load the basic game configuration - i.e. levels, sounds, textures, etc.
        public void Load(string fileName)
        {
            string configFile = System.IO.File.ReadAllText(fileName);

            levelFiles = LoadTag(configFile, @"<levels>([.\s\w\d-]*)</levels>");
            tutorialFiles = LoadTag(configFile, @"<tutorials>([.\s\w\d-]*)</tutorials>");
            textureFiles = LoadTag(configFile, @"<.*textures.*>([.\s\w\d-]*)</textures>");
            soundFiles = LoadTag(configFile, @"<.*soundEffects.*>([.\s\w\d-]*)</soundEffects>");
            songFiles = LoadTag(configFile, @"<.*songs.*>([.\s\w\d-]*)</songs>");
            fontFiles = LoadTag(configFile, @"<fonts>([.\s\w\d-]*)</fonts>");
        }

        // procedure to load all of the data nested between tagRegex xml tags
        public string[] LoadTag(string source, string tagRegex)
        {
            //Use a regex match with the proper regex string + capture group to capture the model name within the XML tags
            Match dataMatch = Regex.Match(source, tagRegex);
            string raw_data = dataMatch.Groups[1].Value.Trim(); // trim out extra whitespace
            raw_data = Regex.Replace(raw_data, @"\n", " "); // remove newlines
            raw_data = Regex.Replace(raw_data, @"[\s\t]{1,}", " "); // only have one space between elements in levels
            string[] tokenizedData = raw_data.Split(' '); // split up the level files by spaces
            int numData = tokenizedData.Length;
            string[] tagData = new string[numData];

            for (int i = 0; i < numData; i++)
            {
                tagData[i] = tokenizedData[i];
            }

            return tagData;
        }

        public GameLoader(string fileName)
        {
            Load(fileName);
        }

        public int NumTextures
        {
            get { return textureFiles.Length; }
        }

        public int NumLevels
        {
            get { return levelFiles.Length; }
        }

        public int NumTutorials
        {
            get { return tutorialFiles.Length; }
        }

        public int NumSoundEffects
        {
            get { return soundFiles.Length; }
        }

        public int NumSongs
        {
            get { return songFiles.Length; }
        }

        public string[] LevelFiles
        {
            get { return levelFiles; }
        }

        public string[] TutorialFiles
        {
            get { return tutorialFiles; }
        }

        public string[] SpriteFiles
        {
            get { return SpriteFiles; }
        }

        public int NumFonts
        {
            get { return fontFiles.Length; }
        }

        public string getTextureFile(int i)
        {
            Debug.Assert(i < textureFiles.Length);
            return textureFiles[i];
        }

        public string getSoundFile(int i)
        {
            Debug.Assert(i < soundFiles.Length);
            return soundFiles[i];
        }

        public string getFontFile(int i)
        {
            Debug.Assert(i < fontFiles.Length);
            return fontFiles[i];
        }

        public string getSongFile(int i)
        {
            Debug.Assert(i < songFiles.Length);
            return songFiles[i];
        }

    }
}
