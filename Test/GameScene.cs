using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Library;
using MonoGame.Library.Graphics;
using MonoGame.Library.Physics;

namespace Test;

public class GameScene : Scene
{
    private readonly SdfCircle _center = new ();

    private readonly Player _player = new ();

    private readonly PhysicsWorld _physicsWorld = new ();

    public override void Initialize ()
    {
        Vector2 position = new (Core.ScreenWidth / 2f, Core.ScreenHeight / 2f);

        _center.Position = position;
        _center.Thickness = 1f;
        _center.Color = Color.Green;
        _center.Radius = 5f;

        _player.Initialize (position, 0f);

        for (int i = 0; i < 10; i++)
        {
            var entity = new PhysicsEntity ();
            entity.Position = new Vector2 (position.X + i * 20f, position.Y + i * 20f);
            entity.Collider.Size = new Vector2 (30f, 30f);
            _physicsWorld.Add (entity);
        }

        base.Initialize ();
    }

    public override void Update (GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _player.Update (Input, deltaTime);

        _physicsWorld.Update (deltaTime);

        base.Update (gameTime);
    }

    public override void Draw (GameTime gameTime)
    {
        GraphicsDevice.Clear (Color.CornflowerBlue);

        _center.Draw (Render);
        _player.Draw (Render);

        base.Draw (gameTime);
    }
}
