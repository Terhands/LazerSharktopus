using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame4
{
    class Bolt : ADynamicGameObject, IGameObject
    {
        Texture2D boltTexture;
        Rectangle position;

        float deltaX = 3;
        float deltaY = 1;
        float k_looks_gravity = -0.15f;

        public Bolt(Game game, Direction direction, int xStart, int yStart, Texture2D texture) : base(game)
        {
            position = new Rectangle(xStart, yStart, 15, 15);

            boltTexture = texture;
        }

        public override void HandleCollision(IList<ITile> obj)
        {
            throw new NotImplementedException();
        }

        public override Rectangle GetPosition()
        {
            return position;
        }

        public override void Update(Action action, int velocity)
        {
            position.X += (int)deltaX;
            position.Y += (int)deltaY;

            deltaY -= k_looks_gravity;
            Console.WriteLine("Bolt is at {0}, {1}", position.X, position.Y);
        }

        public override void  Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(boltTexture, new Rectangle((int)position.X, (int)position.Y, 15, 15), Color.White);
        }

    }
}
