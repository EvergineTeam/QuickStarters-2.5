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
        private Transform2D scoreSliceTransform;
        private NinePatchSpriteAtlas scoreSliceNinePatch;
        private Transform2D[] starsTransforms;
        private SpriteAtlas[] starsSpriteAtlas;

        private float currentScore;
        private float desiredScore;

        private float minScoreSliceX;
        private float maxScoreSliceX;
        private float currentScoreSliceX;
        private float desiredScoreSliceX;
        
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
                this.scoreSliceNinePatch = scoreSliceEntity.FindComponent<NinePatchSpriteAtlas>();
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

            this.minScoreSliceX = this.scoreSliceNinePatch.TextureSize.X;
            this.maxScoreSliceX = this.scoreSliceNinePatch.Size.X;
            this.currentScoreSliceX = this.desiredScoreSliceX = 0;
            this.UpdateStarsPosition();
            this.UpdateStarsTexture();
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
            var scoreSlice = this.scoreSliceNinePatch.Size;
            scoreSlice.X = Math.Max(this.minScoreSliceX, Math.Min(this.currentScoreSliceX, this.maxScoreSliceX));
            this.scoreSliceNinePatch.Size = scoreSlice;

            var xScale = Math.Min(1.0, this.currentScoreSliceX / this.minScoreSliceX);
            this.scoreSliceTransform.LocalXScale = (float)xScale;
        }

        private float ScoreSliceX(float score)
        {
            var scoreRatio = score / this.gameLogic.StarsScores[this.gameLogic.StarsScores.Length - 1];
            return scoreRatio * this.maxScoreSliceX;
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (this.desiredScoreSliceX != this.currentScoreSliceX)
            {
                this.currentScore = MathHelper.SmoothStep(this.currentScore, this.desiredScore, 0.1f);
                this.currentScoreSliceX = MathHelper.SmoothStep(this.currentScoreSliceX, this.desiredScoreSliceX, 0.1f);
                this.UpdateSliceSize();
                this.UpdateStarsTexture();
            }
        }
    }
}
