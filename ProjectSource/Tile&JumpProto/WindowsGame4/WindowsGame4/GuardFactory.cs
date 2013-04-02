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
        Texture2D[] guardTextures;

        public GuardFactory(Texture2D wizard, Texture2D soldier, Texture2D LOS)
        {
            guardTextures = new Texture2D[3];

            guardTextures[0] = wizard;
            guardTextures[1] = soldier;
            guardTextures[2] = LOS;
        }

        public IGuard createGuard(Game game, int xStart, int yStart, Direction FacingDirectionStart, int patrolLength, int guardType)
        {
            IGuard guard = null;

            if (guardType == 0)
            {
                guard = new Wizard(game, guardTextures[guardType], guardTextures[2], xStart, yStart, FacingDirectionStart, patrolLength);
            }
            else if (guardType == 1)
            {
                guard = new Soldier(game, guardTextures[guardType], guardTextures[2], xStart, yStart, FacingDirectionStart, patrolLength);
            }

            return guard;
        }

    }
}
