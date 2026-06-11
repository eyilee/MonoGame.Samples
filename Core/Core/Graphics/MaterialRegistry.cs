using System;
using System.Collections.Generic;

namespace MonoGame.Samples.Library.Graphics;

internal static class MaterialRegistry
{
    private static readonly Dictionary<ushort, Material> _materialIds = [];
    private static readonly Dictionary<string, Material> _materialNames = [];
    private static ushort _nextId = 0;
    private static bool[] _accquiredIds = new bool[32];

    internal static ushort Regist (string name, Material material)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual (_materialIds.Count, ushort.MaxValue + 1);

        if (_materialNames.ContainsKey (name))
        {
            throw new ArgumentException ($"A material with the name '{name}' already exists.");
        }

        ushort id = AccquireId ();

        if (_materialIds.ContainsKey (id))
        {
            throw new ArgumentException ($"A material with the id '{id}' already exists.");
        }

        _materialIds.Add (id, material);
        _materialNames.Add (name, material);

        return id;
    }

    internal static void UnRegist (ushort id)
    {
        if (!_materialIds.TryGetValue (id, out Material? material))
        {
            return;
        }

        _materialIds.Remove (id);
        _materialNames.Remove (material.Name);
        ReleaseId (id);
    }

    internal static void UnRegist (string name)
    {
        if (!_materialNames.TryGetValue (name, out Material? material))
        {
            return;
        }

        _materialIds.Remove (material.Id);
        _materialNames.Remove (name);
        ReleaseId (material.Id);
    }

    internal static void UnRegist (Material material) => UnRegist (material.Id);

    private static ushort AccquireId ()
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual (_materialIds.Count, ushort.MaxValue + 1);

        unchecked
        {
            while (_accquiredIds[_nextId])
            {
                _nextId++;

                if (_nextId >= _accquiredIds.Length)
                {
                    Array.Resize (ref _accquiredIds, _accquiredIds.Length * 2);
                }
            }
        }

        return _nextId;
    }

    private static void ReleaseId (ushort id)
    {
        if (id >= _accquiredIds.Length)
        {
            return;
        }

        _accquiredIds[id] = false;
    }
}
