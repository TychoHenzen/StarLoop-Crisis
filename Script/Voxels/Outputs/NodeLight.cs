using Godot;
using StarLoop.Script;
using StarLoop.Script.Voxels;

namespace StarLoop.Scene;

public partial class NodeLight : OmniLight3D
{
    [Export] private VoxelController _ctrl;
    private Vector3I _targetVoxel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var hitPosition = GlobalPosition;
        hitPosition *= VoxelConstants.VoxelScalar;
        hitPosition.X = Mathf.Round(hitPosition.X);
        hitPosition.Y = Mathf.Round(hitPosition.Y);
        hitPosition.Z = Mathf.Round(hitPosition.Z);
        hitPosition /= VoxelConstants.VoxelScalar;
        GlobalPosition = hitPosition;
        _targetVoxel = (Vector3I)(GlobalPosition * VoxelConstants.VoxelScalar - Vector3.One / 2);
        var currentVoxel = (Cell)_ctrl.GetVoxel(_targetVoxel);
        if (currentVoxel != Cell.Lamp1 &&
            currentVoxel != Cell.Lamp2 &&
            currentVoxel != Cell.Lamp3 &&
            currentVoxel != Cell.Lamp4)
        {
            FindLampNearby();
            GlobalPosition = (_targetVoxel + Vector3.One / 2) / VoxelConstants.VoxelScalar;
        }
    }

    private void FindLampNearby()
    {
        for (var offset = new Vector3I(-3, -3, -3); offset.X <= 3; offset.X++)
        for (offset.Y = -3; offset.Y <= 3; offset.Y++)
        for (offset.Z = -3; offset.Z <= 3; offset.Z++)
        {
            var currentVoxel = (Cell)_ctrl.GetVoxel(_targetVoxel + offset);
            if (currentVoxel != Cell.Lamp1 && currentVoxel != Cell.Lamp2 && currentVoxel != Cell.Lamp3 &&
                currentVoxel != Cell.Lamp4) continue;

            _targetVoxel += offset;
            return;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        var currentVoxel = _ctrl.GetVoxel(_targetVoxel);
        switch ((Cell)currentVoxel)
        {
            case Cell.Lamp1:
                LightEnergy = 0;
                break;
            case Cell.Lamp2:
                LightEnergy = 0.25f;
                break;
            case Cell.Lamp3:
                LightEnergy = 0.75f;
                break;
            case Cell.Lamp4:
                LightEnergy = 1f;
                break;
        }
    }
}