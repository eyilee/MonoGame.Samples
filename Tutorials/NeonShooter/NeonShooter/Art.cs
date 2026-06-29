using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NeonShooter
{
    public static class Art
    {
        public static Texture2D Player { get; private set; }
        public static Texture2D Seeker { get; private set; }
        public static Texture2D Wanderer { get; private set; }
        public static Texture2D Bullet { get; private set; }
        public static Texture2D Pointer { get; private set; }
        public static Texture2D BlackHole { get; private set; }

        public static Texture2D LineParticle { get; private set; }
        public static Texture2D Glow { get; private set; }
        public static Texture2D Pixel { get; private set; }

        public static SpriteFont Font { get; private set; }

        public static void Load (ContentManager _contentManager, GraphicsDevice _graphicsDevice)
        {
            Player = _contentManager.Load<Texture2D> ("Art/Player");
            Seeker = _contentManager.Load<Texture2D> ("Art/Seeker");
            Wanderer = _contentManager.Load<Texture2D> ("Art/Wanderer");
            Bullet = _contentManager.Load<Texture2D> ("Art/Bullet");
            Pointer = _contentManager.Load<Texture2D> ("Art/Pointer");
            BlackHole = _contentManager.Load<Texture2D> ("Art/Black Hole");

            LineParticle = _contentManager.Load<Texture2D> ("Art/Laser");
            Glow = _contentManager.Load<Texture2D> ("Art/Glow");

            Pixel = new Texture2D (_graphicsDevice, 1, 1);
            Pixel.SetData ([Color.White]);

            Font = _contentManager.Load<SpriteFont> ("Font");
        }
    }
}
