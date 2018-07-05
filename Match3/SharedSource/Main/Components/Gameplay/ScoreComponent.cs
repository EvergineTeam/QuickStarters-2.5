using Match3.Services;
using System;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace Match3.Components.Gameplay
{
    [DataContract]
    public class ScoreComponent : Behavior
    {
        private GameLogic gameLogic;
        private Transform2D scoreSliceTransform;
        private Transform2D[] starsTransforms;
        private SpriteAtlas[] starsSpriteAtlas;

        private float currentScore;
        private float desiredScore;

        private float maxScoreSliceXScale;
        private float currentScoreSliceXScale;
        private float desiredScoreSliceXScale;

        public float CurrentScore
        {
            get
            {
                return (ulong)this.desiredScore;
            }
            set
            {
                this.desiredScore = value;
                this.desiredScoreSliceXScale = this.ScoreSliceX(this.desiredScore);
            }
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.gameLogic = CustomServices.GameLogic;
            if (this.gameLogic != null)
            {
                var stars = this.Owner.FindChildrenByTag("star", true).OrderBy(x => x.Name).ToArray();
                this.starsTransforms = stars.Select(x => x.FindComponent<Transform2D>()).ToArray();
                this.starsSpriteAtlas = stars.Select(x => x.FindComponent<SpriteAtlas>()).ToArray();

                var scoreSliceEntity = this.Owner.FindChild("ScoreSlice");
                this.scoreSliceTransform = scoreSliceEntity.FindComponent<Transform2D>();
            }
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.Owner.EntityInitialized += this.Owner_EntityInitialized;
        }

        private void Owner_EntityInitialized(object sender, EventArgs e)
        {
            this.Owner.EntityInitialized -= this.Owner_EntityInitialized;

            this.maxScoreSliceXScale = this.scoreSliceTransform.XScale;
            this.currentScoreSliceXScale = this.desiredScoreSliceXScale = 0;
            this.UpdateStarsPosition();
            this.UpdateStarsTexture();
            this.scoreSliceTransform.XScale = 1;
        }

        private void UpdateStarsPosition()
        {
            var firstStar = this.starsTransforms.First().LocalPosition;
            var lastStar = this.starsTransforms.Last().LocalPosition;

            var starsSpace = lastStar.X - firstStar.X;
            if (this.gameLogic.StarsScores != null)
            {
                for (int i = 0; i < this.gameLogic.StarsScores.Length - 1; i++)
                {
                    var scoreRatio = this.gameLogic.StarsScores[i] / (float)this.gameLogic.StarsScores[this.gameLogic.StarsScores.Length - 1];
                    var starPosition = this.starsTransforms[i].LocalPosition;
                    starPosition.X = firstStar.X + (starsSpace * scoreRatio);
                    this.starsTransforms[i].LocalPosition = starPosition;
                }
            }
        }

        private void UpdateStarsTexture()
        {
            var unlockedStars = CustomServices.GameLogic.UnlockedStartsByScore((ulong)this.currentScore);

            for (int i = 0; i < this.starsSpriteAtlas.Length; i++)
            {
                if (i < unlockedStars)
                {
                    this.starsSpriteAtlas[i].TextureName = WaveContent.Assets.GUI.Panels_spritesheet_TextureName.StarMediumColor;
                }
                else
                {
                    this.starsSpriteAtlas[i].TextureName = WaveContent.Assets.GUI.Panels_spritesheet_TextureName.StarMediumGray;
                }
            }
        }

        private void UpdateSliceSize()
        {
            var xScale = Math.Max(1, Math.Min(this.currentScoreSliceXScale, this.maxScoreSliceXScale));
            this.scoreSliceTransform.XScale = xScale;
        }

        private float ScoreSliceX(float score)
        {
            var scoreRatio = score / this.gameLogic.StarsScores[this.gameLogic.StarsScores.Length - 1];
            return (scoreRatio * this.maxScoreSliceXScale) + 1;
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (this.desiredScoreSliceXScale != this.currentScoreSliceXScale)
            {
                this.currentScore = MathHelper.SmoothStep(this.currentScore, this.desiredScore, 0.1f);
                this.currentScoreSliceXScale = MathHelper.SmoothStep(this.currentScoreSliceXScale, this.desiredScoreSliceXScale, 0.1f);
                this.UpdateSliceSize();
                this.UpdateStarsTexture();
            }
        }
    }
}
