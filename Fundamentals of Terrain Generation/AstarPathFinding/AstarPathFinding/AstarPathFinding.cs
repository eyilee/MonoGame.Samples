using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AstarPathFinding
{
    public class AstarPathFinding
    {
        private static readonly Random _random = new ();

        private readonly Texture2D _texture;
        private readonly int _cellSize = 32;

        private readonly int _size;
        private readonly float _aliveRate;

        private static readonly Tuple<int, int>[] _neighborOffsets =
        [
            new (-1, 0),
            new (0, 1),
            new (1, 0),
            new (0, -1)
        ];

        private bool[] _cellMap;
        private int _startIndex;
        private int _endIndex;

        private IEnumerator<int> _stepBehaviour;

        private class PathNode (int index, PathNode parent, int gCost, int hCost)
        {
            public int Index = index;
            public PathNode Parent = parent;
            public int GCost = gCost;
            public int HCost = hCost;
            public int Cost => GCost + HCost;
            public bool IsPath = false;
        }

        private readonly Dictionary<int, PathNode> _openList = [];
        private readonly Dictionary<int, PathNode> _closedList = [];

        private int GetIndex (int x, int y) => x + (y * _size);

        private Tuple<int, int> GetCoordinates (int index)
        {
            int x = index % _size;
            int y = index / _size;
            return new Tuple<int, int> (x, y);
        }

        private int GetHeuristicCost (int indexA, int indexB)
        {
            (int xA, int yA) = GetCoordinates (indexA);
            (int xB, int yB) = GetCoordinates (indexB);
            return Math.Abs (xA - xB) + Math.Abs (yA - yB);
        }

        public AstarPathFinding (int size, float aliveRate)
        {
            _texture = new Texture2D (Core.GraphicsDevice, 1, 1);
            _texture.SetData ([Color.White]);

            if (size <= 0)
            {
                throw new ArgumentException ("Size must be greater than 0", nameof (size));
            }

            _size = size;
            _aliveRate = aliveRate;

            Reset ();
        }

        public void Reset ()
        {
            _cellMap = new bool[_size * _size];
            Array.Fill (_cellMap, false);

            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    _cellMap[GetIndex (x, y)] = _random.NextDouble () < _aliveRate;
                }
            }

            _startIndex = -1;
            while (_startIndex == -1 || !_cellMap[_startIndex])
            {
                _startIndex = _random.Next (_size * _size);
            }

            _endIndex = -1;
            while (_endIndex == -1 || !_cellMap[_endIndex] || _endIndex == _startIndex)
            {
                _endIndex = _random.Next (_size * _size);
            }

            _openList.Clear ();
            _openList.Add (_startIndex, new PathNode (_startIndex, null, 0, GetHeuristicCost (_startIndex, _endIndex)));

            _closedList.Clear ();

            _stepBehaviour = AstarPathFindingStepBehaviour ();
        }

        public void NextStep ()
        {
            if (_stepBehaviour != null)
            {
                if (!_stepBehaviour.MoveNext ())
                {
                    _stepBehaviour = null;
                }
            }
        }

        private IEnumerator<int> AstarPathFindingStepBehaviour ()
        {
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

                    yield break;
                }

                (int x, int y) = GetCoordinates (index);

                foreach ((int xOffset, int yOffset) in _neighborOffsets)
                {
                    if (x + xOffset < 0 || x + xOffset >= _size || y + yOffset < 0 || y + yOffset >= _size)
                    {
                        continue;
                    }

                    int neighborIndex = GetIndex (x + xOffset, y + yOffset);
                    if (!_cellMap[neighborIndex])
                    {
                        continue;
                    }

                    if (_closedList.ContainsKey (neighborIndex))
                    {
                        continue;
                    }

                    int gCost = pathNode.GCost + 1;

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

                yield return 0;
            }
        }

        public void Draw (SpriteBatch spriteBatch)
        {
            Color aliveColor = new (224, 224, 224, 255);
            Color deadColor = new (32, 32, 32, 255);
            Color openListColor = new (96, 255, 96, 255);
            Color closedListColor = new (255, 96, 96, 255);
            Color costTextColor = new (96, 96, 96, 255);
            Color pathColor = Color.Yellow;
            Color pathTextColor = Color.Black;

            for (int index = 0; index < _cellMap.Length; index++)
            {
                DrawCell (spriteBatch, index, _cellMap[index] ? aliveColor : deadColor);
            }

            foreach (PathNode pathNode in _openList.Values)
            {
                DrawCell (spriteBatch, pathNode.Index, openListColor);

                if (pathNode.Index != _startIndex && pathNode.Index != _endIndex)
                {
                    DrawCellText (spriteBatch, pathNode.Index, pathNode.Cost.ToString (), costTextColor);
                }
            }

            foreach (PathNode pathNode in _closedList.Values)
            {
                DrawCell (spriteBatch, pathNode.Index, pathNode.IsPath ? pathColor : closedListColor);

                if (pathNode.Index != _startIndex && pathNode.Index != _endIndex)
                {
                    if (pathNode.IsPath && pathNode.Parent != null)
                    {
                        (int x0, int y0) = GetCoordinates (pathNode.Parent.Index);
                        (int x1, int y1) = GetCoordinates (pathNode.Index);

                        int dx = x1 - x0;
                        int dy = y1 - y0;

                        if (dx == -1)
                        {
                            DrawCellText (spriteBatch, pathNode.Index, "←", pathTextColor);
                        }
                        else if (dy == 1)
                        {
                            DrawCellText (spriteBatch, pathNode.Index, "↑", pathTextColor);
                        }
                        else if (dx == 1)
                        {
                            DrawCellText (spriteBatch, pathNode.Index, "→", pathTextColor);
                        }
                        else if (dy == -1)
                        {
                            DrawCellText (spriteBatch, pathNode.Index, "↓", pathTextColor);
                        }
                    }
                    else
                    {
                        DrawCellText (spriteBatch, pathNode.Index, pathNode.Cost.ToString (), costTextColor);
                    }
                }
            }

            DrawCellText (spriteBatch, _startIndex, "S", pathTextColor);
            DrawCellText (spriteBatch, _endIndex, "E", pathTextColor);
        }

        private void DrawCell (SpriteBatch spriteBatch, int index, Color color)
        {
            (int x, int y) = GetCoordinates (index);
            spriteBatch.Draw (_texture, GetCellRectangle (x, y), color);
        }

        private Rectangle GetCellRectangle (int x, int y)
        {
            int halfSize = _size * _cellSize / 2;
            return new Rectangle ((x * _cellSize) - halfSize, halfSize - (y * _cellSize) - _cellSize, _cellSize, _cellSize);
        }

        private void DrawCellText (SpriteBatch spriteBatch, int index, string text, Color color)
        {
            (int x, int y) = GetCoordinates (index);
            int halfSize = _size * _cellSize / 2;
            Vector2 textSize = Core.Font.MeasureString (text);
            Vector2 position = new ((x * _cellSize) - halfSize + (_cellSize - textSize.X) / 2, halfSize - (y * _cellSize) - _cellSize + (_cellSize - textSize.Y) / 2);
            spriteBatch.DrawString (Core.Font, text, position, color);
        }
    }
}
