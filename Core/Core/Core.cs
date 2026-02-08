using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Samples.Library.Input;
using System;

namespace MonoGame.Samples.Library;

public class Core : Game
{
    private static Core? s_instance;

    public static Core Instance => s_instance!;

    private static Scene? s_activeScene;

    private static Scene? s_nextScene;

    public static Scene? ActiveScene => s_activeScene;

    private static Camera s_defaultCamera = null!;

    public static Camera MainCamera { get; private set; } = null!;

    public static GraphicsDeviceManager Graphics { get; private set; } = null!;

    public static new ContentManager Content { get; private set; } = null!;

    public static new GraphicsDevice GraphicsDevice { get; private set; } = null!;

    public static int ScreenWidth => Graphics.PreferredBackBufferWidth;

    public static int ScreenHeight => Graphics.PreferredBackBufferHeight;

    public static SpriteBatch SpriteBatch { get; private set; } = null!;

    public static InputManager Input { get; private set; } = null!;

    public static bool ExitOnEscape { get; set; }

    public Core (string title, int width, int height, bool isFullScreen)
    {
        if (s_instance != null)
        {
            throw new InvalidOperationException ($"Only a single Core instance can be created");
        }

        s_instance = this;

        Window.Title = title;

        Graphics = new GraphicsDeviceManager (this)
        {
            PreferredBackBufferWidth = width,
            PreferredBackBufferHeight = height,
            IsFullScreen = isFullScreen
        };

        Content = base.Content;
        Content.RootDirectory = "Content";

        IsMouseVisible = true;

        ExitOnEscape = true;
    }

    protected override void Initialize ()
    {
        GraphicsDevice = base.GraphicsDevice;

        SpriteBatch = new SpriteBatch (GraphicsDevice);

        Input = new InputManager ();

        base.Initialize ();
    }

    protected override void LoadContent ()
    {
        s_defaultCamera = new Camera (GraphicsDevice);

        SetCamera (s_defaultCamera);

        base.LoadContent ();
    }

    protected override void Update (GameTime gameTime)
    {
        Input.Update (gameTime);

        if (ExitOnEscape && Input.Keyboard.WasJustPressed (Keys.Escape))
        {
            Exit ();
        }

        if (s_nextScene != null)
        {
            TransitionScene ();
        }

        s_activeScene?.Update (gameTime);

        base.Update (gameTime);
    }

    protected override void Draw (GameTime gameTime)
    {
        s_activeScene?.Draw (gameTime);

        base.Draw (gameTime);
    }

    public static void ChangeScene (Scene next)
    {
        if (s_activeScene != next)
        {
            s_nextScene = next;
        }
    }

    private static void TransitionScene ()
    {
        s_activeScene?.Dispose ();

        GC.Collect ();

        s_activeScene = s_nextScene;

        s_nextScene = null;

        s_activeScene?.Initialize ();
    }

    public static void SetCamera (Camera? camera)
    {
        MainCamera = camera ?? s_defaultCamera;
    }
}
