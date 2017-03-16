using Match3.Services;
using Match3.UI.NinePatch;
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
        private NinePatchSpriteAtlas scoreSlice;
        private Entity[] stars;
        private SpriteAtlas[] starsSprite;
        private float scoreSliceMax;

        private float currentScore;
        private float desiredScore;

        public float CurrentScore
        {
            get
            {
                return (ulong)this.desiredScore;
            }
            set
            {
                this.desiredScore = value;
                this.desiredScoreSliceX = this.ScoreSliceX(this.desiredScore);
            }
        }

        private float currentScoreSliceX;
        private float desiredScoreSliceX;

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.gameLogic = CustomServices.GameLogic;
            if (this.gameLogic != null)
            {
                this.stars = this.Owner.FindAllChildrenByTag("star").OrderBy(x => x.Name).ToArray();
                this.starsSprite = this.stars.Select(x => x.FindComponent<SpriteAtlas>()).ToArray();
                this.scoreSlice = this.Owner.FindChild("ScoreSlice").FindComponent<NinePatchSpriteAtlas>();
                this.scoreSliceMax = this.scoreSlice.Size.X;
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.currentScoreSliceX = this.desiredScoreSliceX = 0;
            this.UpdateSliceSize();
            this.UpdateStarsPosition();

            for (int i = 0; i < this.starsSprite.Length; i++)
            {
                this.starsSprite[i].TextureName = nameof(WaveContent.Assets.GUI.Panels_spritesheet_TextureName.StarMediumGray);
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

        private float ScoreSliceX(float score)
        {
            var scoreRatio = score / this.gameLogic.StarsScores[this.gameLogic.StarsScores.Length - 1];
            return scoreRatio * this.scoreSliceMax;
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (this.desiredScoreSliceX != this.currentScoreSliceX)
            {
                this.currentScore = MathHelper.SmoothStep(this.currentScore, this.desiredScore, 0.1f);
                this.currentScoreSliceX = MathHelper.SmoothStep(this.currentScoreSliceX, this.desiredScoreSliceX, 0.1f);
                this.UpdateSliceSize();

                for (int i = 0; i < this.starsSprite.Length; i++)
                {
                    if (i < gameLogic.StarsScores.Length && gameLogic.StarsScores[i] <= this.currentScore)
                    {
                        this.starsSprite[i].TextureName = nameof(WaveContent.Assets.GUI.Panels_spritesheet_TextureName.StarMediumColor);
                    }
                    else
                    {
                        this.starsSprite[i].TextureName = nameof(WaveContent.Assets.GUI.Panels_spritesheet_TextureName.StarMediumGray);
                    }
                }
            }
        }

        private void UpdateSliceSize()
        {
            var scoreSlice = this.scoreSlice.Size;
            scoreSlice.X = this.currentScoreSliceX;
            this.scoreSlice.Size = scoreSlice;
        }
    }
}
