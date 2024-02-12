#region

using System;
using System.Linq;
using StarLoop.Script.Voxels.Data;

#endregion

namespace StarLoop.Script.Voxels.WireWorld;

public readonly struct WireWorldTransition : IEquatable<WireWorldTransition>
{
  public Rule[] Neighbors { get; }

  public readonly byte Self;
  public readonly byte Result;
  public readonly Func<double, bool> Odds;

  public WireWorldTransition(Rule[] neighbors, byte self, byte result,
    Func<double, bool> odds = null)
  {
    Neighbors = neighbors;
    Self = self;
    Result = result;
    Odds = odds ?? VoxelConstants.AlwaysOdds;
  }

  public WireWorldTransition(Rule[] neighbors, Cell self, Cell result,
    Func<double, bool> odds = null)
  {
    Neighbors = neighbors;
    Odds = odds ?? VoxelConstants.AlwaysOdds;
    Self = (byte)self;
    Result = (byte)result;
  }

  public bool Equals(WireWorldTransition other) =>
    Neighbors.SequenceEqual(other.Neighbors) &&
    Self == other.Self &&
    Result == other.Result &&
    Odds(1) == other.Odds(1) &&
    Odds(0) == other.Odds(0);

  public override bool Equals(object obj) =>
    obj is WireWorldTransition other && Equals(other);

  public override int GetHashCode() =>
    HashCode.Combine(Neighbors, Self, Result, Odds);

  public static bool operator ==(WireWorldTransition left, WireWorldTransition right) =>
    left.Equals(right);

  public static bool operator !=(WireWorldTransition left, WireWorldTransition right) =>
    !(left == right);
}