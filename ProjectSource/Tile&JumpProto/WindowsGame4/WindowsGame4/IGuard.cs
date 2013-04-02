using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WindowsGame4
{
    interface IGuard : IDynamicGameObject
    {
        // procedure to set the guard into distracted mode when a bold is detected within hearing range
        void HandleHearing(IList<Bolt> bolts);

        // procedure to check if the guard can see the player - if so game over
        void HandleVision(IPlayer player, IList<ITile> surroundingTiles);

        // procedure to retrieve the bounding rectangle of the guard's line of sight cone
        Rectangle GetLOSRectangle();

        bool IsDead
        {
            get;
        }
    }
}
