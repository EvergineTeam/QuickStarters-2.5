using Match3.Services;
using Match3.UI.NinePatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace Match3.Components.Gameplay
{
    [DataContract]
    public class ScoreComponent : Component
    {
        private GameLogic gameLogic;
        private NinePatchSpriteAtlas scoreSlice;
        private Entity[] stars;
        private float scoreSliceMax;

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.gameLogic = CustomServices.GameLogic;
            if (this.gameLogic != null)
            {
                this.gameLogic.ScoreUpdated -= GameLogic_ScoreUpdated;
                this.gameLogic.ScoreUpdated += GameLogic_ScoreUpdated;

                this.stars = this.Owner.FindAllChildrenByTag("star").OrderBy(x => x.Name).ToArray();
                this.scoreSlice = this.Owner.FindChild("ScoreSlice").FindComponent<NinePatchSpriteAtlas>();
                this.scoreSliceMax = this.scoreSlice.Size.X;
                this.UpdateStarsPosition();
                this.UpdateCurrentScore();
            }
        }

        private void UpdateStarsPosition()
        {
            var starsTransforms = this.stars.Select(x => x.FindComponent<Transform2D>()).ToArray();
            var firstStar = starsTransforms.First().LocalPosition;
            var lastStar = starsTransforms.Last().LocalPosition;

            var starsSpace = lastStar.X - firstStar.X;
            for (int i = 0; i < this.gameLogic.StarsScores.Length - 1; i++)
            {
                var scoreRatio = this.gameLogic.StarsScores[i] / (float)this.gameLogic.StarsScores[this.gameLogic.StarsScores.Length - 1];
                var starPosition = starsTransforms[i].LocalPosition;
                starPosition.X = firstStar.X + (starsSpace * scoreRatio);
                starsTransforms[i].LocalPosition = starPosition;
            }
        }

        protected override void DeleteDependencies()
        {
            base.DeleteDependencies();
            if (this.gameLogic != null)
            {
                this.gameLogic.ScoreUpdated -= GameLogic_ScoreUpdated;
            }
        }

        private void GameLogic_ScoreUpdated(object sender, EventArgs e)
        {
            this.UpdateCurrentScore();
        }

        private void UpdateCurrentScore()
        {
            var scoreRatio = this.gameLogic.CurrentScore / (float)this.gameLogic.StarsScores[this.gameLogic.StarsScores.Length - 1];
            var scoreSlice = this.scoreSlice.Size;
            scoreSlice.X = scoreRatio * this.scoreSliceMax;
            this.scoreSlice.Size = scoreSlice;
        }
    }
}
