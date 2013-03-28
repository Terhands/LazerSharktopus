using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame4
{
    class GuardFactory
    {
        public GuardFactory()
        {

        }

        public IGuard createGuard(Game game, Texture2D texture, int xStart, int yStart, Direction FacingDirectionStart, int patrolLength, int guardType)
        {
            IGuard guard = null;

            if (guardType == 0)
            {
                guard = new Guard(game,texture, xStart, yStart, FacingDirectionStart, patrolLength);
            }
            else if (guardType == 1)
            {
                guard = new Soldier(game, texture, xStart, yStart, FacingDirectionStart, patrolLength);
            }

            return guard;
        }

    }
}
