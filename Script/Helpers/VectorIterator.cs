#region

using System;
using Godot;

#endregion

namespace StarLoop.Script.Helpers;

public static class VectorIterator
{
  public static void ForUntil(this Vector3I min, Vector3I max, Func<Vector3I, bool> predicate)
  {
    for (var pos = min; pos.X < max.X; pos.X++)
    {
      for (pos.Y = min.Y; pos.Y < max.Y; pos.Y++)
      {
        for (pos.Z = min.Z; pos.Z < max.Z; pos.Z++)
        {
          if (!predicate(pos))
            return;
        }
      }
    }
  }

  public static void ForEachI(this Vector3I min, Vector3I max, Action<Vector3I> predicate)
  {
    for (var pos = min; pos.X < max.X; pos.X++)
    {
      for (pos.Y = min.Y; pos.Y < max.Y; pos.Y++)
      {
        for (pos.Z = min.Z; pos.Z < max.Z; pos.Z++)
        {
          predicate(pos);
        }
      }
    }
  }

  public static void ForEach(this Vector3I min, Vector3I max, Action<Vector3> predicate)
  {
    for (var pos = min; pos.X < max.X; pos.X++)
    {
      for (pos.Y = min.Y; pos.Y < max.Y; pos.Y++)
      {
        for (pos.Z = min.Z; pos.Z < max.Z; pos.Z++)
        {
          predicate(pos);
        }
      }
    }
  }
}