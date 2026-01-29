using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Samples.Library.SDF
{
    struct SDFQuad : IVertexType
    {
        public Vector2 Position;

        public static readonly VertexDeclaration VertexDeclaration = new (
                new VertexElement (0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0)
            );

        readonly VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
    }

    struct SDFInstance : IVertexType
    {
        public Vector2 Position;
        public Vector2 Scale;
        public Vector4 ShapeData0;
        public Vector4 ShapeMask0;
        public Color Color;

        public static readonly VertexDeclaration VertexDeclaration = new (
            new VertexElement (0, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement (8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement (16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3),
            new VertexElement (32, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 4),
            new VertexElement (48, VertexElementFormat.Color, VertexElementUsage.Color, 1)
            );

        readonly VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
    }

    public class SDFBatch
    {
        private GraphicsDevice _graphicsDevice;

        private readonly VertexBuffer _quadBuffer;
        private readonly DynamicVertexBuffer _instanceBuffer;
        IndexBuffer quadIndexBuffer;
        SDFInstance[] _instances;
        SDFEffect _sdfEffect;

        public SDFBatch (GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _sdfEffect = new SDFEffect (graphicsDevice);

            _quadBuffer = new VertexBuffer (graphicsDevice, SDFQuad.VertexDeclaration, 4, BufferUsage.WriteOnly);
            _quadBuffer.SetData ([
                new SDFQuad { Position = new Vector2 (-0.5f, -0.5f) },
                new SDFQuad { Position = new Vector2 ( 0.5f, -0.5f) },
                new SDFQuad { Position = new Vector2 (-0.5f,  0.5f) },
                new SDFQuad { Position = new Vector2 ( 0.5f,  0.5f) }
                ]);

            quadIndexBuffer = new IndexBuffer (
                graphicsDevice,
                IndexElementSize.SixteenBits,
                6,
                BufferUsage.WriteOnly);

            quadIndexBuffer.SetData (new ushort[]
            {
                0, 1, 2,
                2, 1, 3
            });

            _instanceBuffer = new DynamicVertexBuffer (graphicsDevice, SDFInstance.VertexDeclaration, 10, BufferUsage.WriteOnly);
            _instances = new SDFInstance[10];

            for (int i = 0; i < 10; i++)
            {
                _instances[i] = new SDFInstance
                {
                    Position = new Vector2 (i * 20, i * 20),
                    Scale = new Vector2 (4, 4),
                    ShapeData0 = new Vector4 (10, 0, 5, 0), // line length, circle radius etc
                    ShapeMask0 = new Vector4 (0, 1, 0, 0),  // only circle active
                    Color = Color.Lerp (Color.Red, Color.Yellow, i / 10f)
                };
            }
            _instanceBuffer.SetData (_instances);
        }

        public void Draw ()
        {
            _graphicsDevice.SetVertexBuffers (
                new VertexBufferBinding (_quadBuffer, 0, 0),
                new VertexBufferBinding (_instanceBuffer, 0, 1)
                );

            _graphicsDevice.Indices = quadIndexBuffer;

            _sdfEffect.Parameters["WorldViewProjection"].SetValue (Matrix.CreateOrthographicOffCenter (
                0, _graphicsDevice.Viewport.Width,
                _graphicsDevice.Viewport.Height, 0,
                0, 1));

            foreach (EffectPass pass in _sdfEffect.CurrentTechnique.Passes)
            {
                pass.Apply ();
                _graphicsDevice.DrawInstancedPrimitives (
                    PrimitiveType.TriangleStrip,
                    0, 0,
                    2,            // 2 triangles per quad
                    10   // instance count
                );
            }
        }
    }
}
