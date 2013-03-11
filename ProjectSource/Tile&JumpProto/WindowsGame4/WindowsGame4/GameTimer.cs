﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace WindowsGame4
{
    class GameTimer
    {
        protected int levelTime;
        private int beatCounter;
        private SpriteFont timerFont;
        
        public GameTimer(int time, SpriteFont spriteFont)
        {
            levelTime = time;
            beatCounter = 30;
            timerFont = spriteFont;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(timerFont, levelTime.ToString(), new Vector2(10, 10), Color.Black);
        }

        public void Update()
        {
            beatCounter--;
            if (beatCounter <= 0)
            {
                levelTime--;
                beatCounter = 30;
            }
        }
    }
}
