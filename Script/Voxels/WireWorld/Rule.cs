using System;
using StarLoop.Script.Voxels.Data;

namespace StarLoop.Script.Voxels.WireWorld;

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

  public bool Equals(Rule other) =>
    Type == other.Type &&
    Min == other.Min &&
    Max == other.Max;

  public override bool Equals(object obj) =>
    obj is Rule other && Equals(other);

  public override int GetHashCode() =>
    HashCode.Combine(Type, Min, Max);

  public static bool operator ==(Rule left, Rule right) =>
    left.Equals(right);

  public static bool operator !=(Rule left, Rule right) =>
    !(left == right);
}