using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;

namespace NewImpossibleGame.Models
{
    /// <summary>
    /// Block position
    /// </summary>
    public struct BlockPathPosition
    {
        /// <summary>
        /// Block Path (WaveEngine block name)
        /// </summary>
        public string Path;

        /// <summary>
        /// Block position
        /// </summary>
        public float ZPosition;
    }
}
