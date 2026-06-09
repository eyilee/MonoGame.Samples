using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Samples.Library;
using MonoGame.Samples.Library.Content;
using MonoGame.Samples.Library.Graphics;

namespace MonoGame.Samples.Test;

public class GameScene : Scene
{
    private MaterialInstance _materialInstance = null!;
    private TextureHandle _textureHandle1 = null!;
    private TextureHandle _textureHandle2 = null!;

    public override void Initialize ()
    {
        SpriteEffect spriteEffect = new (GraphicsDevice);
        Material spriteMaterial = MaterialManager.CreateMaterial ("Sprite", spriteEffect);
        _materialInstance = spriteMaterial.CreateInstance ();

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

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Render.Enqueue (new RenderCommand (_materialInstance, null, new Rectangle (50 + i * 60, 50 + j * 60, 50, 50), _textureHandle1, new Rectangle (), Color.White, Vector2.Zero, depth: 0.01f));
                Render.Enqueue (new RenderCommand (_materialInstance, null, new Rectangle (80 + i * 60, 80 + j * 60, 50, 50), _textureHandle2, new Rectangle (), Color.White, Vector2.Zero, depth: 0.02f));
            }
        }

        base.Draw (gameTime);
    }
}
