using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace SuperSlingshot.Drawables
{
    [DataContract]
    public class LinesDrawable : Drawable2D
    {
        protected Vector2[] curve;
        protected VertexPositionColorTexture[] vertices;
        protected int vertexIndex;
        protected Platform platform;
        protected Mesh mesh;

        [RequiredComponent]
        protected MaterialsMap materialsMap;

        [RequiredComponent]
        protected Transform2D transform2D;

        [DataMember]
        public int NumberOfPoints { get; set; }

        [DataMember]
        public float CurveWith { get; set; }

        [DataMember]
        public Color DiffuseColor { get; set; }

        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.curve = new Vector2[0];
            this.NumberOfPoints = 40;
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.curve = new Vector2[this.NumberOfPoints];
            this.platform = WaveServices.Platform;
        }

        public override void Draw(TimeSpan gameTime)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (this.mesh != null)
            {
                this.GraphicsDevice.DestroyIndexBuffer(this.mesh.IndexBuffer);
                this.GraphicsDevice.DestroyVertexBuffer(this.mesh.VertexBuffer);
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
    }
}
