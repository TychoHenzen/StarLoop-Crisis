using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace StarLoop.Script.Voxels;

public readonly struct WireWorldTransition : IEquatable<WireWorldTransition>
{
    public readonly ImmutableArray<Rule> Neighbors;
    public readonly byte Self;
    public readonly byte Result;
    public readonly Func<double, bool> Odds;

    public WireWorldTransition(IEnumerable<Rule> neighbors, byte self, byte result, Func<double, bool> odds = null)
    {
        Neighbors = neighbors.ToImmutableArray();
        Self = self;
        Result = result;
        Odds = odds ?? (_ => true);
    }

    public WireWorldTransition(IEnumerable<Rule> neighbors, Cell self, Cell result, Func<double, bool> odds = null)
    {
        Neighbors = neighbors.ToImmutableArray();
        Odds = odds ?? (_ => true);
        Self = (byte)self;
        Result = (byte)result;
    }

    public bool Equals(WireWorldTransition other)
    {
        return Neighbors.SequenceEqual(other.Neighbors) && Self == other.Self && Result == other.Result &&
               Odds(1) == other.Odds(1) && Odds(0) == other.Odds(0);
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