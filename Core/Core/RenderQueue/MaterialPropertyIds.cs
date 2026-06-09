using System;
using System.Collections.Generic;

namespace MonoGame.Samples.Library.RenderQueue;

public static class MaterialPropertyIds
{
    private static readonly Dictionary<string, int> _nameToId = [];
    private static readonly Dictionary<int, string> _idToName = [];
    private static int _nextId = 1;

    public static int GetId (string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace (name);

        if (_nameToId.TryGetValue (name, out int id))
        {
            return id;
        }

        id = _nextId++;

        _nameToId.Add (name, id);
        _idToName.Add (id, name);

        return id;
    }

    public static bool TryGetName (int id, out string name) => _idToName.TryGetValue (id, out name!);
}
