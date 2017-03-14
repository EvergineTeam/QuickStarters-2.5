using Match3.Services;
using Match3.UI.NinePatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Components.GameActions;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace Match3.Components.Gameplay
{
    [DataContract]
    public class ScoreComponent : Component, IDisposable
    {
        private GameLogic gameLogic;
        private NinePatchSpriteAtlas scoreSlice;
        private Entity[] stars;
        private float scoreSliceMax;

        private IGameAction updateScoreGameAction;

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
                this.UpdateCurrentScore(this.gameLogic.CurrentScore);
            }
        }

        public void Dispose()
        {
            if (this.gameLogic != null)
            {
                this.gameLogic.ScoreUpdated -= GameLogic_ScoreUpdated;
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

        private void GameLogic_ScoreUpdated(object sender, ulong currentScore)
        {
            if(updateScoreGameAction == null)
            {
                updateScoreGameAction = this.Owner.Scene.CreateEmptyGameAction();
            }

            updateScoreGameAction = updateScoreGameAction
                .ContinueWith(this.Owner.Scene.CreateGameAction(() => 
                {
                    var scoreRatio = currentScore / (float)this.gameLogic.StarsScores[this.gameLogic.StarsScores.Length - 1];
                    return new FloatAnimationGameAction(this.Owner, this.scoreSlice.Size.X, scoreRatio * this.scoreSliceMax, TimeSpan.FromSeconds(1), EaseFunction.QuarticInOutEase, x =>
                    {
                        var scoreSlice = this.scoreSlice.Size;
                        scoreSlice.X = x;
                        this.scoreSlice.Size = scoreSlice;
                    });
                }));

            updateScoreGameAction.Run();
        }

        private void UpdateCurrentScore(ulong score)
        {
            var scoreRatio = score / (float)this.gameLogic.StarsScores[this.gameLogic.StarsScores.Length - 1];
            var scoreSlice = this.scoreSlice.Size;
            scoreSlice.X = scoreRatio * this.scoreSliceMax;
            this.scoreSlice.Size = scoreSlice;
        }
    }
}
