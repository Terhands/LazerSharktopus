using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame4
{
    interface IGameObject
    {
        /* update the offset of this game object */
        void Update(Action action, int velocity);

        /* draw this game object to the screen */
        void Draw(SpriteBatch spriteBatch);
    }
}
