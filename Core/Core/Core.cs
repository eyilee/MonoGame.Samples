using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Samples.Library
{
    /// <summary>
    /// The core MonoGame application class.
    /// </summary>
    public class Core : Game
    {
        private static Core? s_instance;

        /// <summary>
        /// Gets the singleton instance of the Core application.
        /// </summary>
        public static Core Instance => s_instance!;

        /// <summary>
        /// Gets the graphics device manager.
        /// </summary>
        public static GraphicsDeviceManager Graphics { get; private set; } = null!;

        /// <summary>
        /// Gets the content manager.
        /// </summary>
        public static new ContentManager Content { get; private set; } = null!;

        /// <summary>
        /// Gets the graphics device.
        /// </summary>
        public static new GraphicsDevice GraphicsDevice { get; private set; } = null!;

        /// <summary>
        /// Gets the sprite batch for rendering 2D graphics.
        /// </summary>
        public static SpriteBatch SpriteBatch { get; private set; } = null!;

        /// <summary>
        /// Initializes a new instance of the Core class.
        /// </summary>
        /// <param name="title">Title of the window.</param>
        /// <param name="width">Width of the window.</param>
        /// <param name="height">Height of the window.</param>
        /// <param name="isFullScreen">Indicates whether the window should be fullscreen.</param>
        /// <exception cref="InvalidOperationException">Thrown if an instance of Core already exists.</exception>
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
        }

        /// <summary>
        /// Initializes the core components of the application.
        /// </summary>
        protected override void Initialize ()
        {
            GraphicsDevice = base.GraphicsDevice;

            SpriteBatch = new SpriteBatch (GraphicsDevice);

            base.Initialize ();
        }
    }
}
