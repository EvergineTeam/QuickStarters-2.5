using System;
using System.Collections.Generic;
using System.Text;

namespace NewImpossibleGame.Enums
{
    /// <summary>
    /// Player State enumeration
    /// </summary>
    public enum PlayerState
    {
        /// <summary>
        /// The initial state
        /// </summary>
        INITIAL,

        /// <summary>
        /// The player in the ground running state
        /// </summary>
        GROUND,

        /// <summary>
        /// The player jumping state
        /// </summary>
        JUMPING,

        /// <summary>
        /// The player falling state
        /// </summary>
        FALLING,
    }
}
