using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;

namespace SuperSlingshot.Drawables
{
    public class Trajectory2DDrawable : Drawable2D
    {
        private const int NUMBER_OF_POINTS = 70;

        [RequiredComponent]
        protected MaterialsMap materialsMap;

        [RequiredComponent]
        protected Transform2D transform2D;

        [RequiredComponent]
        protected RigidBody2D rigidBody2D;

        private Vector2[] curve;

        private IEasingFunction ease;

        private VertexPositionColorTexture[] vertices;
        private Mesh mesh;
        private Vector2 point;
        private Vector2 nextPoint;
        private Vector2 unitaryPerpendicularVector;
        private int vertexIndex;
        private Platform platform;

        private float visibilityPercentage;
        private int visibilityLength;
        private float opacityStep;

        [DataMember]
        public float CurveWith
        {
            get;
            set;
        }

        [DataMember]
        public Vector2 DesiredVelocity
        {
            get;
            set;
        }

        [DataMember]
        public Color DiffuseColor
        {
            get;
            set;
        }

        [DataMember]
        public float VisibilityPercentage
        {
            get { return this.visibilityPercentage; }

            set
            {
                this.visibilityPercentage = value;
                this.visibilityLength = (int)(NUMBER_OF_POINTS * value);
                this.opacityStep = 1f / this.visibilityLength;
            }
        }

        #region Overridden Methods
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.VisibilityPercentage = 1f;
            this.CurveWith = 3.5f;
            this.ease = new CubicEase();
            this.DiffuseColor = Color.White;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.curve = new Vector2[NUMBER_OF_POINTS];
            this.platform = WaveServices.Platform;
        }

        protected override void Dispose(bool disposing)
        {
            if (this.mesh != null)
            {
                this.GraphicsDevice.DestroyIndexBuffer(this.mesh.IndexBuffer);
                this.GraphicsDevice.DestroyVertexBuffer(this.mesh.VertexBuffer);
            }
        }

        public override void Draw(TimeSpan gameTime)
        {
            this.UpdateCurvePoints();
            this.SetUpVertexBuffer();

            this.vertexIndex = 0;

            var curveLastIndex = this.curve.Length - 1;

            for (int i = 0; i < curveLastIndex; i++)
            {
                this.point = this.curve[i];
                this.nextPoint = this.curve[i + 1];
                this.SetUpPointsAndVertices(i);
            }

            this.point = this.curve[curveLastIndex - 1];
            this.nextPoint = this.curve[curveLastIndex];
            this.SetUpPointsAndVertices(curveLastIndex, lastPoint: true);

            this.UpdateGraphicDeviceAndDraw();
        }
        #endregion

        #region Private Methods
        private void SetUpVertexBuffer()
        {
            if (this.mesh == null)
            {
                // Per each point we have 2 vertices
                int verticesNumber = 2 * this.curve.Length;

                this.vertices = new VertexPositionColorTexture[verticesNumber];
                DynamicVertexBuffer vertexBuffer = new DynamicVertexBuffer(VertexPositionColorTexture.VertexFormat);
                ushort[] indices = new ushort[verticesNumber];

                for (ushort i = 0; i < indices.Length; i++)
                {
                    indices[i] = i;
                }

                var indexBuffer = new IndexBuffer(indices);

                this.mesh = new Mesh(
                    0,
                    verticesNumber,
                    0,
                    verticesNumber - 2,
                    vertexBuffer,
                    indexBuffer,
                    PrimitiveType.TriangleStrip);
            }
        }

        protected override void DrawDebugLines()
        {
            base.DrawDebugLines();

            for (int i = 0; i < this.curve.Length - 1; i++)
            {
                this.RenderManager.LineBatch2D.DrawLine(this.curve[i], this.curve[i + 1], Color.Red, 0f);
            }
        }

        private void SetUpPointsAndVertices(int i, bool lastPoint = false)
        {
            this.SetUpPoints();

            Vector2 goodPoint = lastPoint ? this.nextPoint : this.point;

            Color pointColor;

            if (i < this.visibilityLength)
            {
                var amount = 1f - (this.opacityStep * i);
                amount = (float)this.ease.Ease(amount);

                pointColor = this.DiffuseColor * amount;
            }
            else
            {
                pointColor = Color.Transparent;
            }

            float opacity = this.RenderManager.DebugLines ? DebugAlpha : this.transform2D.GlobalOpacity;
            pointColor *= opacity;

            this.SetUpVertex(i, ref goodPoint, ref pointColor);
            this.SetUpVertex(i, ref goodPoint, ref pointColor);
        }

        private void SetUpPoints()
        {
            var vectorBetweenPoints = this.nextPoint - this.point;
            var perpendicularVector = new Vector2(-vectorBetweenPoints.Y, vectorBetweenPoints.X);
            this.unitaryPerpendicularVector = Vector2.Normalize(perpendicularVector);
        }

        private void SetUpVertex(int i, ref Vector2 point, ref Color color)
        {
            float x, y;
            Vector2 texCoord;

            if ((this.vertexIndex % 2) == 0)
            {
                x = point.X + this.unitaryPerpendicularVector.X * this.CurveWith;
                y = point.Y + this.unitaryPerpendicularVector.Y * this.CurveWith;
                texCoord = (i % 2) == 0 ? Vector2.Zero : Vector2.UnitY;
            }
            else
            {
                x = point.X - this.unitaryPerpendicularVector.X * this.CurveWith;
                y = point.Y - this.unitaryPerpendicularVector.Y * this.CurveWith;
                texCoord = (i % 2) == 0 ? Vector2.UnitX : Vector2.One;
            }

            if (this.RenderManager.ActiveRenderTarget != null && this.platform.AdapterType != AdapterType.DirectX)
            {
                this.vertices[this.vertexIndex].Position = new Vector3(x, platform.ScreenHeight - y, this.transform2D.DrawOrder);
            }
            else
            {
                this.vertices[this.vertexIndex].Position = new Vector3(x, y, this.transform2D.DrawOrder);
            }

            this.vertices[this.vertexIndex].TexCoord = texCoord;
            this.vertices[this.vertexIndex].Color = color;

            this.vertexIndex++;
        }

        private void UpdateGraphicDeviceAndDraw()
        {
            this.mesh.ZOrder = this.transform2D.DrawOrder + 0.1f;
            this.mesh.VertexBuffer.SetData(this.vertices, this.vertices.Length);
            this.GraphicsDevice.BindVertexBuffer(this.mesh.VertexBuffer);

            // WORKAROUND: Force spritebatch to start a new render command
            this.layer.SpriteBatch.Draw(StaticResources.WhitePixel, -Vector2.One, null, Color.White, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, this.mesh.ZOrder);

            this.RenderManager.DrawMesh(this.mesh, this.materialsMap.DefaultMaterial, Matrix.Identity);
        }

        /// <summary>
        /// Update the curve points from the control points
        /// </summary>
        private void UpdateCurvePoints()
        {
            for (var i = 0; i < this.curve.Length; i++)
            {
                this.curve[i] = this.GetTrajectoryPoint(i * 2);
            }
        }

        private Vector2 GetTrajectoryPoint(float n)
        {
            const float physicConversionFactor = 100f;

            var startingPosition = this.transform2D.Position / physicConversionFactor;
            var startingVelocity = this.DesiredVelocity;

            //velocity and gravity are given per second but we want time step values here
            float t = 0.0166f; // seconds per time step (at 60fps)

            Vector2 stepVelocity = t * startingVelocity; // m/s
            Vector2 stepGravity = t * t * this.Owner.Scene.PhysicsManager.Simulation2D.Gravity; // m/s/s

            if (this.rigidBody2D.LinearDamping > 0)
            {
                float d = MathHelper.Clamp(1.0f - t * this.rigidBody2D.LinearDamping, 0.0f, 1.0f);
                float vd = 0;
                float ad = 0;
                for (int i = 0; i < n; i++)
                {
                    float p = (float)Math.Pow(d, i + 1);
                    vd += p;
                    ad += (n - i) * p;
                }

                return (startingPosition + vd * stepVelocity + ad * stepGravity) * physicConversionFactor;
            }
            else
            {
                return (startingPosition + (n * stepVelocity) + 0.5f * (n * n + n) * stepGravity) * physicConversionFactor;
            }
        }
        #endregion
    }
}
