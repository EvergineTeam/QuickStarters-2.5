#region File Description
//-----------------------------------------------------------------------------
// Flying Kite
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using FlyingKite.Behaviors;
using WaveEngine.Framework;
#endregion

namespace FlyingKite.Scenes
{
    public class GameScene : Scene
    {        
        /// <summary>
        /// Creates the scene.
        /// </summary>
        /// <remarks>
        /// This method is called before all <see cref="T:WaveEngine.Framework.Entity" /> instances in this instance are initialized.
        /// </remarks>
        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.GameScene);
            
            // UI
            var currentScoreTB = EntitiesFactory.CreateCurrentScore(40, (int)this.VirtualScreenManager.VirtualWidth);
            this.EntityManager.Add(currentScoreTB);

#if DEBUG
            this.AddSceneBehavior(new DebugSceneBehavior(), SceneBehavior.Order.PreUpdate);
#endif
        }
    }
}
