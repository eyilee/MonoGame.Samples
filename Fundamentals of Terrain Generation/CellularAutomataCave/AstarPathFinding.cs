using System;
using System.Collections.Generic;
using System.Linq;

namespace CellularAutomataCave
{
    public static class AstarPathFinding
    {
        private static readonly Random _random = new ();

        private static readonly int _aliveCost = 1;

        private static readonly int _deadCostBase = 5;

        private static readonly int _deadCostRange = 8;

        private static readonly (int, int)[] _neighborOffsets =
        [
            new (-1, 0),
            new (0, 1),
            new (1, 0),
            new (0, -1)
        ];

        public static List<int> FindPath (bool[] nodes, int width, int height, int startIndex, int endIndex)
        {
            Dictionary<int, PathNode> openList = [];
            Dictionary<int, PathNode> closedList = [];

            openList.Add (startIndex, new PathNode (startIndex, null, 0, GetHeuristicCost (startIndex, endIndex, width)));

            while (openList.Count > 0)
            {
                (int index, PathNode pathNode) = openList.MinBy (p => p.Value.Cost);

                openList.Remove (index);
                closedList.Add (index, pathNode);

                if (index == endIndex)
                {
                    while (true)
                    {
                        pathNode.IsPath = true;

                        if (pathNode.Parent == null)
                        {
                            break;
                        }

                        pathNode.Parent.Next = pathNode;
                        pathNode = pathNode.Parent;
                    }

                    break;
                }

                (int x, int y) = GetCoordinates (index, width);

                foreach ((int xOffset, int yOffset) in _neighborOffsets)
                {
                    int neighborX = x + xOffset;
                    int neighborY = y + yOffset;

                    if (neighborX < 0 || neighborX >= width || neighborY < 0 || neighborY >= height)
                    {
                        continue;
                    }

                    int neighborIndex = GetIndex (neighborX, neighborY, width);

                    if (closedList.ContainsKey (neighborIndex))
                    {
                        continue;
                    }

                    int gCost = pathNode.GCost + (nodes[neighborIndex] ? _aliveCost : _deadCostBase + _random.Next (_deadCostRange));

                    if (openList.TryGetValue (neighborIndex, out PathNode? neighborNode))
                    {
                        if (neighborNode.GCost > gCost)
                        {
                            neighborNode.Parent = pathNode;
                            neighborNode.GCost = gCost;
                        }
                    }
                    else
                    {
                        openList.Add (neighborIndex, new PathNode (neighborIndex, pathNode, gCost, GetHeuristicCost (neighborIndex, endIndex, width)));
                    }
                }
            }

            return [.. closedList.Values.Where (p => p.IsPath).Select (p => p.Index)];
        }

        private static int GetIndex (int x, int y, int width) => x + (y * width);

        private static (int, int) GetCoordinates (int index, int width) => (index % width, index / width);

        private static int GetHeuristicCost (int indexA, int indexB, int width)
        {
            (int xA, int yA) = GetCoordinates (indexA, width);
            (int xB, int yB) = GetCoordinates (indexB, width);

            return Math.Abs (xA - xB) + Math.Abs (yA - yB);
        }
    }
}
