#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common; 
#endregion

namespace MangomacoProject.Services
{
    /// <summary>
    /// Level Info Service
    /// </summary>
    public class LevelInfoService : Service
    {
        private List<LevelDefinition> availableLevels;

        /// <summary>
        /// Gets the available levels.
        /// </summary>
        public IEnumerable<LevelDefinition> AvailableLevels
        {
            get
            {
                return this.availableLevels;
            }
        }

        /// <summary>
        /// Allows to execute custom logic during the initialization of this instance.
        /// </summary>
        protected override void Initialize()
        {
            this.availableLevels = new List<LevelDefinition>()
                {
                    new LevelDefinition()
                    {
                        TileMapPath = "Content/Level1.tmx"
                    },

                    new LevelDefinition()
                    {
                        TileMapPath = "Content/Level2.tmx"
                    },
                };
        }

        /// <summary>
        /// Allow to execute custom logic during the finalization of this instance.
        /// </summary>
        protected override void Terminate()
        {
        }
    }
}
