using System;

namespace MonoGame.Samples.Library.Graphics;

public struct SortingIndex : IComparable<SortingIndex>
{
    public int Index { get; set; }

    public ulong SortKey { get; set; }

    public readonly int CompareTo (SortingIndex other) => SortKey.CompareTo (other.SortKey);
}
