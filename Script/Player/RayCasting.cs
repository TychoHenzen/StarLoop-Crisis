#region

using Godot;
using Godot.Collections;
using StarLoop.Script.Voxels.Data;

#endregion

namespace StarLoop.Script.Player;

public sealed partial class RayCasting : Node
{
    [Export] private Node3D _highlight;

    private PhysicsRayQueryParameters3D _parameters;
    [Export] private Node3D _targeted;

    public override void _Ready()
    {
        _parameters = new PhysicsRayQueryParameters3D
        {
            Exclude = new Array<Rid> { new(_highlight), new(_targeted) }
        };
    }

    public Vector3 LookedAt(bool inFront) =>
        (inFront ? _targeted : _highlight).GlobalPosition * VoxelConstants.VoxelScalar - Vector3.One / 2;

    public override void _Process(double delta)
    {
        var camera = GetViewport().GetCamera3D();
        var spaceState = GetViewport().World3D.DirectSpaceState;

        var viewportSize = GetViewport().GetVisibleRect().Size;
        var centerScreen = viewportSize / 2;
        var rayOrigin = camera.GlobalTransform.Origin;
        var rayDirection = camera.ProjectRayNormal(centerScreen);

        _parameters.From = rayOrigin;
        _parameters.To = rayOrigin + rayDirection * 1000;
        var result = spaceState.IntersectRay(_parameters);

        ParseResults(result);
    }

    private void ParseResults(Dictionary result)
    {
        if (result.Count > 0)
        {
            HandleHit(result);
        }
        else
        {
            _highlight.GlobalPosition = Vector3.Zero;
            _targeted.GlobalPosition = Vector3.Zero;
        }
    }

    private void HandleHit(Dictionary result)
    {
        var hitPosition = (Vector3)result["position"];
        var hitNormal = (Vector3)result["normal"];
        hitPosition *= VoxelConstants.VoxelScalar;
        hitPosition += new Vector3(
            hitNormal.X < 0 ? 0.499f : -0.499f,
            hitNormal.Y < 0 ? 0.499f : -0.499f,
            hitNormal.Z < 0 ? 0.499f : -0.499f);
        hitPosition.X = Mathf.Round(hitPosition.X);
        hitPosition.Y = Mathf.Round(hitPosition.Y);
        hitPosition.Z = Mathf.Round(hitPosition.Z);
        hitPosition += new Vector3(
            hitNormal.X < 0 ? -0.5f : 0.5f,
            hitNormal.Y < 0 ? -0.5f : 0.5f,
            hitNormal.Z < 0 ? -0.5f : 0.5f);
        hitPosition /= VoxelConstants.VoxelScalar;
        _targeted.GlobalPosition = hitPosition;
        hitPosition *= VoxelConstants.VoxelScalar;
        hitPosition -= hitNormal;
        hitPosition /= VoxelConstants.VoxelScalar;
        _highlight.GlobalPosition = hitPosition;
    }
}