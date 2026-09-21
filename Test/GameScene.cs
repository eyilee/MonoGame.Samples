using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Library;
using MonoGame.Library.Graphics;
using MonoGame.Library.Physics;

namespace Test;

public class GameScene : Scene
{
    private readonly SdfCircle _center = new ();

    private readonly Player _player = new ();

    private readonly List<Cube> _cubes = [];

    private readonly PhysicsWorld _physicsWorld = new ();

    public override void Initialize ()
    {
        Vector2 position = new (Core.ScreenWidth / 2f, Core.ScreenHeight / 2f);

        _center.Position = position;
        _center.Thickness = 1f;
        _center.Color = Color.Green;
        _center.Radius = 5f;

        _player.Initialize (position, 0f);
        _player.AttachPhysics (_physicsWorld);

        for (int i = 0; i < 10; i++)
        {
            Cube cube = new ();
            cube.Initialize (new Vector2 (position.X + i * 20f, position.Y + i * 20f), 0f);
            cube.AttachPhysics (_physicsWorld);
            cube.Size = new Vector2 (30f, 30f);
            _cubes.Add (cube);
        }

        base.Initialize ();
    }

    public override void Update (GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _player.Update (Input, deltaTime);
        _physicsWorld.Update (deltaTime);

        MouseState state = Mouse.GetState ();
        _cubes[0].Position = Camera.Main.Position + state.Position.ToVector2 ();

        base.Update (gameTime);
    }

    public override void Draw (GameTime gameTime)
    {
        GraphicsDevice.Clear (Color.CornflowerBlue);

        _center.Draw (Render);
        _player.Draw (Render);

        foreach (Cube cube in _cubes)
        {
            cube.Draw (Render);
        }

        base.Draw (gameTime);
    }
}
