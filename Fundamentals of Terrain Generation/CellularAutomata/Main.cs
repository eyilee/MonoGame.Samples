using MonoGame.Library;

namespace CellularAutomata;

public class Main : Core
{
    private const int WindowWidth = 800;

    private const int WindowHeight = 600;

    public Main ()
        : base ("CellularAutomata", WindowWidth, WindowHeight, false)
    {
    }

    protected override void Initialize ()
    {
        ChangeScene (new GameScene ());

        base.Initialize ();
    }
}
