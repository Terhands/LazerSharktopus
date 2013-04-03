using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame4
{
    class Gate : IDynamicGameObject
    {
        public enum GateState { closed, quarterOpen, halfOpen, open};
        public enum GateStatus { opening, closing };
        private int ticker;
     
        private Rectangle position;
        private Rectangle leftHalf;
        private Rectangle rightHalf;
        private GateState state;
        private GateStatus status;

        private Rectangle region;

        private const int spriteWidth = 32;
        private const int spriteHeight = 64;

        Texture2D gateTexture;

        public Gate(Game game, int xPos, int yPos, int screenWidth, int screenHeight, Texture2D texture)
        {
            position = new Rectangle(xPos, yPos, screenWidth * 2/64, screenHeight * 4 / 32);
            
            region = new Rectangle(0, 0, spriteWidth, spriteHeight);

            leftHalf = new Rectangle(0, 0, spriteWidth / 2, spriteHeight);
            rightHalf = new Rectangle(spriteWidth / 2, 0, spriteWidth / 2, spriteHeight);

            state = GateState.closed;
            status = GateStatus.closing;
            ticker = -1;
            gateTexture = texture;
        }
        
        public void Update(GameTime gameTime)
        {
            if (ticker == 0)
            {
                if (state == GateState.closed)
                {
                    leftHalf.X = spriteWidth * 0;
                    if (status == GateStatus.opening) state = GateState.quarterOpen;
                    else ticker = -1;
                }
                else if (state == GateState.quarterOpen)
                {
                    leftHalf.X = spriteWidth * 1;
                    if (status == GateStatus.opening) state = GateState.halfOpen;
                    else state = GateState.closed;
                }
                else if (state == GateState.halfOpen)
                {
                    leftHalf.X = spriteWidth * 2;
                    if (status == GateStatus.opening) state = GateState.open;
                    else state = GateState.quarterOpen;
                }
                else if (state == GateState.open)
                {
                    leftHalf.X = spriteWidth * 3;
                    if (status == GateStatus.closing) state = GateState.halfOpen;
                    else ticker = -1;
                }
                rightHalf.X = leftHalf.X + spriteWidth / 2;
            }
            if (ticker >= 0)
            {
                ticker++;
                if (ticker >= 15) ticker = 0;
            }
        }

        public void Update(Action action, int myInt)
        {
        }

        public void flip()
        {
            if (status == GateStatus.closing && state == GateState.closed)
            {
                status = GateStatus.opening;
                state = GateState.quarterOpen;
            }
            else if (status == GateStatus.opening && state == GateState.open)
            {
                status = GateStatus.closing;
                state = GateState.halfOpen;
            }
            ticker = 0;
        }

        public void reposition(int deltaX)
        {
            position.X -= deltaX;
        }

        public void HandleCollision(IList<ITile> tiles)
        {

        }

        public Rectangle GetPosition()
        {
            return position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(gateTexture, new Rectangle(position.X, position.Y, spriteWidth/2, spriteHeight), leftHalf, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.15f);
            spriteBatch.Draw(gateTexture, new Rectangle(position.X + spriteWidth/2, position.Y, spriteWidth/2, spriteHeight), rightHalf, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.95f);
        }
    }
}