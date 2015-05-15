#region Using Statements
using System;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services; 
#endregion

namespace MangomacoProject.Components
{
    /// <summary>
    /// Camera Behavior class
    /// </summary>
    public class CameraBehavior : Behavior, IDisposable
    {
        private RectangleF limitRectangle;
        private float minCameraX, maxCameraX;
        private float minCameraY, maxCameraY;

        private const float CameraSpeed = 4;

        private ViewportManager viewportManager;
        private Platform platform;

        [RequiredComponent]
        private Camera2D camera2D = null;

        private Entity followEntity;
        private Transform2D followTransform;

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraBehavior"/> class.
        /// </summary>
        /// <param name="followEntity">The follow entity.</param>
        /// <param name="limitRectangle">The limit rectangle.</param>
        public CameraBehavior(Entity followEntity, RectangleF limitRectangle)
        {
            this.followEntity = followEntity;
            this.limitRectangle = limitRectangle;
        }

        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        /// <remarks>
        /// By default this method does nothing.
        /// </remarks>
        protected override void Initialize()
        {
            base.Initialize();

            this.followTransform = this.followEntity.FindComponent<Transform2D>();

            this.viewportManager = WaveServices.ViewportManager;
            this.platform = WaveServices.Platform;

            this.platform.OnScreenSizeChanged += OnScreenSizeChanged;
            this.RefreshCameraLimits();

            Vector3 desiredPosition = this.GetDesiredPosition();

            this.camera2D.Position = desiredPosition;
        }

        /// <summary>
        /// Allows this instance to execute custom logic during its <c>Update</c>.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <remarks>
        /// This method will not be executed if the <see cref="T:WaveEngine.Framework.Component" />, or the <see cref="T:WaveEngine.Framework.Entity" />
        /// owning it are not <c>Active</c>.
        /// </remarks>
        protected override void Update(TimeSpan gameTime)
        {
            Vector3 currentPosition = this.camera2D.Position;
            Vector3 desiredPosition = this.GetDesiredPosition();

            this.camera2D.Position = currentPosition + (desiredPosition - currentPosition) * MathHelper.Min((float)(gameTime.TotalSeconds * CameraSpeed), 1);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.platform.OnScreenSizeChanged -= OnScreenSizeChanged;
        }

        /// <summary>
        /// Gets the desired position.
        /// </summary>
        /// <returns></returns>
        private Vector3 GetDesiredPosition()
        {
            Vector3 desiredPosition = this.camera2D.Position;

            desiredPosition.X = Math.Max(this.minCameraX, Math.Min(this.followTransform.X, this.maxCameraX));
            desiredPosition.Y = Math.Max(this.minCameraY, Math.Min(this.followTransform.Y, this.maxCameraY));

            return desiredPosition;
        }

        /// <summary>
        /// Called when [screen size changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="WaveEngine.Common.Helpers.SizeEventArgs"/> instance containing the event data.</param>
        private void OnScreenSizeChanged(object sender, WaveEngine.Common.Helpers.SizeEventArgs e)
        {
            this.RefreshCameraLimits();
        }

        /// <summary>
        /// Refreshes the camera limits.
        /// </summary>
        private void RefreshCameraLimits()
        {
            var vm = WaveServices.ViewportManager;

            float marginX = (vm.RightEdge - vm.LeftEdge) * this.camera2D.Zoom.X / 2;
            float marginY = (vm.BottomEdge - vm.TopEdge) * this.camera2D.Zoom.Y / 2;            

            this.minCameraX = this.limitRectangle.X + marginX;
            this.maxCameraX = this.limitRectangle.X + this.limitRectangle.Width - marginX;
            this.minCameraY = this.limitRectangle.Y + marginY;
            this.maxCameraY = this.limitRectangle.Y + this.limitRectangle.Height - marginY;
        }
    }
}
