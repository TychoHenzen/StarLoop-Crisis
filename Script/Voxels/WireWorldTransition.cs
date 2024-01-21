using System;
using System.Linq;

namespace StarLoop.Script.Voxels;

public readonly struct WireWorldTransition : IEquatable<WireWorldTransition>
{
    public readonly Rule[] Neighbors;
    public readonly byte Self;
    public readonly byte Result;
    public readonly float Odds;

    public WireWorldTransition(Rule[] neighbors, byte self, byte result, float odds= 1)
    {
        Neighbors = neighbors;
        Self = self;
        Result = result;
        Odds = odds;
    }
    
    public WireWorldTransition(Rule[] neighbors, Cell self, Cell result, float odds = 1)
    {
        Neighbors = neighbors;
        Odds = odds;
        Self = (byte)self;
        Result = (byte)result;
    }

    public bool Equals(WireWorldTransition other)
    {
        return Neighbors.SequenceEqual(other.Neighbors) && Self == other.Self && Result == other.Result && Math.Abs(Odds - other.Odds) < 0.01f;
    }

    public override bool Equals(object obj)
    {
        return obj is WireWorldTransition other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Neighbors, Self, Result, Odds);
    }

    public static bool operator ==(WireWorldTransition left, WireWorldTransition right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(WireWorldTransition left, WireWorldTransition right)
    {
        return !(left == right);
    }
}