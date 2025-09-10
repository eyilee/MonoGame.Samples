using System;
using System.Collections.Generic;
using System.Linq;

namespace CellularAutomataCave
{
    public static class AstarPathFinding
    {
        private static readonly Random _random = new ();

        private static readonly int _aliveCost = 1;
        private static readonly int _deadCostBase = 3;
        private static readonly int _deadCostRange = 5;

        private static readonly Tuple<int, int>[] _neighborOffsets =
        [
            new (-1, 0),
            new (0, 1),
            new (1, 0),
            new (0, -1)
        ];

        private static bool[] _cellMap;
        private static int _size;
        private static int _startIndex;
        private static int _endIndex;

        private class PathNode (int index, PathNode parent, int gCost, int hCost)
        {
            public int Index = index;
            public PathNode Parent = parent;
            public int GCost = gCost;
            public int HCost = hCost;
            public int Cost => GCost + HCost;
            public bool IsPath = false;
        }

        private static readonly Dictionary<int, PathNode> _openList = [];
        private static readonly Dictionary<int, PathNode> _closedList = [];

        private static int GetIndex (int x, int y) => x + (y * _size);

        private static Tuple<int, int> GetCoordinates (int index)
        {
            int x = index % _size;
            int y = index / _size;
            return new Tuple<int, int> (x, y);
        }

        private static int GetHeuristicCost (int indexA, int indexB)
        {
            (int xA, int yA) = GetCoordinates (indexA);
            (int xB, int yB) = GetCoordinates (indexB);
            return Math.Abs (xA - xB) + Math.Abs (yA - yB);
        }

        public static void Reset ()
        {
            _cellMap = null;
            _startIndex = -1;
            _endIndex = -1;
            _openList.Clear ();
            _closedList.Clear ();
        }

        public static List<int> FindPath (bool[] cellMap, int size, int startIndex, int endIndex)
        {
            Reset ();

            _cellMap = cellMap;
            _size = size;
            _startIndex = startIndex;
            _endIndex = endIndex;
            _openList.Add (_startIndex, new PathNode (_startIndex, null, 0, GetHeuristicCost (_startIndex, _endIndex)));

            while (_openList.Count > 0)
            {
                KeyValuePair<int, PathNode> keyValuePair = _openList.MinBy (p => p.Value.Cost);

                int index = keyValuePair.Key;
                PathNode pathNode = keyValuePair.Value;

                _openList.Remove (index);
                _closedList.Add (index, pathNode);

                if (index == _endIndex)
                {
                    while (pathNode != null)
                    {
                        pathNode.IsPath = true;
                        pathNode = pathNode.Parent;
                    }

                    break;
                }

                (int x, int y) = GetCoordinates (index);

                foreach ((int xOffset, int yOffset) in _neighborOffsets)
                {
                    if (x + xOffset < 0 || x + xOffset >= _size || y + yOffset < 0 || y + yOffset >= _size)
                    {
                        continue;
                    }

                    int neighborIndex = GetIndex (x + xOffset, y + yOffset);
                    if (_closedList.ContainsKey (neighborIndex))
                    {
                        continue;
                    }

                    int gCost = pathNode.GCost + (_cellMap[neighborIndex] ? _aliveCost : _deadCostBase + _random.Next (_deadCostRange));

                    if (_openList.TryGetValue (neighborIndex, out PathNode neighborNode))
                    {
                        if (neighborNode.GCost > gCost)
                        {
                            neighborNode.Parent = pathNode;
                            neighborNode.GCost = gCost;
                        }
                    }
                    else
                    {
                        _openList.Add (neighborIndex, new PathNode (neighborIndex, pathNode, gCost, GetHeuristicCost (neighborIndex, _endIndex)));
                    }
                }
            }

            return [.. _closedList.Values.Where (p => p.IsPath).Select (p => p.Index)];
        }
    }
}
