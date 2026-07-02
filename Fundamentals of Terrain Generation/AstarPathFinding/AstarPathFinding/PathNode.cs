namespace AstarPathFinding;

public class PathNode (int index, PathNode? parent, int gCost, int hCost)
{
    public int Index = index;

    public PathNode? Parent = parent;

    public PathNode? Next;

    public int GCost = gCost;

    public int HCost = hCost;

    public int Cost => GCost + HCost;

    public bool IsPath = false;
}
