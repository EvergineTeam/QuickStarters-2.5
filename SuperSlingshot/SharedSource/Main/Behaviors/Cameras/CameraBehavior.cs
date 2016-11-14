#region Using Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Helpers;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Services;
#endregion

namespace SuperSlingshot.Behaviors
{
    [DataContract]
    public abstract class CameraBehavior : Behavior
    {
        private Platform platform;
        protected Entity TargetEntity;
        protected VirtualScreenManager virtualScreenManager;
        protected Vector2 pixelLimitMin;
        protected Vector2 pixelLimitMax;

        protected Camera2D camera2D;
        protected VirtualScreenManager vsm;

        [RequiredComponent]
        protected Transform2D transform = null;

        [DataMember]
        public float Speed
        {
            get;
            set;
        }

        [DataMember]
        public bool Follow
        {
            get;
            set;
        }

        [IgnoreDataMember]
        public Transform2D TargetTransform { get; private set; }

        [IgnoreDataMember]
        public Vector2 BoundsMin { get; private set; }

        [IgnoreDataMember]
        public Vector2 BoundsMax { get; private set; }

        public void SetLimits(Vector2 min, Vector2 max)
        {
            this.BoundsMin = min;
            this.BoundsMax = max;
            this.RefreshCameraLimits();

            this.transform.Scale = new Vector2(2.0f, 2.0f);
        }

        public void SetTarget(string entityPath)
        {
            if (string.IsNullOrEmpty(entityPath))
            {
                this.TargetEntity = null;
                this.TargetTransform = null;
                return;
            }

            if (this.EntityManager != null)
            {
                this.TargetEntity = this.EntityManager.Find(entityPath);
                if (this.TargetEntity != null)
                {
                    this.TargetTransform = this.TargetEntity.FindComponent<Transform2D>();
                }
            }
        }

        public void Dispose()
        {
            this.platform.OnScreenSizeChanged -= OnScreenSizeChanged;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.virtualScreenManager = this.Owner.Scene.VirtualScreenManager;
            this.platform = WaveServices.Platform;

            this.platform.OnScreenSizeChanged += this.OnScreenSizeChanged;
            this.RefreshCameraLimits();

            this.camera2D = this.RenderManager.ActiveCamera2D;
            this.vsm = camera2D.UsedVirtualScreen;
        }

        private void RefreshCameraLimits()
        {
            Vector2 halfScreenSize = new Vector2((this.virtualScreenManager.RightEdge - this.virtualScreenManager.LeftEdge) / 2,
                (this.virtualScreenManager.TopEdge - this.virtualScreenManager.BottomEdge) / 2);

            this.pixelLimitMax.X = this.BoundsMax.X - (halfScreenSize.X * this.transform.XScale);
            this.pixelLimitMax.Y = this.BoundsMax.Y + (halfScreenSize.Y * this.transform.YScale);
            this.pixelLimitMin.X = this.BoundsMin.X + (halfScreenSize.X * this.transform.XScale);
            this.pixelLimitMin.Y = this.BoundsMin.Y - (halfScreenSize.Y * this.transform.YScale);

            // when camera scale create an Y-axis inverse rectangle (this puts the bottom limit in the bottom camera edge)
            if(this.pixelLimitMin.Y > this.pixelLimitMax.Y)
            {
                this.pixelLimitMin.Y = this.pixelLimitMax.Y;
            }

            // when camera zoom creates an X-axis inverse rectangle (selected zoom is big enought to show all the scene)
            if(this.pixelLimitMin.X > this.pixelLimitMax.X)
            {
                var middle = (this.pixelLimitMin.X - this.pixelLimitMax.X) / 2;
                this.pixelLimitMin.X = middle;
                this.pixelLimitMax.X = middle;
            }

            this.ChildRefreshCamera();
        }

        protected override void Update(TimeSpan gameTime)
        {
            this.CameraUpdate(gameTime);
        }

        protected virtual void ChildRefreshCamera()
        {
            throw new NotImplementedException();
        }

        protected virtual void CameraUpdate(TimeSpan gameTime)
        {
            throw new NotImplementedException();
        }

        private void OnScreenSizeChanged(object sender, SizeEventArgs e)
        {
            this.RefreshCameraLimits();
        }
    }
}
