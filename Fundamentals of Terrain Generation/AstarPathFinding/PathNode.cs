namespace AstarPathFinding;

public class PathNode (int index, PathNode? parent, int gCost, int hCost)
{
    public int Index => index;

    public PathNode? Parent { get; set; } = parent;

    public PathNode? Next { get; set; }

    public int GCost { get; set; } = gCost;

    public int HCost => hCost;

    public int Cost => GCost + HCost;

    public bool IsPath = false;
}
