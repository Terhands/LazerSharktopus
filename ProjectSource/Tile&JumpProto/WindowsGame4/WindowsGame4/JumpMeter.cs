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
        protected const int outlineWidth = 50;
        protected const int outlineHeight = 10;
        protected const int maxMeterWidth = 45;
        protected const int meterPadding = 2;

        // the actual meter that will be drawn to the screen
        protected Rectangle outline;
        protected Rectangle chargeBar;

        protected Color outlineColor;
        protected Color chargeColor;

        protected Texture2D rectangleTexture;

        public JumpMeter(Game game, int xCenter, int yCenter)
            : base(game)
        {
            jumpPower = 0;

            int x = xCenter - (outlineWidth / 2);
            int y = yCenter - (outlineHeight / 2);

            outline = new Rectangle(x, y, outlineWidth, outlineHeight);
            chargeBar = new Rectangle(x + meterPadding, y + meterPadding, 0, outlineHeight - (2 * meterPadding));

            rectangleTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            rectangleTexture.SetData(new Color[] { Color.White });

            outlineColor = Color.Black;
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
            else if (jumpPower < maxJumpPower)
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

        public void setMeterPosition(int xCenter, int yCenter)
        {
            outline.X = xCenter - (outline.Width / 2);
            chargeBar.X = xCenter - (chargeBar.Width / 2);

            outline.Y = yCenter - (outline.Height / 2);
            chargeBar.Y = yCenter - (chargeBar.Height / 2);
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
            if (jumpPower > minJumpPower)
            {
                spriteBatch.Draw(rectangleTexture, outline, outlineColor);
                spriteBatch.Draw(rectangleTexture, chargeBar, chargeColor);
            }
        }
    }
}
