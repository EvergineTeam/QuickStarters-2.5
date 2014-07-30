using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewImpossibleGameProject.Enums
{
    public enum PlayerState
    {
        /// <summary>
        /// The initial state
        /// </summary>
        INITIAL,

        /// <summary>
        /// The player in the ground state
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

        /// <summary>
        /// The die state
        /// </summary>
        DIE,
    };
}
