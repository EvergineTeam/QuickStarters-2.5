#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace BasketKing.Drawables
{
    [DataContract(Namespace = "BasketKing.Drawables")]
    public class NetDrawable : Drawable2D
    {
        [RequiredComponent]
        private MaterialsMap materialsMap;

        [RequiredComponent]
        private Transform2D transform2D;

        private Vector2[] netPoints;
        private VertexPositionColorTexture[] vertices;
        private DynamicVertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        private Mesh mesh;
        private ushort[] indices;

        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // Vertex buffer
            vertices = new VertexPositionColorTexture[8];
            vertices[0].Position = new Vector3(100, 100, this.transform2D.DrawOrder);
            vertices[0].Color = Color.White;
            vertices[0].TexCoord = new Vector2(0, 0);

            vertices[1].Position = new Vector3(100, 200, this.transform2D.DrawOrder);
            vertices[1].Color = Color.White;
            vertices[1].TexCoord = new Vector2(0.05f, 0.33f);

            vertices[2].Position = new Vector3(100, 300, this.transform2D.DrawOrder);
            vertices[2].Color = Color.White;
            vertices[2].TexCoord = new Vector2(0.1f, 0.66f);

            vertices[3].Position = new Vector3(100, 400, this.transform2D.DrawOrder);
            vertices[3].Color = Color.White;
            vertices[3].TexCoord = new Vector2(0.15f, 1);

            vertices[4].Position = new Vector3(200, 100, this.transform2D.DrawOrder);
            vertices[4].Color = Color.White;
            vertices[4].TexCoord = new Vector2(1, 0);

            vertices[5].Position = new Vector3(200, 200, this.transform2D.DrawOrder);
            vertices[5].Color = Color.White;
            vertices[5].TexCoord = new Vector2(0.96f, 0.33f);

            vertices[6].Position = new Vector3(200, 300, this.transform2D.DrawOrder);
            vertices[6].Color = Color.White;
            vertices[6].TexCoord = new Vector2(0.93f, 0.66f);

            vertices[7].Position = new Vector3(200, 400, this.transform2D.DrawOrder);
            vertices[7].Color = Color.White;
            vertices[7].TexCoord = new Vector2(0.9f, 1);


            this.vertexBuffer = new DynamicVertexBuffer(VertexPositionColorTexture.VertexFormat);

            this.vertexBuffer.SetData(vertices, vertices.Length);

            // Index buffer
            this.indices = new ushort[3 * 6];

            indices[0] = 0;
            indices[1] = 4;
            indices[2] = 5;

            indices[3] = 0;
            indices[4] = 5;
            indices[5] = 1;

            indices[6] = 1;
            indices[7] = 5;
            indices[8] = 6;

            indices[9] = 1;
            indices[10] = 6;
            indices[11] = 2;

            indices[12] = 2;
            indices[13] = 6;
            indices[14] = 7;

            indices[15] = 2;
            indices[16] = 7;
            indices[17] = 3;

            this.indexBuffer = new IndexBuffer(indices);
            this.mesh = new Mesh(
                            0,
                            this.vertices.Length,
                            0,
                            this.indices.Length / 3,
                            this.vertexBuffer,
                            this.indexBuffer,
                            PrimitiveType.TriangleList);
        }

        /// <summary>
        /// Helper method that draws debug lines.
        /// </summary>
        /// <remarks>
        /// This method will only work on debug mode and if RenderManager.DebugLines /&gt;
        /// is set to <c>true</c>.
        /// </remarks>
        protected override void DrawDebugLines()
        {
            base.DrawDebugLines();

            Vector2 vertexPosition;
            for (int i = 0; i < this.vertices.Length; i++)
            {
                vertexPosition.X = this.vertices[i].Position.X;
                vertexPosition.Y = this.vertices[i].Position.Y;
                this.RenderManager.LineBatch2D.DrawPoint(vertexPosition, 20, Color.Red, 0);
            }
        }

        /// <summary>
        /// Allows to perform custom drawing.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        /// <remarks>
        /// This method will only be called if all the following points are true:
        /// <list type="bullet">
        /// <item>
        /// <description>The parent of the owner <see cref="T:WaveEngine.Framework.Entity" /> of the <see cref="T:WaveEngine.Framework.Drawable" /> cascades its visibility to its children and it is visible.</description>
        /// </item>
        /// <item>
        /// <description>The <see cref="T:WaveEngine.Framework.Drawable" /> is active.</description>
        /// </item>
        /// <item>
        /// <description>The owner <see cref="T:WaveEngine.Framework.Entity" /> of the <see cref="T:WaveEngine.Framework.Drawable" /> is active and visible.</description>
        /// </item>
        /// </list>
        /// </remarks>
        public override void Draw(TimeSpan gameTime)
        {
            this.UpdateNetPins();

            this.vertexBuffer.SetData(this.vertices, this.vertices.Length);
            this.GraphicsDevice.BindVertexBuffer(this.vertexBuffer);

            Matrix worldTransform = transform2D.WorldTransform;
            this.RenderManager.DrawMesh(this.mesh, this.materialsMap.DefaultMaterial, ref worldTransform);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (this.indexBuffer != null)
            {
                this.GraphicsDevice.DestroyIndexBuffer(this.indexBuffer);
            }

            if (this.vertexBuffer != null)
            {
                this.GraphicsDevice.DestroyVertexBuffer(this.vertexBuffer);
            }
        }        

        private void UpdateNetPins()
        {
            var pins = this.EntityManager.FindAllByTag("NetPin");
            int i = 0;

            foreach(var pin in pins)
            {
                if(i >= this.vertices.Length)
                {
                    break;
                }

                var pinEntity = pin as Entity;
                
                if(pin == null)
                {
                    continue;
                }

                Transform2D pinTransform = pinEntity.FindComponent<Transform2D>();
                Vector2 pinPosition = pinTransform.Position;
                this.vertices[i].Position = new Vector3(pinPosition.X, pinPosition.Y, this.transform2D.DrawOrder);

                i++;
            }
        }
    }
}
