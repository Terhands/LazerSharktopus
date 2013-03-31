using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace WindowsGame4
{
    /* Health meter class, creates a health meter for the player */
    class HealthMeter : DrawableGameComponent, IGameObject
    {
        // the dimensions of the health meter
        protected const int healthWidth = 200;
        protected const int healthHeight = 10;
        protected const int healthHeightLooksGood = 20;

        protected const int healthPadding = 2;

        protected float spriteDepth;

        protected Rectangle outline;
        protected Rectangle healthBar;

        protected Color outlineColor;
        protected Color healthBarColor;

        protected Texture2D rectangleTexture;

        protected int width; // Width of the health meter, ties into player health

        /* Constructor, initializes position and size */
        public HealthMeter(Game game, int xCenter, int yCenter, float layer)
            : base(game)
        {
            spriteDepth = layer;

            int screenWidth = game.GraphicsDevice.Viewport.Width;

            outline = new Rectangle((screenWidth/2) - (healthWidth/2) - healthPadding, healthHeightLooksGood - healthPadding, healthWidth + 2 * healthPadding, healthHeight + 2 * healthPadding);
            healthBar = new Rectangle((screenWidth / 2) - (healthWidth / 2), healthHeightLooksGood, healthWidth, healthHeight);

            rectangleTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            rectangleTexture.SetData(new Color[] { Color.White });

            outlineColor = Color.GhostWhite;
            healthBarColor = Color.Red;
            width = healthBar.Width;
        }

        /* Lowers the width of the health bar when bolt is thrown */
        public void lowerHealthMeter()
        {
            width -= 20;
        }

        public int Health
        {
            get { return width; }
        }

        /* Update size of health bar */
        public void Update(Action action, int velocity)
        {
            healthBar.Width = width;
        }

        /* Draws the health meter */
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(rectangleTexture, outline, new Rectangle(0, 0, 0, 0), outlineColor, 0, new Vector2(0, 0), SpriteEffects.None, spriteDepth);
            spriteBatch.Draw(rectangleTexture, healthBar, new Rectangle(0, 0, 0, 0), healthBarColor, 0, new Vector2(0, 0), SpriteEffects.None, spriteDepth + 0.1f);
        }
    }
}