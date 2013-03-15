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

        /* the player hides in the environment to avoid detection */
        void Hide(IList<ITile> tiles);

        /* the player stops trying to hide to avoid detection */
        void StopHiding();

        Action GetFacingDirection();
        bool DoneLevel { get; }

        /* getters & setters for player state */
        bool IsDead { get; set; }
        int DeltaX { get; }
        float HiddenPercent { get; }
        
        /* move the player back deltaX (for use when the screen shifts) */
        void reposition();

        void Kill();
    }
}
