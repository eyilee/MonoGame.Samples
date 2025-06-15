using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace NeonShooter
{
    public class Particle
    {
        public Texture2D Texture = null;
        public Vector2 Position = Vector2.Zero;
        public Vector2 Velocity = Vector2.Zero;
        public Color Tint = Color.White;
        public float Rotation = 0f;
        public Vector2 Origin = Vector2.Zero;
        public Vector2 Scale = Vector2.One;

        public float Duration = 1f;
        public float NormalizeTime = 0f;
        public float LengthMultiplier = 1f;

        public void Update ()
        {
            Vector2.Add (ref Position, ref Velocity, out Position);

            float speed = Velocity.Length ();
            float alpha = Math.Min (1, Math.Min ((1f - NormalizeTime) * 2, speed * 1f));
            alpha *= alpha;

            Tint.A = (byte)(255 * alpha);

            Rotation = MathF.Atan2 (Velocity.Y, Velocity.X);

            Scale.X = LengthMultiplier * Math.Min (Math.Min (1f, 0.1f * speed + 0.1f), alpha);

            if (Position.X < 0)
            {
                Velocity.X = Math.Abs (Velocity.X);
            }
            else if (Position.X > Game1.Width)
            {
                Velocity.X = -Math.Abs (Velocity.X);
            }

            if (Position.Y < 0)
            {
                Velocity.Y = Math.Abs (Velocity.Y);
            }
            else if (Position.Y > Game1.Height)
            {
                Velocity.Y = -Math.Abs (Velocity.Y);
            }

            if (Math.Abs (Velocity.X) + Math.Abs (Velocity.Y) < 0.00000000001f)
            {
                Velocity = Vector2.Zero;
            }
            else
            {
                Velocity *= 0.96f;
            }
        }
    }
}
