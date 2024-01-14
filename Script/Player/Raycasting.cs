using Godot;
using System;
using Godot.Collections;
using StarLoop.Script;

public partial class Raycasting : Node
{
    [Export] public Node3D Highlight;
    [Export] public Node3D Targeted;

    private Vector3 BaseVoxel => Highlight.GlobalPosition * VoxelBuilder.VoxelScalar - Vector3.One / 2;
    public Vector3 ChunkVoxel => new(Mathf.PosMod(BaseVoxel.X, 32),
        Mathf.PosMod(BaseVoxel.Y, 32),
        Mathf.PosMod(BaseVoxel.Z, 32));
    private Vector3 VoxelChunkFloat => BaseVoxel / VoxelBuilder.chunkSize;

    public Vector3 VoxelChunk => new(Mathf.FloorToInt(VoxelChunkFloat.X) + 1,
        Mathf.FloorToInt(VoxelChunkFloat.Y) + 1,
        Mathf.FloorToInt(VoxelChunkFloat.Z) + 1);

    public override void _Process(double delta)
    {
        var camera = GetViewport().GetCamera3D();
        var spaceState = GetViewport().World3D.DirectSpaceState;

        Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
        Vector2 centerScreen = viewportSize / 2;
        Vector3 rayOrigin = camera.GlobalTransform.Origin;
        Vector3 rayDirection = camera.ProjectRayNormal(centerScreen);

        var result = spaceState.IntersectRay(new PhysicsRayQueryParameters3D
        {
            From = rayOrigin,
            To = rayOrigin + rayDirection * 1000,
            Exclude = new Array<Rid> { new(Highlight),new(Targeted) }
        });

        if (result.Count > 0)
        {
            Vector3 hitPosition = (Vector3)result["position"];
            Vector3 hitNormal = (Vector3)result["normal"];
            hitPosition *= VoxelBuilder.VoxelScalar;
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
            hitPosition /= VoxelBuilder.VoxelScalar;
            Targeted.GlobalPosition = hitPosition;
            hitPosition *= VoxelBuilder.VoxelScalar;
            hitPosition -= hitNormal;
            hitPosition /= VoxelBuilder.VoxelScalar;
            Highlight.GlobalPosition = hitPosition;
        }
        else
        {
            Highlight.GlobalPosition = Vector3.Zero;
            Targeted.GlobalPosition = Vector3.Zero;
        }
    }
}