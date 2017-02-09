using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;

namespace SuperSlingshot.Drawables
{
    [DataContract]
    public class ElasticBandsDrawable : LinesDrawable
    {
        private Mesh secondaryMesh;
        private VertexPositionColorTexture[] secondaryVertices;

        [DataMember]
        public Vector2 FixedPointATop { get; set; }

        [DataMember]
        public Vector2 FixedPointABottom { get; set; }

        [DataMember]
        public Vector2 FixedPointBTop { get; set; }

        [DataMember]
        public Vector2 FixedPointBBottom { get; set; }

        [DataMember]
        public Color Color { get; set; }

        [IgnoreDataMember]
        public Transform2D TargetTransform { get; set; }

        [DataMember]
        public float ZOrderA { get; set; }

        [DataMember]
        public float ZOrderB { get; set; }

        public override void Draw(TimeSpan gameTime)
        {
            this.SetUpVertexBuffer();

            this.SetupPointsAndVertices();

            this.UpdateGraphicDeviceAndDraw();
        }

        private void UpdateGraphicDeviceAndDraw()
        {
            this.mesh.ZOrder = this.ZOrderA;
            this.secondaryMesh.ZOrder = this.ZOrderB;

            this.mesh.VertexBuffer.SetData(this.vertices, this.vertices.Length);
            this.secondaryMesh.VertexBuffer.SetData(this.secondaryVertices, this.secondaryVertices.Length);

            this.GraphicsDevice.BindVertexBuffer(this.mesh.VertexBuffer);
            this.GraphicsDevice.BindVertexBuffer(this.secondaryMesh.VertexBuffer);

            // WORKAROUND: Force spritebatch to start a new render command
            this.layer.SpriteBatch.Draw(StaticResources.WhitePixel, -Vector2.One, null, Color.White, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, this.mesh.ZOrder);

            if (this.TargetTransform.Position.X > this.FixedPointBTop.X)
            {
                this.RenderManager.DrawMesh(this.secondaryMesh, this.materialsMap.Materials["elastic"], Matrix.Identity);
                this.RenderManager.DrawMesh(this.mesh, this.materialsMap.Materials["elastic"], Matrix.Identity);
                
            }
            else
            {
                this.RenderManager.DrawMesh(this.mesh, this.materialsMap.Materials["elastic"], Matrix.Identity);
                this.RenderManager.DrawMesh(this.secondaryMesh, this.materialsMap.Materials["elastic"], Matrix.Identity);
            }
        }

        private void SetupPointsAndVertices()
        {
            var offset = 40;

            this.CalculateSling(this.FixedPointATop.ToVector3(0), this.FixedPointABottom.ToVector3(0), this.FixedPointBTop.ToVector3(0), this.FixedPointBBottom.ToVector3(0), offset);
        }

        private void CalculateSling(Vector3 fixedAPointTop, Vector3 fixedAPointBottom, Vector3 fixedBPointTop, Vector3 fixedBPointBottom, int offset)
        {
            var currentTargetPosition = this.TargetTransform.Position.ToVector3(0);

            Vector3 finalTop = currentTargetPosition - fixedAPointTop;
            Vector3 finalBottom = currentTargetPosition - fixedAPointBottom;

            finalTop.Normalize();
            finalBottom.Normalize();

            this.SetVertice(0, fixedAPointTop, Vector2.Zero, ref this.vertices);
            this.SetVertice(1, fixedAPointBottom, Vector2.UnitX, ref this.vertices);
            this.SetVertice(3, currentTargetPosition + finalTop * offset, Vector2.One, ref this.vertices);
            this.SetVertice(2, currentTargetPosition + finalBottom * offset, Vector2.UnitY, ref this.vertices);

            this.SetVertice(0, fixedBPointTop, Vector2.Zero, ref this.secondaryVertices);
            this.SetVertice(1, fixedBPointBottom, Vector2.UnitX, ref this.secondaryVertices);
            this.SetVertice(3, currentTargetPosition + finalTop * offset, Vector2.One, ref this.secondaryVertices);
            this.SetVertice(2, currentTargetPosition + finalBottom * offset, Vector2.UnitY, ref this.secondaryVertices);
        }

        private void SetVertice(int i, Vector3 position, Vector2 texCoord, ref VertexPositionColorTexture[] vertices)
        {
            vertices[i].Position = position;
            vertices[i].TexCoord = texCoord;
            vertices[i].Color = Color.White;
        }

        private void SetUpVertexBuffer()
        {
            if (this.mesh == null)
            {
                int verticesNumber = 4;

                this.vertices = new VertexPositionColorTexture[verticesNumber];
                this.secondaryVertices = new VertexPositionColorTexture[verticesNumber];

                DynamicVertexBuffer vertexBuffer = new DynamicVertexBuffer(VertexPositionColorTexture.VertexFormat);
                DynamicVertexBuffer secondaryVertexBuffer = new DynamicVertexBuffer(VertexPositionColorTexture.VertexFormat);

                ushort[] indices = new ushort[verticesNumber];

                for (ushort i = 0; i < indices.Length; i++)
                {
                    indices[i] = i;
                }

                var indexBuffer = new IndexBuffer(indices);
                var secondaryIndexBuffer = new IndexBuffer(indices);

                this.mesh = new Mesh(
                    0,
                    verticesNumber,
                    0,
                    verticesNumber - 2,
                    vertexBuffer,
                    indexBuffer,
                    PrimitiveType.TriangleStrip);

                this.secondaryMesh = new Mesh(
                    0,
                    verticesNumber,
                    0,
                    verticesNumber - 2,
                    secondaryVertexBuffer,
                    secondaryIndexBuffer,
                    PrimitiveType.TriangleStrip);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (this.secondaryMesh != null)
            {
                this.GraphicsDevice.DestroyIndexBuffer(this.secondaryMesh.IndexBuffer);
                this.GraphicsDevice.DestroyVertexBuffer(this.secondaryMesh.VertexBuffer);
            }
        }
    }
}