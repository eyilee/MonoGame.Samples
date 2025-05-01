using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace NeonShooter
{
    static class BulletManager
    {
        static readonly List<Bullet> m_BulletList = [];

        public static void AddBullet (Bullet _bullet)
        {
            m_BulletList.Add (_bullet);
        }

        public static void Update ()
        {
            foreach (Bullet bullet in m_BulletList)
            {
                bullet.Update ();
            }

            m_BulletList.RemoveAll (x => x.IsExpired);
        }

        public static void Draw (SpriteBatch _spriteBatch)
        {
            foreach (Bullet bullet in m_BulletList)
            {
                bullet.Draw (_spriteBatch);
            }
        }
    }
}
