using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace NeonShooter
{
    public static class EntityManager
    {
        static readonly List<Entity> m_EntityList = [];
        static readonly List<Entity> m_AddEntityList = [];
        static PlayerShip m_PlayerShip;
        static readonly List<Bullet> m_BulletList = [];
        static readonly List<Enemy> m_EnemyList = [];

        static bool m_IsUpdating = false;

        public static PlayerShip PlayerShip { get { return m_PlayerShip; } }

        public static void AddEntity (Entity _entity)
        {
            if (!m_IsUpdating)
            {
                m_EntityList.Add (_entity);
            }
            else
            {
                m_AddEntityList.Add (_entity);
            }

            if (_entity is PlayerShip)
            {
                m_PlayerShip = _entity as PlayerShip;
            }
            else if (_entity is Bullet)
            {
                m_BulletList.Add (_entity as Bullet);
            }
            else if (_entity is Enemy)
            {
                m_EnemyList.Add (_entity as Enemy);
            }
        }

        public static void Update ()
        {
            m_IsUpdating = true;

            HandleCollision ();

            foreach (Entity entity in m_EntityList)
            {
                entity.Update ();
            }

            m_IsUpdating = false;

            foreach (Entity entity in m_AddEntityList)
            {
                AddEntity (entity);
            }
            m_AddEntityList.Clear ();

            m_EntityList.RemoveAll (x => x.IsExpired);
            m_BulletList.RemoveAll (x => x.IsExpired);
            m_EnemyList.RemoveAll (x => x.IsExpired);
        }

        public static void Draw (SpriteBatch _spriteBatch)
        {
            foreach (Entity entity in m_EntityList)
            {
                entity.Draw (_spriteBatch);
            }
        }

        public static void HandleCollision ()
        {
            foreach (Enemy enemy in m_EnemyList)
            {
                if (!enemy.IsExpired)
                {
                    float radius = enemy.Radius + m_PlayerShip.Radius;
                    if (Vector2.DistanceSquared (enemy.Position, m_PlayerShip.Position) < radius * radius)
                    {
                        m_PlayerShip.Kill ();
                    }
                }
            }

            foreach (Enemy enemy in m_EnemyList)
            {
                foreach (Bullet bullet in m_BulletList)
                {
                    if (!enemy.IsExpired && !bullet.IsExpired)
                    {
                        float radius = enemy.Radius + bullet.Radius;
                        if (Vector2.DistanceSquared (enemy.Position, bullet.Position) < radius * radius)
                        {
                            enemy.Kill ();
                            bullet.Kill ();
                        }
                    }
                }
            }
        }

        public static void Reset ()
        {
            m_PlayerShip.Reset ();

            foreach (Enemy enemy in m_EnemyList)
            {
                enemy.Kill ();
            }

            foreach (Bullet bullet in m_BulletList)
            {
                bullet.Kill ();
            }
        }
    }
}
