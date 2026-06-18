using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Samples.Library;
using MonoGame.Samples.Library.Content;
using MonoGame.Samples.Library.Graphics;
using MonoGame.Samples.Library.Shaders;
using System.Collections.Generic;

namespace MonoGame.Samples.Test;

public class GameScene : Scene
{
    private MaterialInstance _materialInstance = null!;
    private MaterialInstance _sdfCircleMaterial = null!;
    private TextureHandle _textureHandle1 = null!;
    private TextureHandle _textureHandle2 = null!;
    private List<Mesh> _meshes = [];
    private List<Mesh> _sdfMeshes = [];

    public override void Initialize ()
    {
        SpriteEffect spriteEffect = new (GraphicsDevice);
        Material spriteMaterial = new ("Sprite", spriteEffect, batcherName: "Sprite");
        _materialInstance = spriteMaterial.CreateInstance ();

        SdfCircleEffect sdfCircleEffect = new (GraphicsDevice);
        Material sdfCircleMaterial = new ("SdfCircle", sdfCircleEffect, rasterizerState: RasterizerState.CullNone, batcherName: "SdfInstance");
        _sdfCircleMaterial = sdfCircleMaterial.CreateInstance ();
        _sdfCircleMaterial.PropertyBlock.SetMatrix ("WorldViewProjection", Camera.Main.GetViewProjectionMatrix ());


        for (int i = 0; i < 512; i++)
        {
            for (int j = 0; j < 512; j++)
            {
                _meshes.Add (CreateMesh (new Rectangle (i, j, 1, 1), ColorUtility.HSVToRGB (i / 512f, j / 512f, 1f)));
            }
        }

        _sdfMeshes.Add (CreateSdfCircleMesh (new Vector2 (100f, 100f), 50f, Color.Red, 5f));

        base.Initialize ();
    }

    public override void LoadContent ()
    {
        Texture2D texture1 = new (GraphicsDevice, 1, 1);
        texture1.SetData ([Color.White]);
        _textureHandle1 = new TextureHandle (texture1, 0);

        Texture2D texture2 = new (GraphicsDevice, 1, 1);
        texture2.SetData ([Color.Red]);
        _textureHandle2 = new TextureHandle (texture2, 1);

        base.LoadContent ();
    }

    public override void UnloadContent ()
    {
        base.UnloadContent ();
    }

    protected override void Dispose (bool disposing)
    {
        if (disposing)
        {
        }

        base.Dispose (disposing);
    }

    public override void Draw (GameTime gameTime)
    {
        GraphicsDevice.Clear (Color.CornflowerBlue);

        for (int i = 0; i < _meshes.Count; i++)
        {
            Render.Enqueue (new RenderCommand (_materialInstance, null, _meshes[i], (i % 2 == 0) ? _textureHandle1 : _textureHandle2));
        }

        for (int i = 0; i < _sdfMeshes.Count; i++)
        {
            Render.Enqueue (new RenderCommand (_sdfCircleMaterial, _sdfMeshes[i]));
        }

        base.Draw (gameTime);
    }

    private static Mesh CreateMesh (Rectangle destination, Color color, float depth = 0f)
    {
        Mesh mesh = new ();

        float x = destination.X;
        float y = destination.Y;
        float w = destination.Width;
        float h = destination.Height;

        mesh.SetVertices ([
            new Vector3 (x, y, depth),
            new Vector3 (x + w, y, depth),
            new Vector3 (x, y + h, depth),
            new Vector3 (x + w, y + h, depth)
            ]);

        mesh.SetColors ([color, color, color, color]);

        mesh.SetUVs ([
            new Vector2 (0f, 0f),
            new Vector2 (1f, 0f),
            new Vector2 (0f, 1f),
            new Vector2 (1f, 1f)
            ]);

        return mesh;
    }

    private static Mesh CreateSdfCircleMesh (Vector2 center, float radius, Color color, float thickness = 1f)
    {
        Mesh mesh = new ();

        mesh.SetUVs ([center]);
        mesh.SetUV1s ([new Vector4 (0f, (radius + thickness) * 2, (radius + thickness) * 2, thickness)]);
        mesh.SetUV2s ([new Vector4 (radius, 0f, 0f, 0f)]);
        mesh.SetColors ([color]);

        return mesh;
    }
}
