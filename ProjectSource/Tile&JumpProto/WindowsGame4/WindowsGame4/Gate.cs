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
        private Level parentLevel;

        private const int spriteWidth = 16;
        private const int spriteHeight = 64;
        protected const int rowsPerScreen = 32;
        protected const int colsPerScreen = 64;

        // keep track of how much the level has shifted, total
        private int totalDeltaX = 0;

        Texture2D gateTexture;

        public Gate(Game game, int xPos, int yPos, int screenWidth, int screenHeight, Texture2D texture, Level _parentLevel)
        {
            parentLevel = _parentLevel;
            position = new Rectangle();
            position.Width = screenWidth / colsPerScreen;
            position.Height = screenHeight / rowsPerScreen;

            position.X = xPos * position.Width;
            position.Y = yPos * position.Height;

            leftHalf = new Rectangle(0, 0, spriteWidth, spriteHeight);
            rightHalf = new Rectangle(spriteWidth, 0, spriteWidth, spriteHeight);
            
            state = GateState.closed;
            status = GateStatus.closing;
            ticker = -1;
            gateTexture = texture;

            /* Block off the right half of the gate right now */
            Rectangle changeRect = new Rectangle((position.X / position.Width) + 1, position.Y / position.Height, 1, 4);
            parentLevel.modifyTiles(changeRect, CollisionType.invisible);
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
                    leftHalf.X = spriteWidth * 1 * 2;
                    if (status == GateStatus.opening) state = GateState.halfOpen;
                    else state = GateState.closed;
                }
                else if (state == GateState.halfOpen)
                {
                    //Gate is not fully open - make sure it won't let anything through now
                    Rectangle changeRect = new Rectangle(((position.X  + totalDeltaX)/ position.Width) + 1, position.Y / position.Height, 1, 4);
                    parentLevel.modifyTiles(changeRect, CollisionType.invisible);
                    leftHalf.X = spriteWidth * 2 * 2;
                    if (status == GateStatus.opening) state = GateState.open;
                    else state = GateState.quarterOpen;
                }
                else if (state == GateState.open)
                {
                    leftHalf.X = spriteWidth * 3 * 2;
                    if (status == GateStatus.closing)
                    {
                        state = GateState.halfOpen;
                    }
                    else
                    {
                        //Gate has fully opened. Stop the timer and change the underlying tiles to let it through
                        ticker = -1;
                        parentLevel.modifyTiles(new Rectangle(((position.X + totalDeltaX) / position.Width) + 1, position.Y / position.Height, 1, 4), CollisionType.passable);
                    }
                }
                rightHalf.X = leftHalf.X + spriteWidth;
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
            totalDeltaX += deltaX;
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
            spriteBatch.Draw(gateTexture, new Rectangle(position.X, position.Y, position.Width, position.Height * 4), leftHalf, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.15f);
            spriteBatch.Draw(gateTexture, new Rectangle(position.X + position.Width, position.Y, position.Width, position.Height * 4), rightHalf, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0.95f);
        }
    }
}