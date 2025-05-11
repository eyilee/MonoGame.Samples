using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;

namespace NeonShooter
{
    public static class Sound
    {
        public static Song Music { get; private set; }

        private static readonly Random m_Rand = new ();

        private static SoundEffect[] m_Explosions;
        public static SoundEffect Explosion { get { return m_Explosions[m_Rand.Next (m_Explosions.Length)]; } }

        private static SoundEffect[] m_Shots;
        public static SoundEffect Shot { get { return m_Shots[m_Rand.Next (m_Shots.Length)]; } }

        private static SoundEffect[] m_Spawns;
        public static SoundEffect Spawn { get { return m_Spawns[m_Rand.Next (m_Spawns.Length)]; } }

        public static void Load (ContentManager _contentManager)
        {
            Music = _contentManager.Load<Song> ("Audio/Music");

            m_Explosions = Enumerable.Range (1, 8).Select (x => _contentManager.Load<SoundEffect> ("Audio/explosion-0" + x)).ToArray ();
            m_Shots = Enumerable.Range (1, 4).Select (x => _contentManager.Load<SoundEffect> ("Audio/shoot-0" + x)).ToArray ();
            m_Spawns = Enumerable.Range (1, 8).Select (x => _contentManager.Load<SoundEffect> ("Audio/spawn-0" + x)).ToArray ();
        }
    }
}
