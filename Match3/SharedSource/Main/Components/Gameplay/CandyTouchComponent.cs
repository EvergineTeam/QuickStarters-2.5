using Match3.Gameboard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Components.Gestures;
using WaveEngine.Framework;

namespace Match3.Components.Gameplay
{
    [DataContract]
    public class CandyTouchComponent : Component, IDisposable
    {
        private const int MinimumDisplacement = 10;
        
        [RequiredComponent]
        protected TouchGestures touchGestures;

        private Vector2 initialTouchPosition;

        public Coordinate Coordinate { get; set; }

        public event EventHandler<CandyMoves> OnMoveOperation;

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.touchGestures.TouchPressed -= this.TouchGestures_TouchPressed;
            this.touchGestures.TouchReleased -= this.TouchGestures_TouchReleased;

            this.touchGestures.TouchPressed += this.TouchGestures_TouchPressed;
            this.touchGestures.TouchReleased += this.TouchGestures_TouchReleased;
        }

        protected override void DeleteDependencies()
        {
            this.DeleteInternalDependencies();

            base.DeleteDependencies();
        }

        public void Dispose()
        {
            this.DeleteInternalDependencies();
        }

        private void DeleteInternalDependencies()
        {
            if (this.touchGestures != null)
            {
                this.touchGestures.TouchPressed -= this.TouchGestures_TouchPressed;
                this.touchGestures.TouchReleased -= this.TouchGestures_TouchReleased;
            }
        }

        private void TouchGestures_TouchPressed(object sender, GestureEventArgs e)
        {
            this.initialTouchPosition = e.GestureSample.Position;
        }


        private void TouchGestures_TouchReleased(object sender, GestureEventArgs e)
        {
            var diffPosition = e.GestureSample.Position - this.initialTouchPosition;

            CandyMoves? detectedMove = null;

            if (Math.Abs(diffPosition.X) > Math.Abs(diffPosition.Y))
            {
                if (diffPosition.X > MinimumDisplacement)
                {
                    detectedMove = CandyMoves.Right;
                }
                else if (diffPosition.X < -MinimumDisplacement)
                {
                    detectedMove = CandyMoves.Left;
                }
            }
            else if (diffPosition.Y > MinimumDisplacement)
            {
                detectedMove = CandyMoves.Bottom;
            }
            else if (diffPosition.Y < -MinimumDisplacement)
            {
                detectedMove = CandyMoves.Top;
            }

            if(detectedMove.HasValue)
            {
                Debug.WriteLine("Move [{0},{1}] {2}!", this.Coordinate.X, this.Coordinate.Y, detectedMove.Value);

                this.OnMoveOperation?.Invoke(this, detectedMove.Value);
            }
        }
    }
}
