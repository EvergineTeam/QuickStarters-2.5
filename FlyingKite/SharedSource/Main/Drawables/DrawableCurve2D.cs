#region File Description
//-----------------------------------------------------------------------------
// Flying Kite
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace FlyingKite.Drawables
{
    [DataContract]
    public class DrawableCurve2D : Drawable2D
    {
        [RequiredComponent]
        private MaterialsMap materialsMap = null;

        private Vector2[] curve;
        private VertexPositionColorTexture[] vertices;
        private Mesh mesh;
        private Vector2 point;
        private Vector2 nextPoint;
        private Vector2 unitaryPerpendicularVector;
        private int vertexIndex;
        private Platform platform;
        private Matrix identityTransform;

        [DontRenderProperty]
        public Vector2[] Curve
        {
            get
            {
                return this.curve;
            }

            set
            {
                if (value.Count() < 2)
                {
                    throw new ArgumentOutOfRangeException("At least 2 points must be provided.");
                }

                this.curve = value;

                this.SetUpVertexBuffer();
            }
        }

        #region Overridden Methods
        protected override void Initialize()
        {
            this.identityTransform = Matrix.Identity;
            this.platform = WaveServices.Platform;

            base.Initialize();
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
            if (this.curve != null)
            {
                this.vertexIndex = 0;

                for (int i = 0; i < this.curve.Length - 1; i++)
                {
                    this.point = this.curve[i];
                    this.nextPoint = this.curve[i + 1];
                    this.SetUpPointsAndVertices(i);
                }

                this.point = this.curve[this.curve.Length - 2];
                this.nextPoint = this.curve[this.curve.Length - 1];
                this.SetUpPointsAndVertices(lastPoint: true);

                this.UpdateGraphicDeviceAndDraw();
            }
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
                    PrimitiveType.TriangleStrip)
                {
                    DisableBatch = true
                };
            }
        }

        protected override void DrawDebugLines()
        {
            base.DrawDebugLines();

            if (this.curve != null)
            {
                for (int i = 0; i < this.curve.Length - 1; i++)
                {
                    this.RenderManager.LineBatch2D.DrawLine(this.curve[i], this.curve[i + 1], Color.Red, 0f);
                }
            }
        }

        private void SetUpPointsAndVertices(int i = 1, bool lastPoint = false)
        {
            this.SetUpPoints();

            Vector2 goodPoint = lastPoint ? this.nextPoint : this.point;
            this.SetUpVertex(i, ref goodPoint);
            this.SetUpVertex(i, ref goodPoint);
        }

        private void SetUpPoints()
        {
            var vectorBetweenPoints = this.nextPoint - this.point;
            var perpendicularVector = new Vector2(-vectorBetweenPoints.Y, vectorBetweenPoints.X);
            this.unitaryPerpendicularVector = Vector2.Normalize(perpendicularVector);
        }

        private void SetUpVertex(int i, ref Vector2 point)
        {
            float x, y;
            Vector2 texCoord;

            if ((this.vertexIndex % 2) == 0)
            {
                x = point.X + this.unitaryPerpendicularVector.X;
                y = point.Y + this.unitaryPerpendicularVector.Y;
                texCoord = (i % 2) == 0 ? Vector2.Zero : Vector2.UnitY;
            }
            else
            {
                x = point.X - this.unitaryPerpendicularVector.X;
                y = point.Y - this.unitaryPerpendicularVector.Y;
                texCoord = (i % 2) == 0 ? Vector2.UnitX : Vector2.One;
            }

            if (this.RenderManager.ActiveRenderTarget != null && this.platform.AdapterType != AdapterType.DirectX)
            {
                this.vertices[this.vertexIndex].Position = new Vector3(x, platform.ScreenHeight - y, 1f);
            }
            else
            {
                this.vertices[this.vertexIndex].Position = new Vector3(x, y, 1f);
            }

            this.vertices[this.vertexIndex].TexCoord = texCoord;
            this.vertices[this.vertexIndex].Color = Color.White;

            this.vertexIndex++;
        }

        private void UpdateGraphicDeviceAndDraw()
        {
            this.mesh.VertexBuffer.SetData(this.vertices, this.vertices.Length);
            this.GraphicsDevice.BindVertexBuffer(this.mesh.VertexBuffer);

            this.RenderManager.DrawMesh(this.mesh, this.materialsMap.DefaultMaterial, ref this.identityTransform);
        }
        #endregion
    }
}
