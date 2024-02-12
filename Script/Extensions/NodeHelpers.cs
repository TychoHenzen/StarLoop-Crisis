#region

using Godot;

#endregion

namespace StarLoop.Script.Extensions;

public static class NodeHelpers
{
  public static T FirstChild<T>(this Node self, bool recursive = false) where T : Node
  {
    foreach (var child in self.GetChildren())
    {
      if (child is T t)
        return t;

      if (!recursive) continue;

      var recurse = child.FirstChild<T>(true);
      if (recurse != null)
        return recurse;
    }

    return null;
  }
}