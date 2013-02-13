using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WindowsGame4
{
    interface IDynamicGameObject : IGameObject
    {
        /* procedure to handle two game objects that collide (primarily for object/tile collisions) */
        void HandleCollision(IList<ITile> obj);

        /* procedure to get the current position of this dynamic game object */
        Rectangle GetPosition();
    }
}
