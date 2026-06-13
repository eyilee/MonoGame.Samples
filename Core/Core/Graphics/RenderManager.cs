using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Samples.Library.Graphics;

public class RenderManager
{
    private RenderCommand[] _commands = new RenderCommand[32];
    private SortingIndex[] _sortingIndices = new SortingIndex[32];
    private int _commandCount = 0;

    private readonly QuadBatcher<VertexPositionColorTexture> _batcher;
    private readonly QuadInstanceBatcher<VertexSdfInstance> _sdfInstanceBatcher;

    public RenderManager (GraphicsDevice graphicsDevice)
    {
        _batcher = new QuadBatcher<VertexPositionColorTexture> (graphicsDevice, new SpriteBatchEncoder ());
        _sdfInstanceBatcher = new QuadInstanceBatcher<VertexSdfInstance> (graphicsDevice, new SdfInstanceBatchEncoder ());
    }

    public void Enqueue (RenderCommand command)
    {
        EnsureArrayCapacity ();

        int index = _commandCount++;
        _commands[index] = command;
        _sortingIndices[index] = new SortingIndex (index, command.SortKey);
    }

    private void EnsureArrayCapacity ()
    {
        if (_commandCount >= _commands.Length)
        {
            RenderCommand[] newCommands = new RenderCommand[_commands.Length * 2];
            _commands.CopyTo (newCommands, 0);
            _commands = newCommands;
        }

        if (_commandCount >= _sortingIndices.Length)
        {
            SortingIndex[] newSortingIndices = new SortingIndex[_sortingIndices.Length * 2];
            _sortingIndices.CopyTo (newSortingIndices, 0);
            _sortingIndices = newSortingIndices;
        }
    }

    public void Draw ()
    {
        if (_commandCount == 0)
        {
            return;
        }

        Array.Sort (_sortingIndices, 0, _commandCount);

        int batchStartIndex = 0;
        while (batchStartIndex < _commandCount)
        {
            ref SortingIndex firstSortingIndex = ref _sortingIndices[batchStartIndex];
            ref RenderCommand firstCommand = ref _commands[firstSortingIndex.Index];

            // TODO: select batcher by command

            int batchEndIndex = FindBatchEnd (batchStartIndex, firstCommand);
            for (int i = batchStartIndex; i < batchEndIndex; i++)
            {
                ref RenderCommand command = ref _commands[_sortingIndices[i].Index];
                _batcher.Batch (command.Mesh);
            }

            _batcher.DrawBatch (firstCommand.Material, firstCommand.Properties, firstCommand.Texture?.Texture);

            batchStartIndex = batchEndIndex;
        }

        _commandCount = 0;
    }

    private int FindBatchEnd (int startIndex, in RenderCommand firstCommand)
    {
        int commandIndex = startIndex + 1;
        while (commandIndex < _commandCount)
        {
            if (!CanBatch (firstCommand, _commands[_sortingIndices[commandIndex].Index]))
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
}
