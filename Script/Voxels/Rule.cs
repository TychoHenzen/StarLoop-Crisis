using System;

namespace StarLoop.Script.Voxels;

public readonly struct Rule : IEquatable<Rule>
{
    public readonly byte Type;
    public readonly byte Min;
    public readonly byte Max;

    public Rule(byte type, byte min, byte max)
    {
        Type = type;
        Min = min;
        Max = max;
    }
    public Rule(Cell type, byte min, byte max)
    {
        Type = (byte)type;
        Min = min;
        Max = max;
    }

    public bool Equals(Rule other)
    {
        return Type == other.Type && Min == other.Min && Max == other.Max;
    }

    public override bool Equals(object obj)
    {
        return obj is Rule other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, Min, Max);
    }
}