using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Samples.Library;
using MonoGame.Samples.Library.Content;
using MonoGame.Samples.Library.Graphics;

namespace MonoGame.Samples.Test;

public class GameScene : Scene
{
    private MaterialInstance _materialInstance = null!;
    private TextureHandle _textureHandle = null!;

    public override void Initialize ()
    {
        SpriteEffect spriteEffect = new (GraphicsDevice);
        Material spriteMaterial = MaterialManager.CreateMaterial ("Sprite", spriteEffect);
        _materialInstance = spriteMaterial.CreateInstance ();

        base.Initialize ();
    }

    public override void LoadContent ()
    {
        Texture2D texture = new (GraphicsDevice, 1, 1);
        texture.SetData ([Color.White]);
        _textureHandle = new TextureHandle (texture, 0);

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

        Render.Enqueue (new RenderCommand (_materialInstance, null, new Rectangle (50, 50, 50, 50), _textureHandle, new Rectangle (), Color.Red, Vector2.Zero));

        base.Draw (gameTime);
    }
}
