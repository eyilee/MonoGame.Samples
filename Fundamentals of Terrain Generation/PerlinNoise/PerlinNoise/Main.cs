using MonoGame.Samples.Library;

namespace MonoGame.Samples.PerlinNoise
{
    public class Main : Core
    {
        private const int WindowWidth = 512;
        private const int WindowHeight = 512;

        public Main ()
            : base ("PerlinNoise", WindowWidth, WindowHeight, false)
        {
        }

        protected override void Initialize ()
        {
            ChangeScene (new GameScene ());

            base.Initialize ();
        }
    }
}
