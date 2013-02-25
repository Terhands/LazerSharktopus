using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace WindowsGame4
{
    class JumpMeter : DrawableGameComponent, IGameObject
    {
        protected float jumpPower;
        protected const float maxJumpPower = 7.0f;
        protected const float minJumpPower = 3.0f;

        // the dimensions of the jump charge meter
        protected const int maxMeterWidth = 94;
        protected const int meterHeight = 16;

        // the actual meter that will be drawn to the screen
        protected Rectangle outline;
        protected Rectangle chargeBar;

        protected Color outlineColor;
        protected Color chargeColor;

        protected Texture2D rectangleTexture;

        public JumpMeter(Game game, int x, int y) : base(game)
        {
            jumpPower = 0;

            outline = new Rectangle(x, y, 100, 20);
            chargeBar = new Rectangle(x + 2, y + 2, 0, 16);

            rectangleTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            rectangleTexture.SetData(new Color[] { Color.White });

            outlineColor = Color.MidnightBlue;
            chargeColor = Color.Green;
        }

        public void reset()
        {
            jumpPower = 0;
        }

        public void chargeJumpPower()
        {
            if (jumpPower < minJumpPower)
            {
                jumpPower = minJumpPower;
            }
            else if(jumpPower < maxJumpPower)
            {
                jumpPower += 0.1f;
            }
        }

        public void drainJumpPower(float drainAmount)
        {
            Debug.Assert(drainAmount > 0);
            jumpPower -= drainAmount;
        }

        public float JumpPower
        {
            get { return jumpPower; }
            set { jumpPower = value; }
        }

        public void Update(Action action, int velocity)
        {
            if (jumpPower <= minJumpPower)
            {
                chargeBar.Width = 0;
            }
            else
            {
                chargeBar.Width = (int)(((jumpPower - minJumpPower) / (maxJumpPower - minJumpPower)) * maxMeterWidth);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(rectangleTexture, outline, outlineColor);
            spriteBatch.Draw(rectangleTexture, chargeBar, chargeColor);
        }
    }
}
