using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame4
{
    class GameOver : Microsoft.Xna.Framework.GameComponent
    {
        SpriteFont timerFont;
        Texture2D background;

        GameLoop gameLoop;

        public GameOver(GameLoop game, Texture2D _background) : base(game)
        {
            background = _background;
            gameLoop = game;
        }

        public void Update()
        {
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Space))

                gameLoop.Exit();
            else if (keyState.IsKeyDown(Keys.Enter))
                gameLoop.State = GameLoop.States.level;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);
        }
    }
}