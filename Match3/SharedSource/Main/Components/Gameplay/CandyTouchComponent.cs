using Match3.Gameboard;
using Match3.Helpers;
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Math;
using WaveEngine.Components.Gestures;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace Match3.Components.Gameplay
{
    [DataContract]
    public class CandyTouchComponent : Component
    {
        private const int MinimumDisplacement = (int)(GameboardContent.DistanceBtwCandies * 0.66);

        [RequiredComponent]
        protected Transform2D transform2D;

        [RequiredComponent]
        protected TouchGestures touchGestures;

        [RequiredComponent]
        protected CandyAnimationBehavior animator;

        protected GameboardAnimationsOrchestrator gameboardOrchestrator;

        private Vector2 initialCandyPosition;

        private Vector2 initialTouchPosition;

        private Vector2 lastTouchPosition;
        
        private CandyMoves? detectedMove;

        public bool IsPressed { get; private set; }

        public event EventHandler<CandyMoves> OnMoveOperation;

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.gameboardOrchestrator = this.Owner.Parent.FindComponent<GameboardAnimationsOrchestrator>();

            this.touchGestures.TouchPressed += this.TouchGestures_TouchPressed;
            this.touchGestures.TouchReleased += this.TouchGestures_TouchReleased;
            this.touchGestures.TouchMoved += this.TouchGestures_TouchMoved;
        }

        protected override void DeleteDependencies()
        {
            if (this.touchGestures != null)
            {
                this.touchGestures.TouchPressed -= this.TouchGestures_TouchPressed;
                this.touchGestures.TouchReleased -= this.TouchGestures_TouchReleased;
                this.touchGestures.TouchMoved -= this.TouchGestures_TouchMoved;
            }

            base.DeleteDependencies();
        }

        private void TouchGestures_TouchPressed(object sender, GestureEventArgs e)
        {
            if (!this.gameboardOrchestrator.IsAnimationInProgress)
            {
                this.IsPressed = true;
                this.initialTouchPosition = e.GestureSample.Position;
                this.lastTouchPosition = this.initialTouchPosition;
                this.initialCandyPosition = this.transform2D.LocalPosition;

                this.UpdateCandyPosition(e.GestureSample.Position);
            }
        }

        private void TouchGestures_TouchReleased(object sender, GestureEventArgs e)
        {
            if (this.IsPressed)
            {
                if (this.detectedMove.HasValue)
                {
                    this.OnMoveOperation?.Invoke(this, this.detectedMove.Value);
                    this.detectedMove = null;
                }
                else
                {
                    this.animator.RefreshPositionAnimation();
                }

                this.UpdateCandyPosition(e.GestureSample.Position);

                this.IsPressed = false;
            }
        }

        private void TouchGestures_TouchMoved(object sender, GestureEventArgs e)
        {
            if (this.IsPressed)
            {
                this.UpdateCandyPosition(e.GestureSample.Position);
            }
        }

        private void UpdateCandyPosition(Vector2 newTouchPosition)
        {
            var candyPosition = this.initialCandyPosition;
            var diffTouchPosition = newTouchPosition - this.initialTouchPosition;

            this.detectedMove = null;
            if (Math.Abs(diffTouchPosition.X) > Math.Abs(diffTouchPosition.Y))
            {
                candyPosition.X += Math.Max(-GameboardContent.DistanceBtwCandies, Math.Min(GameboardContent.DistanceBtwCandies, diffTouchPosition.X));

                if (diffTouchPosition.X > MinimumDisplacement)
                {
                    this.detectedMove = CandyMoves.Right;
                }
                else if (diffTouchPosition.X < -MinimumDisplacement)
                {
                    this.detectedMove = CandyMoves.Left;
                }
            }
            else
            {
                candyPosition.Y += Math.Max(-GameboardContent.DistanceBtwCandies, Math.Min(GameboardContent.DistanceBtwCandies, diffTouchPosition.Y));

                if (diffTouchPosition.Y > MinimumDisplacement)
                {
                    this.detectedMove = CandyMoves.Bottom;
                }
                else if (diffTouchPosition.Y < -MinimumDisplacement)
                {
                    this.detectedMove = CandyMoves.Top;
                }
            }

            this.transform2D.LocalPosition = candyPosition;
        }
    }
}
