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
        [DataMember]
        public Vector2 FixedPoint { get; set; }

        [DataMember]
        public Color Color { get; set; }

        [IgnoreDataMember]
        public Transform2D TargetTransform { get; set; }

        [DataMember]
        public float ZOrder { get; set; }

        public override void Draw(TimeSpan gameTime)
        {
            this.UpdateElasticPoint();
            this.SetUpVertexBuffer();

            this.SetupPointsAndVertices();

            this.UpdateGraphicDeviceAndDraw();
        }

        private void UpdateGraphicDeviceAndDraw()
        {
            this.mesh.ZOrder = this.ZOrder;
            this.mesh.VertexBuffer.SetData(this.vertices, this.vertices.Length);
            this.GraphicsDevice.BindVertexBuffer(this.mesh.VertexBuffer);

            // WORKAROUND: Force spritebatch to start a new render command
            this.layer.SpriteBatch.Draw(StaticResources.WhitePixel, -Vector2.One, null, Color.White, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, this.mesh.ZOrder);

            this.RenderManager.DrawMesh(this.mesh, this.materialsMap.DefaultMaterial, Matrix.Identity);
        }

        private void SetupPointsAndVertices()
        {
            Vector3 initialA = curve[0].ToVector3(0) + Vector3.UnitY * this.CurveWith;
            Vector3 initialB = curve[1].ToVector3(0) - Vector3.UnitY * this.CurveWith;

            Vector3 finalA = this.TargetTransform.Position.ToVector3(0);
            Vector3 finalB = this.TargetTransform.Position.ToVector3(0);

            this.SetVertice(0, initialA, Vector2.Zero);
            this.SetVertice(1, initialB, Vector2.UnitX);
            this.SetVertice(2, finalA, Vector2.UnitY);
            this.SetVertice(3, finalB, Vector2.One);
        }

        private void SetVertice(int i, Vector3 position, Vector2 texCoord)
        {
            this.vertices[i].Position = position;
            this.vertices[i].TexCoord = texCoord;
            this.vertices[i].Color = Color.White;
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

        private void UpdateElasticPoint()
        {
            this.curve[0] = this.FixedPoint;
            this.curve[1] = this.TargetTransform.Position;
        }
    }
}