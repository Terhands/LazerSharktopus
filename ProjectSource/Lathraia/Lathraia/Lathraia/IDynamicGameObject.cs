using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame4
{
    interface IDynamicGameObject : IGameObject
    {
        /* procedure to handle two game objects that collide (primarily for player/tile collisions) */
        void HandleCollision(IGameObject obj);
    }
}
