using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Samples.Library.Canvas;

public class Canvas
{
    private readonly Texture2D _texture;

    public int Width { get; init; }

    public int Height { get; init; }

    public int PixelSize { get; init; }

    public int PixelWidth => Width * PixelSize;

    public int PixelHeight => Height * PixelSize;

    public Color[] Pixels { get; init; }

    public int OffsetX { get; set; }

    public int OffsetY { get; set; }

    public Canvas (GraphicsDevice graphicsDevice, int width, int height, int pixelSize = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero (width, nameof (width));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero (height, nameof (height));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero (pixelSize, nameof (pixelSize));

        _texture = new Texture2D (graphicsDevice, 1, 1);
        _texture.SetData ([Color.White]);

        Width = width;
        Height = height;
        PixelSize = pixelSize;
        Pixels = new Color[Width * Height];
    }

    public void SetPixel (int x, int y, Color color) => Pixels[GetIndex (x, y)] = color;

    public Color GetPixel (int x, int y) => Pixels[GetIndex (x, y)];

    private int GetIndex (int x, int y) => y * Width + x;

    public void Clear (Color? color) => Array.Fill (Pixels, color ?? Color.Transparent);

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
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                spriteBatch.Draw (_texture, new Rectangle (OffsetX + x * PixelSize, OffsetY + y * PixelSize, PixelSize, PixelSize), GetPixel (x, y));
            }
        }
    }
}
