using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Samples.Library.Canvas;

public class Canvas : IDisposable
{
    public int Width { get; init; }

    public int Height { get; init; }

    public int PixelSize { get; init; }

    public int PixelWidth => Width * PixelSize;

    public int PixelHeight => Height * PixelSize;

    public int OffsetX { get; set; }

    public int OffsetY { get; set; }

    private readonly Texture2D _texture;
    private readonly Color[] _pixels;
    private bool _isDirty = true;

    private bool _disposed;

    public Canvas (GraphicsDevice graphicsDevice, int width, int height, int pixelSize = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero (width, nameof (width));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero (height, nameof (height));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero (pixelSize, nameof (pixelSize));

        _texture = new Texture2D (graphicsDevice, width, height, false, SurfaceFormat.Color);
        _pixels = new Color[width * height];

        Width = width;
        Height = height;
        PixelSize = pixelSize;
    }

    ~Canvas () => Dispose (false);

    public void SetPixel (int x, int y, Color color)
    {
        _pixels[GetIndex (x, y)] = color;

        _isDirty = true;
    }

    public Color GetPixel (int x, int y) => _pixels[GetIndex (x, y)];

    private int GetIndex (int x, int y) => y * Width + x;

    public void Clear (Color? color)
    {
        Array.Fill (_pixels, color ?? Color.Transparent);

        _isDirty = true;
    }

    public void SetOffset (int offsetX, int offsetY)
    {
        OffsetX = offsetX;
        OffsetY = offsetY;
    }

    public void Translate (int offsetX, int offsetY)
    {
        OffsetX += offsetX;
        OffsetY += offsetY;
    }

    public void Draw (SpriteBatch spriteBatch)
    {
        if (_isDirty)
        {
            _texture.SetData (_pixels);
            _isDirty = false;
        }

        spriteBatch.Draw (_texture, new Rectangle (OffsetX, OffsetY, PixelWidth, PixelHeight), Color.White);
    }

    public void Dispose ()
    {
        Dispose (true);
        GC.SuppressFinalize (this);
    }

    protected virtual void Dispose (bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _texture.Dispose ();
            }

            _disposed = true;
        }
    }
}