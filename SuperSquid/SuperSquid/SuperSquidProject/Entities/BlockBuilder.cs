#region File Description
//-----------------------------------------------------------------------------
// Super Squid
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using SuperSquidProject.Entities.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
#endregion

namespace SuperSquidProject.Entities
{
    public class BlockBuilder : BaseDecorator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockBuilder" /> class.
        /// </summary>
        public BlockBuilder()
        {            
            this.entity = new Entity("BlockBuilder")
                .AddComponent(new BlockBuilderBehavior());
        }

        /// <summary>
        /// Resets all blocks.
        /// </summary>
        public void Reset()
        {
            this.entity.FindComponent<BlockBuilderBehavior>().Reset();
        }
    }
}
