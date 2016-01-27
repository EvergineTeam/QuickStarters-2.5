#region File Description
//-----------------------------------------------------------------------------
// Basket King
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace BasketKing.Drawables
{
    [DataContract(Namespace = "BasketKing.Drawables")]
    public class BezierCurve2DDrawable : Drawable2D
    {
        [RequiredComponent]
        private MaterialsMap materialsMap = null;

        private const int PointsNumber = 20;
        private Vector2[] curve;
        private Vector2[] controlPoints;

        private VertexPositionColorTexture[] vertices;
        private Mesh mesh;
        private Vector2 point;
        private Vector2 nextPoint;
        private Vector2 unitaryPerpendicularVector;
        private int vertexIndex;
        private Platform platform;

        private string destinationPath;
        private Entity destination;
        private Transform2D destinationTransform;

        [RequiredComponent]
        private Transform2D sourceTransform;
        
        [DataMember]
        [RenderPropertyAsEntity(new string[] { "WaveEngine.Framework.Graphics.Transform2D" })]
        public string DestinationEntity
        {
            get
            {
                return this.destinationPath;
            }

            set
            {
                this.destinationPath = value;
                this.destination = null;
                this.destinationTransform = null;

                if (this.isInitialized)
                {
                    this.RefreshDestinationEntity();
                }
            }
        }

        [DataMember]
        public float CurveWith
        {
            get;
            set;
        }

        #region Overridden Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.curve = new Vector2[PointsNumber];
            this.controlPoints = new Vector2[3];
            this.platform = WaveServices.Platform;
            this.CurveWith = 3.5f;

            this.RefreshDestinationEntity();
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
            if (this.destinationTransform == null)
            {
                return;
            }

            this.UpdateControlPoints();
            this.UpdateCurvePoints();
            this.SetUpVertexBuffer();

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
        #endregion

        #region Private Methods

        /// <summary>
        /// Refresh the target and destination entities
        /// </summary>
        private void RefreshDestinationEntity()
        {
            if (!string.IsNullOrEmpty(this.destinationPath))
            {
                this.destination = this.EntityManager.Find(this.destinationPath);

                if (this.destination != null)
                {
                    this.destinationTransform = this.destination.FindComponent<Transform2D>();
                }
            }
        }

        /// <summary>
        /// Refreshes the points.
        /// </summary>
        public void UpdateControlPoints()
        {
            if(this.sourceTransform == null || this.destinationTransform == null)
            {
                return;
            }

            // Destination
            this.controlPoints[2] = this.destinationTransform.Position;            

            // Source
            this.controlPoints[0] = this.sourceTransform.Position;            

            if(this.controlPoints[0].Y > this.controlPoints[2].Y)
            {
                this.controlPoints[1].X = (this.controlPoints[2].X + this.controlPoints[0].X) * 0.5f;
                this.controlPoints[1].Y = this.controlPoints[2].Y;
            }
            else
            {
                this.controlPoints[1].X = (this.controlPoints[0].X + this.controlPoints[2].X) * 0.5f;
                this.controlPoints[1].Y = this.controlPoints[0].Y;
            }
        }

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
                this.vertices[this.vertexIndex].Position = new Vector3(x, platform.ScreenHeight - y, this.sourceTransform.DrawOrder);
            }
            else
            {
                this.vertices[this.vertexIndex].Position = new Vector3(x, y, this.sourceTransform.DrawOrder);
            }

            this.vertices[this.vertexIndex].TexCoord = texCoord;
            this.vertices[this.vertexIndex].Color = Color.White;

            this.vertexIndex++;
        }

        private void UpdateGraphicDeviceAndDraw()
        {
            this.mesh.ZOrder = this.sourceTransform.DrawOrder;
            this.mesh.VertexBuffer.SetData(this.vertices, this.vertices.Length);
            this.GraphicsDevice.BindVertexBuffer(this.mesh.VertexBuffer);

            this.RenderManager.DrawMesh(this.mesh, this.materialsMap.DefaultMaterial, Matrix.Identity);
        } 

        /// <summary>
        /// Update the curve points from the control points
        /// </summary>
        private void UpdateCurvePoints()
        {
            for (var i = 0; i < this.curve.Length; i++)
            {
                this.curve[i] = this.CalcPointAtStep(i);
            }
        }

        /// <summary>
        /// Calcs the point at step.
        /// Algorithm taken from: http://rosettacode.org/wiki/Bitmap/B%C3%A9zier_curves/Quadratic#C
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        private Vector2 CalcPointAtStep(int i)
        {
            Vector2 resultingPoint;

            float t = (float)i / PointsNumber;
            float a = (float)Math.Pow(1 - t, 2);
            float b = 2 * t * (1 - t);
            float c = (float)Math.Pow(t, 2);
            resultingPoint.X = (a * this.controlPoints[0].X) + (b * this.controlPoints[1].X) + (c * this.controlPoints[2].X);
            resultingPoint.Y = (a * this.controlPoints[0].Y) + (b * this.controlPoints[1].Y) + (c * this.controlPoints[2].Y);

            return resultingPoint;
        }
        #endregion
    }
}
