using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace NeonShooter
{
    public static class EnemyManager
    {
        static readonly List<Enemy> m_EnemyList = [];
        public static List<Enemy> EnemyList {  get { return m_EnemyList; } }

        public static void AddEnemy (Enemy _enemy)
        {
            m_EnemyList.Add (_enemy);
        }

        public static void Update ()
        {
            foreach (Enemy enemy in m_EnemyList)
            {
                enemy.Update ();
            }

            m_EnemyList.RemoveAll (x => x.IsExpired);
        }

        public static void Draw (SpriteBatch _spriteBatch)
        {
            foreach (Enemy enemy in m_EnemyList)
            {
                enemy.Draw (_spriteBatch);
            }
        }
    }
}
