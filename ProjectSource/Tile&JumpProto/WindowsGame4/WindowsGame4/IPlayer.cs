using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame4
{
    interface IPlayer : IDynamicGameObject
    {
        /* the player jumps from the ground (user loses control until completion of the jump) */
        void Jump();

        /* the player charges up their jump meter for a higher jump */
        void ChargeJumpPower();

        /* the player throws a bolt object until it either hits the ground, or an impassable tile */
        void ThrowBolt();

        /* the player hides in the environment to avoid detection */
        void Hide();

        bool DoneLevel { get; }

        bool IsDead { get; }
    }
}
