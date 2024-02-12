using System.Collections.Generic;
using System.Linq;
using Godot;

namespace StarLoop.Script.Voxels.Helpers;

public static class VisibilityTester
{
  public static bool AnyVoxelsVisible(IEnumerable<Vector3> dirty, Viewport mainViewport)
  {
    var camera = mainViewport.GetCamera3D();
    var spaceState = mainViewport.World3D.DirectSpaceState;

    var entries = dirty.Where(camera.IsPositionInFrustum)
      .OrderBy(vector3 => -vector3.DistanceSquaredTo(camera.GlobalPosition));

    foreach (var candidate in entries)
    {
      var result = spaceState
        .IntersectRay(new PhysicsRayQueryParameters3D
        {
          From = camera.GlobalPosition,
          To = candidate
        });
      if (result.Count <= 0) continue;

      var pos = (Vector3)result["position"];
      if (pos.DistanceSquaredTo(candidate) < 1)
        return true;
    }

    return false;
  }
}