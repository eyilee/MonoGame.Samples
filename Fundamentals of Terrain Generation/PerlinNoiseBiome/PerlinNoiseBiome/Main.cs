using MonoGame.Samples.Library;

namespace MonoGame.Samples.PerlinNoiseBiome;

public class Main : Core
{
    private const int WindowWidth = 512;
    private const int WindowHeight = 512;

    public Main ()
        : base ("PerlinNoiseBiome", WindowWidth, WindowHeight, false)
    {
    }

    protected override void Initialize ()
    {
        ChangeScene (new GameScene ());

        base.Initialize ();
    }
}
