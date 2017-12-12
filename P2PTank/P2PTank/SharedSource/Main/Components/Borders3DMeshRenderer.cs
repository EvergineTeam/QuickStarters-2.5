using System.Collections.Generic;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics3D;
using WaveEngine.TiledMap;

namespace P2PTank.Components
{
    public class Borders3DMeshRenderer : CustomMesh
    {
        public float WallHeight { get; set; }

        private readonly List<VertexPosition> vertices = new List<VertexPosition>();
        private readonly List<ushort> indices = new List<ushort>();

        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.WallHeight = 1.0f;
        }

        public Borders3DMeshRenderer(List<TiledMapObject> borders)
        {
            float scale = 1 / 30.7f; ;
            foreach (var border in borders)
            {
                int firstVertice = this.vertices.Count;
                foreach (var point in border.Points)
                {
                    this.vertices.Add(new VertexPosition() { Position = new Vector3((float)point.X + border.X, 0, (float)point.Y + border.Y) * scale });
                    this.vertices.Add(new VertexPosition() { Position = new Vector3((float)point.X + border.X, 50.0f, (float)point.Y + border.Y) * scale });
                }
                int verticeCount = this.vertices.Count - firstVertice;

                for (int i = firstVertice; i < (verticeCount * 2) + 2; i += 2)
                {
                    this.indices.Add((ushort)(i + 0));
                    this.indices.Add((ushort)(i + 2));
                    this.indices.Add((ushort)(i + 3));
                    this.indices.Add((ushort)(i + 0));
                    this.indices.Add((ushort)(i + 3));
                    this.indices.Add((ushort)(i + 1));

                    this.indices.Add((ushort)(i + 0));
                    this.indices.Add((ushort)(i + 3));
                    this.indices.Add((ushort)(i + 2));
                    this.indices.Add((ushort)(i + 0));
                    this.indices.Add((ushort)(i + 1));
                    this.indices.Add((ushort)(i + 3));
                }
                this.indices.Add((ushort)(this.vertices.Count - 1));
                this.indices.Add((ushort)(firstVertice));
                this.indices.Add((ushort)(firstVertice + 1));
                this.indices.Add((ushort)(this.vertices.Count - 1));
                this.indices.Add((ushort)(firstVertice + 1));
                this.indices.Add((ushort)(this.vertices.Count));
            }

            var vertexBuffer = new VertexBuffer(VertexPosition.VertexFormat);
            vertexBuffer.SetData<VertexPosition>(this.vertices.ToArray());
            var indexBuffer = new IndexBuffer(this.indices.ToArray());

            this.Mesh = new Mesh(vertexBuffer, indexBuffer, PrimitiveType.TriangleList);
        }
    }
}