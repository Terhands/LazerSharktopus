using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame4
{
    interface ILevel : IGameObject
    {
        /* reset the level */
        void Reset();
    }
}
