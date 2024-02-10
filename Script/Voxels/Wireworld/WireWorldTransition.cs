#region

using System;
using System.Linq;

#endregion

namespace StarLoop.Script.Voxels;

public readonly struct WireWorldTransition : IEquatable<WireWorldTransition>
{
    public Rule[] Neighbors { get; }

    public readonly byte Self;
    public readonly byte Result;
    public readonly Func<double, bool> Odds;

    public WireWorldTransition(Rule[] neighbors, byte self, byte result, Func<double, bool> odds = null)
    {
        Neighbors = neighbors;
        Self = self;
        Result = result;
        Odds = odds ?? (_ => true);
    }

    public WireWorldTransition(Rule[] neighbors, Cell self, Cell result, Func<double, bool> odds = null)
    {
        Neighbors = neighbors;
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