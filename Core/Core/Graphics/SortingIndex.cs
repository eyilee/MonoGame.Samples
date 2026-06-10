using System;

namespace MonoGame.Samples.Library.Graphics;

public struct SortingIndex (int index, ulong sortKey) : IComparable<SortingIndex>
{
    public int Index { get; set; } = index;

    public ulong SortKey { get; set; } = sortKey;

    public readonly int CompareTo (SortingIndex other)
    {
        int r = SortKey.CompareTo (other.SortKey);
        if (r != 0)
        {
            return r;
        }

        return Index.CompareTo (other.Index);
    }
}
