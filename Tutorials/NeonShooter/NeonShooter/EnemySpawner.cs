using Microsoft.Xna.Framework;
using System;

namespace NeonShooter
{
    public static class EnemySpawner
    {
        static readonly Random m_Random = new ();
        static float m_nInverseSpawnChance = 90f;
        static float m_nInverseBlackHoleChance = 600f;

        public static void Update ()
        {
            if (m_Random.Next ((int)m_nInverseSpawnChance) == 0)
            {
                EntityManager.AddEntity (Enemy.CreateSeeker (GetSpawnPosition (), Vector2.Zero, 0f));
            }

            if (m_Random.Next ((int)m_nInverseSpawnChance) == 0)
            {
                EntityManager.AddEntity (Enemy.CreateWanderer (GetSpawnPosition (), Vector2.Zero, 0f));
            }

            if (m_Random.Next ((int)m_nInverseBlackHoleChance) == 0)
            {
                EntityManager.AddEntity (new BlackHole (Art.BlackHole, GetSpawnPosition (), Vector2.Zero, 0f));
            }

            if (m_nInverseSpawnChance > 30f)
            {
                m_nInverseSpawnChance -= 0.005f;
            }
        }

        static Vector2 GetSpawnPosition ()
        {
            Vector2 position;

            do
            {
                position = new Vector2 (m_Random.Next (Game1.Width), m_Random.Next (Game1.Height));
            }
            while (Vector2.DistanceSquared (position, EntityManager.PlayerShip.Position) < 250 * 250);

            return position;
        }
    }
}
