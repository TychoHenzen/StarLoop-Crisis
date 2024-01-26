using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace StarLoop.Script.Voxels;

public static class VisibilityTester
{
    public static bool AnyVoxelsVisible(List<Vector3> dirty, Viewport mainViewport)
    {
        var camera = mainViewport.GetCamera3D();
        var spaceState = mainViewport.World3D.DirectSpaceState;

        return (from candidate in dirty
                    .Where(camera.IsPositionInFrustum)
                    .OrderBy(vector3 => vector3.DistanceSquaredTo(camera.GlobalPosition))
                    .Reverse() 
            let result = spaceState
                .IntersectRay(new PhysicsRayQueryParameters3D
                { From = camera.GlobalPosition, 
                    To = candidate }) 
            where result.Count > 0 
            let pos = (Vector3)result["position"] 
            where pos.DistanceSquaredTo(candidate) < 1 
            select candidate)
            .Any();
    }
}