using MonoGame.Samples.Library;

namespace MonoGame.Samples.VoronoiDiagram;

public class Main : Core
{
    private const int WindowWidth = 1280;
    private const int WindowHeight = 720;

    public Main ()
        : base ("VoronoiDiagram", WindowWidth, WindowHeight, false)
    {
    }

    protected override void Initialize ()
    {
        ChangeScene (new GameScene ());

        base.Initialize ();
    }
}
