using System;
using System.Collections.Generic;

namespace MonoGame.Samples.Library.Graphics;

public class ResourceRegistry<T> where T : class, INamedResource
{
    private static readonly Dictionary<ushort, T> _ids = [];
    private static readonly Dictionary<string, T> _names = [];
    private static ushort _nextId = 0;
    private static bool[] _accquiredIds = new bool[32];

    internal static ushort Regist (string name, T resource)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual (_ids.Count, ushort.MaxValue + 1);

        if (_names.ContainsKey (name))
        {
            throw new ArgumentException ($"A resource type of '{typeof (T)}' with the name '{name}' already exists.");
        }

        ushort id = AccquireId ();

        if (_ids.ContainsKey (id))
        {
            throw new ArgumentException ($"A resource type of '{typeof (T)}' with the id '{id}' already exists.");
        }

        _ids.Add (id, resource);
        _names.Add (name, resource);

        return id;
    }

    internal static void UnRegist (ushort id)
    {
        if (!_ids.TryGetValue (id, out T? material))
        {
            return;
        }

        _ids.Remove (id);
        _names.Remove (material.Name);

        ReleaseId (id);
    }

    internal static void UnRegist (string name)
    {
        if (!_names.TryGetValue (name, out T? material))
        {
            return;
        }

        _ids.Remove (material.Id);
        _names.Remove (name);

        ReleaseId (material.Id);
    }

    internal static void UnRegist (T material) => UnRegist (material.Id);

    public static bool TryGetValue (ushort id, out T? resource) => _ids.TryGetValue (id, out resource);

    public static bool TryGetValue (string name, out T? resource) => _names.TryGetValue (name, out resource);

    public static ushort GetId (string name)
    {
        if (_names.TryGetValue (name, out T? resource))
        {
            return resource.Id;
        }

        return 0;
    }

    public static string GetName (ushort id)
    {
        if (_ids.TryGetValue (id, out T? resource))
        {
            return resource.Name;
        }

        return string.Empty;
    }

    private static ushort AccquireId ()
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual (_ids.Count, ushort.MaxValue + 1);

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

        _accquiredIds[_nextId] = true;

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
