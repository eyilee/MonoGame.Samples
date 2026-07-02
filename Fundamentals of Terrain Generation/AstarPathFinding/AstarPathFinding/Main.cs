using MonoGame.Library;

namespace AstarPathFinding;

public class Main : Core
{
    private const int WindowWidth = 1280;

    private const int WindowHeight = 720;

    public Main ()
        : base ("AstarPathFinding", WindowWidth, WindowHeight, false)
    {
    }

    protected override void Initialize ()
    {
        ChangeScene (new GameScene ());

        base.Initialize ();
    }
}
