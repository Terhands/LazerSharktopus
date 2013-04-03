using System;
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
        private float spriteDepth;
        
        public GameTimer(int time, SpriteFont spriteFont)
        {
            levelTime = time;
            beatCounter = 30;
            timerFont = spriteFont;
            spriteDepth = 1.0f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Do not draw if there is no timer for this level
            if (levelTime != -1 )
                spriteBatch.DrawString(timerFont, levelTime.ToString(), new Vector2(20, 15), Color.Gold, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, spriteDepth);
        }

        public void Update()
        {
            // If there's no level timer, then don't worry about anything
            if (levelTime != -1)
            {
                beatCounter--;
                if (beatCounter <= 0)
                {
                    levelTime--;
                    beatCounter = 30;
                }
            }
        }

        public bool isFinished()
        {
            if (levelTime == 0)
                return true;
            else
                return false;
        }
    }
}
