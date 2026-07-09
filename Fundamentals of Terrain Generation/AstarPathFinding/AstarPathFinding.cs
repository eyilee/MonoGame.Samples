using Microsoft.Xna.Framework;
using MonoGame.Library;
using MonoGame.Library.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AstarPathFinding
{
    public class AstarPathFinding
    {
        private readonly Random _random = new ();

        private readonly Sprite _spriteTemplate;

        private readonly Text _textTemplate;

        private readonly int _width;

        private readonly int _height;

        private readonly int _size;

        private readonly bool[] _originNodes;

        private readonly Node[] _nodes;

        private readonly (int, int)[] _neighborOffsets =
        [
            new (-1, 0),
            new (0, 1),
            new (1, 0),
            new (0, -1)
        ];

        private readonly float _aliveRate;

        private int _startIndex;

        private int _endIndex;

        private readonly Dictionary<int, PathNode> _openList = [];

        private readonly Dictionary<int, PathNode> _closedList = [];

        private IEnumerator<int>? _stepBehaviour;

        private Color _aliveColor = new (224, 224, 224, 255);

        private Color _deadColor = new (32, 32, 32, 255);

        private Color _openListColor = new (96, 255, 96, 255);

        private Color _closedListColor = new (255, 96, 96, 255);

        private Color _costTextColor = new (96, 96, 96, 255);

        private Color _pathColor = Color.Yellow;

        private Color _pathTextColor = Color.Black;

        public AstarPathFinding (int width, int height, int cellSize, float aliveRate)
        {
            if (Texture2DResource.TryGetValue ("Pixel", out Texture2DResource? texture) && texture != null)
            {
                _spriteTemplate = new Sprite (new TextureRegion (texture, 0, 0, 1, 1));
            }
            else
            {
                throw new InvalidOperationException ("Pixel resource not found");
            }

            if (FontResource.TryGetValue ("Font", out FontResource? font) && font != null)
            {
                _textTemplate = new Text (font);
            }
            else
            {
                throw new InvalidOperationException ("Font resource not found");
            }

            _width = width;
            _height = height;
            _size = width * height;
            _originNodes = new bool[_size];
            _nodes = new Node[_size];

            Vector2 offset = new ((Core.ScreenWidth - (width * cellSize)) / 2, (Core.ScreenHeight - (height * cellSize)) / 2);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Sprite sprite = _spriteTemplate.Clone ();
                    sprite.Size = new Vector2 (cellSize, cellSize);
                    sprite.Position = offset + new Vector2 (x * cellSize, y * cellSize);
                    sprite.Color = _deadColor;
                    sprite.Origin = Vector2.Zero;

                    Text text = _textTemplate.Clone ();
                    text.Value = string.Empty;
                    text.Position = offset + new Vector2 (x * cellSize + cellSize / 2f, y * cellSize + cellSize / 2f);
                    text.Color = _costTextColor;

                    _nodes[GetIndex (x, y)] = new Node ()
                    {
                        Value = false,
                        Sprite = sprite,
                        Text = text
                    };
                }
            }

            _aliveRate = aliveRate;

            Reset ();
        }

        public void Reset ()
        {
            for (int i = 0; i < _size; i++)
            {
                bool isAlive = _random.NextDouble () < _aliveRate;
                _originNodes[i] = isAlive;

                Node node = _nodes[i];
                node.Value = isAlive;
                node.Color = isAlive ? _aliveColor : _deadColor;
                node.TextValue = string.Empty;
                node.TextColor = _costTextColor;
            }

            _startIndex = -1;
            _endIndex = -1;

            while (_startIndex == -1 || !_nodes[_startIndex].Value)
            {
                _startIndex = _random.Next (_size);
            }

            while (_endIndex == -1 || !_nodes[_endIndex].Value || _endIndex == _startIndex)
            {
                _endIndex = _random.Next (_size);
            }

            Node startNode = _nodes[_startIndex];
            startNode.Color = _pathColor;
            startNode.TextValue = "S";
            startNode.TextColor = _pathTextColor;

            Node endNode = _nodes[_endIndex];
            endNode.Color = _pathColor;
            endNode.TextValue = "E";
            endNode.TextColor = _pathTextColor;

            _openList.Clear ();
            _openList.Add (_startIndex, new PathNode (_startIndex, null, 0, GetHeuristicCost (_startIndex, _endIndex)));

            _closedList.Clear ();

            _stepBehaviour = Run ();

            Draw ();
        }

        public void Redo ()
        {
            for (int i = 0; i < _size; i++)
            {
                bool isAlive = _originNodes[i];

                Node node = _nodes[i];
                node.Value = isAlive;
                node.Color = isAlive ? _aliveColor : _deadColor;
                node.TextValue = string.Empty;
                node.TextColor = _costTextColor;
            }

            Node startNode = _nodes[_startIndex];
            startNode.Color = _pathColor;
            startNode.TextValue = "S";
            startNode.TextColor = _pathTextColor;

            Node endNode = _nodes[_endIndex];
            endNode.Color = _pathColor;
            endNode.TextValue = "E";
            endNode.TextColor = _pathTextColor;

            _openList.Clear ();
            _openList.Add (_startIndex, new PathNode (_startIndex, null, 0, GetHeuristicCost (_startIndex, _endIndex)));

            _closedList.Clear ();

            _stepBehaviour = Run ();

            Draw ();
        }

        public void NextStep ()
        {
            if (_stepBehaviour != null)
            {
                if (!_stepBehaviour.MoveNext ())
                {
                    _stepBehaviour = null;
                }

                Draw ();
            }
        }

        private IEnumerator<int> Run ()
        {
            while (_openList.Count > 0)
            {
                (int index, PathNode pathNode) = _openList.MinBy (p => p.Value.Cost);

                _openList.Remove (index);
                _closedList.Add (index, pathNode);

                if (index == _endIndex)
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

                    yield break;
                }

                (int x, int y) = GetCoordinates (index);

                foreach ((int xOffset, int yOffset) in _neighborOffsets)
                {
                    int neighborX = x + xOffset;
                    int neighborY = y + yOffset;

                    if (neighborX < 0 || neighborX >= _width || neighborY < 0 || neighborY >= _height)
                    {
                        continue;
                    }

                    int neighborIndex = GetIndex (neighborX, neighborY);

                    if (!_nodes[neighborIndex].Value)
                    {
                        continue;
                    }

                    if (_closedList.ContainsKey (neighborIndex))
                    {
                        continue;
                    }

                    int gCost = pathNode.GCost + 1;

                    if (_openList.TryGetValue (neighborIndex, out PathNode? neighborNode))
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

        private void Draw ()
        {
            foreach ((int index, PathNode pathNode) in _openList)
            {
                if (index != _startIndex && index != _endIndex)
                {
                    Node node = _nodes[index];
                    node.Color = _openListColor;
                    node.TextValue = pathNode.Cost.ToString ();
                    node.TextColor = _costTextColor;
                }
            }

            foreach ((int index, PathNode pathNode) in _closedList)
            {
                if (index != _startIndex && index != _endIndex)
                {
                    Node node = _nodes[index];

                    if (pathNode.IsPath && pathNode.Next != null)
                    {
                        node.Color = _pathColor;

                        (int x0, int y0) = GetCoordinates (pathNode.Index);
                        (int x1, int y1) = GetCoordinates (pathNode.Next.Index);

                        int dx = x1 - x0;
                        int dy = y1 - y0;

                        if (dx == -1)
                        {
                            node.TextValue = "←";
                        }
                        else if (dy == 1)
                        {
                            node.TextValue = "↓";
                        }
                        else if (dx == 1)
                        {
                            node.TextValue = "→";
                        }
                        else if (dy == -1)
                        {
                            node.TextValue = "↑";
                        }

                        node.TextColor = _pathTextColor;
                    }
                    else
                    {
                        node.Color = _closedListColor;
                        node.TextValue = pathNode.Cost.ToString ();
                        node.TextColor = _costTextColor;
                    }
                }
            }
        }

        public void Draw (RenderManager render)
        {
            foreach (Node node in _nodes)
            {
                node.Sprite.Draw (render);
            }

            foreach (Node node in _nodes)
            {
                node.Text.Draw (render);
            }
        }

        private int GetIndex (int x, int y) => x + (y * _width);

        private (int, int) GetCoordinates (int index) => (index % _width, index / _width);

        private int GetHeuristicCost (int indexA, int indexB)
        {
            (int xA, int yA) = GetCoordinates (indexA);
            (int xB, int yB) = GetCoordinates (indexB);

            return Math.Abs (xA - xB) + Math.Abs (yA - yB);
        }
    }
}
