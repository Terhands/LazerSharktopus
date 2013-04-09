using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using Microsoft.Xna.Framework.Audio;

namespace WindowsGame4
{
    class Player : ADynamicGameObject, IPlayer
    {
        protected Texture2D sprite;
        protected ArrayList sounds;
        protected Rectangle source;

        protected Rectangle leftRightCollisionRectangle;

        protected Action facingDirection;

        // hidden goes from 0->1 1 being totallly hidden, 0 being standing yelling & flailing in the middle of a well lit room
        protected float hidden;
        protected bool isJumping;
        protected bool isStopped;
        protected bool isDead;
        protected bool hasReachedGoal;
        protected bool displayHealthBar;

        protected float spriteDepth = 0.8f;

        // may replace this with a jump meter object later on
        protected JumpMeter jumpMeter;
        protected int playerPadding = 8;

        // the speed that player starts falling
        protected const float startFalling = -0.25f;
        protected const int maxPainlessFall = 200;
        protected int fallDistance;


        // robro's change in height from crouching to standing up straight
        protected const int crouchDiff = 30;

        protected int frameCountCol = 1; // Which frame we are.  Values = {0, 1, 2, 3, 4} this 4 = crouch only in row 0 
        protected int frameCountRow = 0; //Value = {0,1} 1 = left direction , 0 = right directions
        protected int frameSkipX = 75; // How much to move the frame in X when we increment a frame--X distance between top left corners.
        protected int frameSkipY = 127; // how much to move the frame in y when we change directions 
        protected int frameStartX = 1; // X of top left corner of frame 0. 
        protected int frameStartY = 1; // Y of top left corner of frame 0.
        protected int frameWidth = 75; // X of right minus X of left. 
        protected int frameHeight = 126; // Y of bottom minus Y of top.

        // when robro takes damage he is invincible for a short period of time (from spikes & long falls not invisible/gets free bolt throws)
        protected int damageCounter;
        private const int invincibleMax = 50;
        private const int damageIndex = 3;

        private Game game;

        // Keep a counter, to count the number of ticks since the last change of animation frame.
        int animationCount; // How many ticks since the last frame change.
        int animationMax = 8; // How many ticks to change frame after. 
        bool landed = false; //So the update doesnt keep reseting robro to standard land position this bool is used to do that
        bool setDead = false; //This is so that i can set frameCountCol to the starting frame of death animation then not do it again in update animation function

        protected HealthMeter healthMeter;

        public Player(Game _game, Texture2D texture, ArrayList _sounds, int xStart, int yStart)
            : base(_game)
        {
            game = _game;

            facingDirection = Action.right;
            source = new Rectangle(this.frameStartX + this.frameSkipX * this.frameCountCol, this.frameStartY + this.frameSkipY * this.frameCountRow, this.frameWidth, this.frameHeight);
            int screenwidth = Game.GraphicsDevice.Viewport.Width;
            int screenheight = Game.GraphicsDevice.Viewport.Height;
            int playerHeight = screenheight * 4 / 32;
            int playerWidth = screenwidth * 2 / 64;
            position = new Rectangle(xStart, yStart, playerWidth, playerHeight);
            leftRightCollisionRectangle = new Rectangle(position.X, position.Y, position.Width, position.Height - 2);


            sprite = texture;

            int xCenter = xStart + (position.Width / 2);
            int yCenter = yStart - playerPadding;
            jumpMeter = new JumpMeter(game, xCenter, yCenter, spriteDepth);

            healthMeter = new HealthMeter(game, 200, 10, spriteDepth);

            hidden = 0.0f;
            damageCounter = 0;
            fallDistance = 0;
            isJumping = false;
            isStopped = true; // will we need to know this?? Maybe for a funny animation if you take too long...
            isDead = false;
            hasReachedGoal = false;
            displayHealthBar = true;

            sounds = _sounds;

            deltaX = 0;
            deltaY = 0;
        }

        public bool DoneLevel
        {
            get { return hasReachedGoal; }
        }

        /* Called when a bucket of bolts is gathered, calls the reset health method and resets player health to full */
        public void healDamage()
        {
            healthMeter.resetHealth();
        }

        public bool IsDead
        {
            get { return isDead; }
            set { isDead = value; }
        }

        /* Updates Robro's health when a bolt is thrown */
        public void throwBolt()
        {
            healthMeter.lowerHealthMeter();
        }

        public void Jump()
        {
            // scale the jump so you can go high fast, but fall a bit slower - less sudden
            deltaY = (int)(.9 * jumpMeter.JumpPower);
            if (jumpMeter.JumpPower > 0)
            {
                jumpMeter.drainJumpPower(0.25f);
                fallDistance = 0;
            }
            else
            {
                jumpMeter.drainJumpPower(0.2f);
                fallDistance -= deltaY;
            }

            isJumping = true;
        }

        public void ChargeJumpPower()
        {
            if (!isJumping)
            {
                jumpMeter.chargeJumpPower();
            }
        }

        public void Hide(IList<ITile> tiles)
        {
            // attempting to hide in the middle of a well lit room...
            hidden = 0.5f;

            foreach (ITile t in tiles)
            {
                if (t.getCollisionBehaviour() == CollisionType.hideable)
                {
                    if (t.isInCollision(this))
                    {
                        hidden = 1.0f;
                        spriteDepth = 0.25f;
                        // will also need to swap the texture source rectangle to the crouched sprite
                        
                    }
                }
            }

            frameCountCol = 4;
            source.X = frameStartX + frameSkipX * frameCountCol;
        }

        public void StopHiding()
        {
            hidden = 0.0f;
            spriteDepth = 0.8f;
            // will also need to swap the texture source rectangle to the standing sprite
            frameCountCol = 1;
            source.X = frameStartX + frameSkipX * frameCountCol;
        }

        /* make sure player isn't falling through platforms/walking through walls */
        public override void HandleCollision(IList<ITile> tiles)
        {
            IList<ITile> combinedTiles = joinTiles(tiles);

            // handle foot to floor collisions after intersection collisions have been resolved
            IList<ITile> tilesBelowPlayer = new List<ITile>();
            IList<ITile> tilesAtPlayerLevel = new List<ITile>();

            foreach (ITile t in tiles)
            {
                if (t.getPosition().Top < position.Bottom)
                {
                    tilesAtPlayerLevel.Add(t);
                }
            }

            foreach (ITile t in combinedTiles)
            {
                Rectangle tilePos = t.getPosition();
                Direction direction = determineCollisionType(tilePos);

                if (direction == Direction.bottom && t.getCollisionBehaviour() != CollisionType.hideable)
                {
                    if ((tilePos.Right <= position.Right && tilePos.Left >= position.Left) || (tilePos.Right == position.Right) || (tilePos.Left == position.Left))
                    {
                        position.Y = t.getPosition().Top - position.Height;
                    }
                }
            }

            foreach (ITile t in tiles)
            {
                Rectangle tilePos = t.getPosition();
                Direction direction = determineCollisionType(tilePos);

                if (direction == Direction.bottom && t.getCollisionBehaviour() != CollisionType.hideable)
                {
                    if ((tilePos.Right <= position.Right && tilePos.Left >= position.Left) || (tilePos.Right == position.Right) || (tilePos.Left == position.Left))
                    {
                        position.Y = t.getPosition().Top - position.Height;
                    }
                }
            }

            // handle merged tile collisions
            HandleIntersectionCollisions(combinedTiles);

            // handle any outliers that were not connected to a larger structure
            HandleIntersectionCollisions(tilesAtPlayerLevel);

            foreach (ITile t in tiles)
            {
                if (t.getPosition().Top >= position.Top && t.getCollisionBehaviour() != CollisionType.hideable)
                {
                    tilesBelowPlayer.Add(t);
                }
            }

            HandleFootCollisions(tilesBelowPlayer);
        }

        // join tiles that are side by side into larger rectangles to allow for smoother wall collisions
        // these tiles are not meant for foot collisions -> losing some of the collision types (only persisting platform vs. impassable)
        // where impassable will be a lump of all non-platform, non-hideable, non-passable tiles
        //
        // NOTE: this ONLY returns tiles that have merged up to larger tiles, anything that stayed as a single tile will still need to check collisions
        protected IList<ITile> joinTiles(IList<ITile> tiles)
        {
            IList<ITile> result = new List<ITile>();

            if (tiles.Count > 0)
            {
                int singleWidth = 0;
                int singleHeight = 0;

                // the tile layout size will be more like 5 x 4 - using 10 so there is a buffer
                ITile[,] tileLayout = new Tile[10, 10];

                // creating a dummy tile
                ITile dummyTile = new Tile(game);

                // initialize tileLayout to dummy values;
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        tileLayout[i, j] = dummyTile;
                    }
                }

                // the top-most y position of our tiles being sorted - -1 indicates no tiles yet
                int currY = -1;

                // start y index at -1 so the first y gets set properly
                int yIndex = -1;
                int xIndex = 0;

                // put the tiles into their appropriate row based on y value (smaller y is the top of the screen)
                foreach (ITile t in tiles)
                {
                    // set the width of a single tile
                    singleWidth = t.getPosition().Width;
                    singleHeight = t.getPosition().Height;

                    // new row, so insert the tile (deal with shifting the layout later) 
                    if (currY < 0 || currY < t.getPosition().Top)
                    {
                        xIndex = 0;
                        yIndex += 1;

                        tileLayout[yIndex, xIndex] = t;
                        currY = t.getPosition().Top;
                    }
                    // otherwise that tile belongs to this row, so it is the next tile in the row
                    else
                    {
                        xIndex += 1;

                        tileLayout[yIndex, xIndex] = t;
                    }
                }

                // adjust the tile layout to fix things being always pushed to the left i.e...
                // (0,0) (x,x)  ----> (0,0)(x,x)(x,x)
                // (1,1) (1,2)  ----> (x,x)(1,1)(1,2)
                for (int j = 0; j < 10; j++)
                {
                    int xMin = -1;
                    // get the min x in the current column
                    for (int i = 0; i < 10; i++)
                    {
                        if (xMin == -1 || (tileLayout[i, j].getPosition().X != -1 && xMin > tileLayout[i, j].getPosition().X))
                        {
                            xMin = tileLayout[i, j].getPosition().X;
                        }
                    }

                    // we only care about shifts when there was a valid minimum
                    if (xMin != -1)
                    {
                        // perform any shifts to the layout
                        for (int i = 0; i < 10; i++)
                        {
                            // this row needs to shift to the right
                            if (xMin < tileLayout[i, j].getPosition().X)
                            {
                                for (int k = 9; k > j; k--)
                                {
                                    tileLayout[i, k] = tileLayout[i, k - 1];
                                }

                                // set the start column of the row that just shifted to the dummy tile
                                tileLayout[i, j] = dummyTile;
                            }
                        }
                    }
                }

                ITile combined = new Tile(game);

                // do not merge platforms with any other tile-type -> these rectangles are not for foot collisions
                // combine tiles into taller tiles when they are touching each other
                for (int j = 0; j < 10; j++)
                {
                    // for each row until the end of the rows is hit
                    for (int i = 0; i < 10; i++)
                    {
                        // if the current tile is a valid tile
                        if (tileLayout[i, j].getPosition().X != -1)
                        {
                            // start a new tile
                            if (i == 0 || combined.getPosition().Bottom != tileLayout[i, j].getPosition().Top
                                       || !compatibleTypes(tileLayout[i, j].getCollisionBehaviour(), combined.getCollisionBehaviour()))
                            {
                                // add the merged tile to the result list
                                if (combined.getPosition().X != -1 && combined.getPosition().Height > singleHeight)
                                {
                                    result.Add(combined);
                                }

                                // build the next tile
                                combined = new Tile(game);
                                combined.setPosition(tileLayout[i, j].getPosition());
                                combined.changeType(tileLayout[i, j].getCollisionBehaviour());
                            }
                            else
                            {
                                //extend the rectangle's height
                                Rectangle r = combined.getPosition();
                                r.Height += tileLayout[i, j].getPosition().Height;

                                combined.setPosition(r);
                            }

                        }
                    }
                }

                // add the last combined tile to the result if there were any
                if (combined.getPosition().X != -1 && combined.getPosition().Height > singleHeight)
                {
                    result.Add(combined);
                }

                // clear out the merged tile
                combined = new Tile(game);
                // combine columns into rectangles when they are beside each other
                for (int i = 0; i < 10; i++)
                {
                    // for each row until the end of the rows is hit
                    for (int j = 0; j < 10; j++)
                    {
                        // if the current tile is a valid tile
                        if (tileLayout[i, j].getPosition().X != -1)
                        {
                            // start a new tile
                            if (j == 0 || combined.getPosition().Right != tileLayout[i, j].getPosition().Left
                                       || !compatibleTypes(tileLayout[i, j].getCollisionBehaviour(), combined.getCollisionBehaviour()))
                            {
                                // add the merged tile to the result list
                                if (combined.getPosition().X != -1 && combined.getPosition().Width > singleWidth)
                                {
                                    result.Add(combined);
                                }

                                // build the next tile
                                combined = new Tile(game);
                                combined.setPosition(tileLayout[i, j].getPosition());
                                combined.changeType(tileLayout[i, j].getCollisionBehaviour());
                            }
                            else
                            {
                                //extend the rectangle's height
                                Rectangle r = combined.getPosition();
                                r.Width += tileLayout[i, j].getPosition().Width;

                                combined.setPosition(r);
                            }

                        }
                    }
                }

                // add the last merged tile to the result set
                if (combined.getPosition().X != -1 && combined.getPosition().Width > singleWidth)
                {
                    result.Add(combined);
                }

                /*
                System.Console.WriteLine("------------merged-----------");
                foreach (ITile t in result)
                {
                    System.Console.WriteLine("(" + t.getPosition().X + ", "
                                                 + t.getPosition().Y + ", "
                                                 + t.getPosition().Width + ", "
                                                 + t.getPosition().Height + ") -> "
                                                 + t.getCollisionBehaviour().ToString());
                }
                */
                
            }
            
            return result;

        }

        protected bool compatibleTypes(CollisionType t1, CollisionType t2)
        {
            if (CollisionType.platform == t1 && CollisionType.platform == t2)
            {
                return true;
            }
            else if (CollisionType.hideable == t1 && CollisionType.hideable == t2)
            {
                return true;
            }
            else if (CollisionType.platform == t1 || CollisionType.platform == t2)
            {
                return false;
            }
            else if (CollisionType.hideable == t1 || CollisionType.hideable == t2)
            {
                return false;
            }
            else if (CollisionType.goal == t1 && CollisionType.goal == t2)
            {
                return true;
            }
            else if (CollisionType.goal == t1 || CollisionType.goal == t2)
            {
                return false;
            }
            else
            {
                // to get here neither t1 or t2 are hideable or platforms (i.e they are impassable on the left/right/top)
                return true;
            }
        }

        protected void TakeDamage()
        {
            if (damageCounter == 0)
            {
                healthMeter.lowerHealthMeter();
                ((SoundEffect)sounds[damageIndex]).Play();
                damageCounter = invincibleMax;
            }
        }

        protected void HandleIntersectionCollisions(IList<ITile> tiles)
        {
            // check that any intersections are only on passable tiles
            foreach (ITile t in tiles)
            {
                Rectangle tilePos = t.getPosition();

                Direction direction = determineCollisionType(tilePos);

                if (direction != Direction.none && t.getCollisionBehaviour() == CollisionType.goal)
                {
                    hasReachedGoal = true;
                }

                switch (direction)
                {
                    case Direction.top:
                        if (t.getCollisionBehaviour() != CollisionType.platform && t.getCollisionBehaviour() != CollisionType.hideable)
                        {
                            position.Y = t.getPosition().Bottom;
                            if (isJumping && startFalling < jumpMeter.JumpPower)
                            {
                                jumpMeter.JumpPower = startFalling;
                            }
                        }
                        break;
                    case Direction.left:
                        if (t.getCollisionBehaviour() == CollisionType.impassable || t.getCollisionBehaviour() == CollisionType.invisible ||
                            t.getCollisionBehaviour() == CollisionType.magnet || t.getCollisionBehaviour() == CollisionType.spike)
                        {
                            position.X = t.getPosition().Right;
                            deltaX = 0;
                        }
                        break;
                    case Direction.right:
                        if (t.getCollisionBehaviour() == CollisionType.impassable || t.getCollisionBehaviour() == CollisionType.invisible ||
                            t.getCollisionBehaviour() == CollisionType.magnet || t.getCollisionBehaviour() == CollisionType.spike)
                        {
                            position.X = t.getPosition().Left - position.Width;
                            deltaX = 0;
                        }
                        break;
                }
            }
        }

        protected void HandleFootCollisions(IList<ITile> tiles)
        {
            bool footCollision = false;
            bool isSlowed = false;

            foreach (ITile t in tiles)
            {
                Rectangle tilePosition = t.getPosition();
                tilePosition.Width -= 8;
                tilePosition.X += 4;

                Direction direction = determineCollisionType(tilePosition);

                if (Direction.bottom == direction)
                {
                    if (t.getCollisionBehaviour() == CollisionType.spike)
                    {
                        TakeDamage();
                    }

                    if (t.getCollisionBehaviour() == CollisionType.goal)
                    {
                        hasReachedGoal = true;
                    }

                    footCollision = true;

                    position.Y = t.getPosition().Top - position.Height;

                    // when the player's feet hit a magnet his speed decreases until off the magnet
                    if (CollisionType.magnet == t.getCollisionBehaviour())
                    {
                        if (Math.Abs((float)deltaX) > 1)
                        {
                            deltaX = deltaX / 2;
                        }
                        isSlowed = true;
                    }

                    if (isJumping)
                    {
                        isJumping = false;
                        landed = true;
                        jumpMeter.reset();

                        if (fallDistance > maxPainlessFall)
                        {
                            TakeDamage();
                        }
                        fallDistance = 0;
                    }
                }
            }

            if (isSlowed)
            {
                reposition();
            }

            // if the player is not jumping and has no tiles under their feet, they start falling
            if (!isJumping && !footCollision)
            {
                isJumping = true;
                jumpMeter.JumpPower = startFalling;
            }
        }

        public override Rectangle GetPosition()
        {
            /* do not want to give this player's rectangle to another class that might change it... */
            if (hidden > 0.0f)
            {
                // robro is shorter when he is crouching
                return new Rectangle(position.X, position.Y + crouchDiff, position.Width, position.Height - crouchDiff);
            }
            else
            {
                return new Rectangle(position.X, position.Y, position.Width, position.Height);
            }
        }

        public Action GetFacingDirection()
        {
            return facingDirection;
        }

        public void Kill()
        {
            isDead = true;
            ((SoundEffect)sounds[0]).Play();
        }

        public int DeltaX
        {
            get { return deltaX; }
        }

        public float HiddenPercent
        {
            get { return hidden; }
            set { hidden = value; }
        }

        public void reposition()
        {
            position.X -= deltaX;
        }

        public override void Update(Action direction, int velocity)
        {
            switch (direction)
            {
                case Action.right:
                    facingDirection = direction;
                    if (isJumping && deltaX <= 0)
                    {
                        deltaX = velocity / 2;
                    }
                    else if (!isJumping)
                    {
                        if (hidden <= 0)
                        {
                            deltaX = velocity;
                        }
                        else
                        {
                            deltaX = velocity / 2;
                        }
                    }
                    frameCountRow = 0;
                    animationCount += 1;

                    break;
                case Action.left:
                    facingDirection = direction;

                    if (isJumping && deltaX >= 0)
                    {
                        deltaX = velocity / 2;
                    }
                    else if (!isJumping)
                    {
                        if (hidden <= 0)
                        {
                            deltaX = velocity;
                        }
                        else
                        {
                            deltaX = velocity / 2;
                        }
                    }
                    this.frameCountRow = 1;
                    this.animationCount += 1;
                    break;

                case Action.none:
                    deltaX = 0;
                    break;
            }

            if (isJumping)
            {
                frameCountCol = 3;
                Jump();
            }

            if (landed)
            {
                landed = false;
                frameCountCol = 1;
            }

            healthMeter.Update(Action.none, 0);
            //if (healthMeter.Health <= 0) // If health is less than or equal to zero, Robro dies
            //{
            //    Kill();
            //}

            position.X += deltaX;
            position.Y -= deltaY;

            jumpMeter.setMeterPosition(position.X + (position.Width / 2), position.Y - playerPadding);
            jumpMeter.Update(Action.none, 0);

            if (damageCounter > 0)
            {
                damageCounter -= 1;
            }
            UpdateAnimation();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isDead)
            {
                spriteBatch.Draw(sprite, position, source, Color.Crimson, 0, new Vector2(0, 0), SpriteEffects.None, spriteDepth);
            }
            else if(damageCounter <= 0)
            {
                spriteBatch.Draw(sprite, position, source, Color.White * 1f, 0, new Vector2(0, 0), SpriteEffects.None, spriteDepth);
                jumpMeter.Draw(spriteBatch);
            }
            else if (damageCounter % 2 == 0)
            {
                spriteBatch.Draw(sprite, position, source, Color.White * 1f, 0, new Vector2(0, 0), SpriteEffects.None, spriteDepth);
                jumpMeter.Draw(spriteBatch);
            }

            if (displayHealthBar)
            {
                healthMeter.Draw(spriteBatch);
            }
        }

        public void ToggleHealthBar()
        {
            displayHealthBar = !displayHealthBar;
        }

        public void UpdateAnimation()
        {

            
            // if the count exceeds the animax time up the frame count
            // or if the player has died, because we dont have enough time to wait for full count divide animationMax by 2
            if (animationCount > animationMax)
            {
                animationCount = 0;
                if (hidden > 0 || isJumping)
                { }
                else
                {
                    frameCountCol += 1;
                }
            }

            if (isJumping)
            {
                frameCountCol = 3;
            }
            //once it gets to 3 reset it, however i have dying animation below so i need frame 3 if he dies
            else if (frameCountCol == 3 && !setDead)
            {
                frameCountCol = 0;
            }
            else { }
            source.X = frameStartX + frameSkipX * frameCountCol;
            source.Y = frameStartY + frameSkipY * frameCountRow;

            //If the player is below half health get to the damaged sprites
            if (healthMeter.Health <= 100)
            {
                //multiply by 2 in order to get to the first damaged row (right facing) in robro2.0
                if (frameCountRow == 0)
                {
                    source.Y = frameStartY + frameSkipY * 2;
                }
                //multiply by 3 in order to get to the 2nd damaged row (left facing) in robro2.0
                else
                {
                    source.Y = frameStartY + frameSkipY * 3;
                }

            }
            
            //When he dies time to crumble into a pile
            if (healthMeter.Health <= 0)
            {
                //Console.WriteLine("THE FUCK IS THIS SHIT");
                //Set to the row with the death animation by multiplying by 4
                source.Y = frameStartY + frameSkipY * 4;
                animationCount += 1;
                if (!setDead)
                {
                    frameCountCol = 0;
                    setDead = true;
                }
                //Kill him after it gets to 3 where his head is on his hand
                if (frameCountCol == 3)
                {
                    Kill();
                }


            }
        }
    }
}
