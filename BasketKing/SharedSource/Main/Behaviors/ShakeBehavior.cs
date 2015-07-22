#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace BasketKing.Behaviors
{
    public class ShakeBehavior : Behavior
    {
        [RequiredComponent]
        private Transform2D transform2D;

        private const float INITIAL_AMPLITUDE = 20;

        private float count;
        private float amplitude;
        private bool lastSign;

        private Vector2 initialPosition;
        private float initialRotation;

        public ShakeBehavior()
        {
            this.IsActive = false;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.initialPosition = new Vector2(this.transform2D.X, this.transform2D.Y);
            this.initialRotation = this.transform2D.Rotation;
        }

        protected override void Update(TimeSpan gameTime)
        {
            this.count += 1.2f * 60 * (float)gameTime.TotalSeconds;

            var sin = (float)Math.Sin(this.count);

            // Decrement amplitude on sign change
            if (this.lastSign && sin > 0)
            {
                this.lastSign = false;
                this.amplitude -= 2;
            }
            else if (!this.lastSign && sin < 0)
            {
                this.lastSign = true;
                this.amplitude -= 2;
            }

            // Update transform
            this.transform2D.X = this.initialPosition.X + sin * this.amplitude;            

            if (this.transform2D.XScale > 1)
            {
                this.transform2D.XScale -= 0.1f;
                this.transform2D.YScale -= 0.1f;
            }

            // Effect ending detection
            if (this.amplitude <= 0)
            {
                this.IsActive = false;
            }
        }

        public void DoEffect()
        {
            this.transform2D.XScale = 1.5f;
            this.transform2D.YScale = 1.5f;
            this.amplitude = INITIAL_AMPLITUDE;
            this.count = 0;
            this.IsActive = true;
        }
    }
}
