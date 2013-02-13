using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame4
{
    public enum CollisionType
    {
        impassable,
        passable,
        platform
    };

    public enum Direction
    {
        left,
        right,
        up,
        down
    }

    interface ITile : IGameObject
    {
        /* determine whether or not component is in collision with this tile */
        bool isInCollision(GameComponent component);
    }
}
