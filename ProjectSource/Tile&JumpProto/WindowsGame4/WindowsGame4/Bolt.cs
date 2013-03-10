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

        public Bolt(Game game, Direction direction, int xStart, int yStart) : base(game)
        {
            position = new Rectangle(xStart, yStart, 3, 3);

            boltTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            boltTexture.SetData(new Color[] { Color.White });
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
        }

        public override void  Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(
        }

    }
}
