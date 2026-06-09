using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Samples.Library.Graphics;

public class RenderManager
{
    private RenderCommand[] _commands = new RenderCommand[32];
    private SortingIndex[] _sortingIndexes = new SortingIndex[32];
    private int _commandCount = 0;
    private ushort _nextSequence = 0;

    private readonly QuadBatcher<VertexPositionColorTexture> _batcher;

    public RenderManager (GraphicsDevice graphicsDevice)
    {
        _batcher = new QuadBatcher<VertexPositionColorTexture> (graphicsDevice);
    }

    public void Enqueue (RenderCommand command)
    {
        EnsureArrayCapacity ();

        int index = _commandCount++;
        command.Sequence = _nextSequence++;
        _commands[index] = command;

        _sortingIndexes[index] = new SortingIndex ()
        {
            Index = index,
            SortKey = command.SortKey
        };
    }

    private void EnsureArrayCapacity ()
    {
        if (_commandCount >= _commands.Length)
        {
            RenderCommand[] newCommands = new RenderCommand[_commands.Length * 2];
            _commands.CopyTo (newCommands, 0);
            _commands = newCommands;
        }

        if (_commandCount >= _sortingIndexes.Length)
        {
            SortingIndex[] newSortingIndexes = new SortingIndex[_sortingIndexes.Length * 2];
            _sortingIndexes.CopyTo (newSortingIndexes, 0);
            _sortingIndexes = newSortingIndexes;
        }
    }

    public void Draw ()
    {
        if (_commandCount == 0)
        {
            return;
        }

        Array.Sort (_sortingIndexes, 0, _commandCount);

        int batchStartIndex = 0;
        while (batchStartIndex < _commandCount)
        {
            ref SortingIndex firstSortingIndex = ref _sortingIndexes[batchStartIndex];
            ref RenderCommand firstCommand = ref _commands[firstSortingIndex.Index];

            int batchEndIndex = FindBatchEnd (batchStartIndex, firstCommand);
            for (int i = batchStartIndex; i < batchEndIndex; i++)
            {
                DrawCommand (_commands[_sortingIndexes[i].Index]);
            }

            _batcher.DrawBatch (firstCommand.Material, firstCommand.Properties, firstCommand.Texture?.Texture);

            batchStartIndex = batchEndIndex;
        }

        _commandCount = 0;
        _nextSequence = 0;
    }

    private int FindBatchEnd (int startIndex, in RenderCommand firstCommand)
    {
        int commandIndex = startIndex + 1;
        while (commandIndex < _commandCount)
        {
            if (!CanBatch (firstCommand, _commands[_sortingIndexes[commandIndex].Index]))
            {
                break;
            }

            commandIndex++;
        }

        return commandIndex;
    }

    private static bool CanBatch (in RenderCommand firstCommand, in RenderCommand nextCommand)
    {
        return ReferenceEquals (nextCommand.Material, firstCommand.Material) &&
            ReferenceEquals (nextCommand.Properties, firstCommand.Properties) &&
            ReferenceEquals (nextCommand.Texture, firstCommand.Texture);
    }

    private void DrawCommand (in RenderCommand renderCommand)
    {
        ref QuadBatchItem<VertexPositionColorTexture> batchItem = ref _batcher.CreateBatchItem ();

        float x = renderCommand.Destination.X - renderCommand.Origin.X;
        float y = renderCommand.Destination.Y - renderCommand.Origin.Y;
        float w = renderCommand.Destination.Width;
        float h = renderCommand.Destination.Height;

        batchItem.TL.Position.X = x;
        batchItem.TL.Position.Y = y;
        batchItem.TL.Position.Z = renderCommand.Depth;
        batchItem.TL.Color = renderCommand.Color;
        batchItem.TL.TextureCoordinate.X = 0;
        batchItem.TL.TextureCoordinate.Y = 0;

        batchItem.TR.Position.X = x + w;
        batchItem.TR.Position.Y = y;
        batchItem.TR.Position.Z = renderCommand.Depth;
        batchItem.TR.Color = renderCommand.Color;
        batchItem.TR.TextureCoordinate.X = 1;
        batchItem.TR.TextureCoordinate.Y = 0;

        batchItem.BL.Position.X = x;
        batchItem.BL.Position.Y = y + h;
        batchItem.BL.Position.Z = renderCommand.Depth;
        batchItem.BL.Color = renderCommand.Color;
        batchItem.BL.TextureCoordinate.X = 0;
        batchItem.BL.TextureCoordinate.Y = 1;

        batchItem.BR.Position.X = x + w;
        batchItem.BR.Position.Y = y + h;
        batchItem.BR.Position.Z = renderCommand.Depth;
        batchItem.BR.Color = renderCommand.Color;
        batchItem.BR.TextureCoordinate.X = 1;
        batchItem.BR.TextureCoordinate.Y = 1;
    }
}
