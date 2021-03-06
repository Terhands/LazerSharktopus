﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;

namespace WindowsGame4
{
    class Button : IDynamicGameObject
    {
        public enum Status { on, off };

        private Rectangle position;
        private Rectangle region;

        private Status status;

        private int spriteWidth = 32;
        private int spriteHeight = 32;

        private List<Spout> spoutsControlled;
        
        Texture2D spoutTexture;

        private int resetCounter = 0;
        private const int RESET_TIME = 150;

        public Button(Game game, int xPos, int yPos, int screenWidth, int screenHeight, List<Spout> spouts, Texture2D texture)
        {
            position = new Rectangle(xPos, yPos, 24, 24);
            status = Status.on;
            spoutTexture = texture;
            region = new Rectangle(0, 0, spriteWidth, spriteHeight);
            spoutsControlled = spouts;
        }

        public void Update(GameTime gameTime)
        {
            if (status == Status.off)
            {
                resetCounter--;
                Console.WriteLine(resetCounter);
                if (resetCounter == 0)
                {
                    status = Status.on;
                    region = new Rectangle(0, 0, spriteWidth, spriteHeight);
                }
            }
        }

        public void Update(Action action, int myInt)
        {

        }

        public void flip()
        {
            Console.WriteLine("Flip");
            //if the button is already depressed, then do nothing
            if (status == Status.off) return;
            
            status = Status.off;
            region = new Rectangle(spriteWidth, 0, spriteWidth, spriteHeight);
            resetCounter = RESET_TIME;

            foreach (Spout spout in spoutsControlled)
            {
                spout.activate();
            }
        }

        public void reposition(int deltaX)
        {
            position.X -= deltaX;
        }

        public void HandleCollision(IList<ITile> tiles)
        {
            bool collided = false;

            foreach (ITile t in tiles)
            {

                if (t.isInCollision(this))
                {
                    collided = true;
                    break;
                }
            }

            if (collided) flip();
        }

        public Rectangle GetPosition()
        {
            return position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
                spriteBatch.Draw(spoutTexture, position, region, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.2f);
        }
    }
}
