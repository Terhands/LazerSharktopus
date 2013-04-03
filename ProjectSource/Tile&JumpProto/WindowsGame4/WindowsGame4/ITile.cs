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
        passable,
        platform,
        impassable,
        hideable,
        goal,
        spike,
        magnet,
        invisible
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
        distract,
        throwBolt,
        boltUpdates
    }

    interface ITile : IGameObject
    {
        /* determine whether or not component is in collision with this tile */
        bool isInCollision(IDynamicGameObject obj);

        /* get the screen position of this tile */
        Rectangle getPosition();

        /* get the collision behaviour property for this tile */
        CollisionType getCollisionBehaviour();

        /* Change the collision type of this tile */
        void changeType(CollisionType newType);
    }
}