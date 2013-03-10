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
        platform,
        hideable
    };

    public enum Action
    {
        none,
        left,
        right,
        up,
        down,
        jump,
        chargeJump,
        hide,
        distract
    }

    interface ITile : IGameObject
    {
        /* determine whether or not component is in collision with this tile */
        bool isInCollision(IDynamicGameObject obj);

        /* get the screen position of this tile */
        Rectangle getPosition();

        /* get the collision behaviour property for this tile */
        CollisionType getCollisionBehaviour();
    }
}
