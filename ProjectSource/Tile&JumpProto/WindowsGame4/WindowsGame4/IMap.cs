using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame4
{
    interface IMap : IGameObject
    {
        /* reset the level */
        void Reset();

        /* determine if the screen has hit the left edge of the map */
        bool atLeftEdge();

        /* determine if the screen has hit the right edge of the map */
        bool atRightEdge();

        /* get the tiles that are either intersecting position on the screen */
        IList<ITile> GetNearbyTiles(Rectangle position);
    }
}
