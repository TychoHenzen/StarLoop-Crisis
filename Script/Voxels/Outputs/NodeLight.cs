#region

using Godot;
using StarLoop.Script.Helpers;
using StarLoop.Script.Voxels.Data;
using VoxelController = StarLoop.Script.Voxels.Chunks.VoxelController;

#endregion

namespace StarLoop.Script.Voxels.Outputs;

public sealed partial class NodeLight : OmniLight3D
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
    if (currentVoxel == Cell.Lamp1 ||
        currentVoxel == Cell.Lamp2 ||
        currentVoxel == Cell.Lamp3 ||
        currentVoxel == Cell.Lamp4) return;

    FindLampNearby();
    GlobalPosition = (_targetVoxel + Vector3.One / 2) / VoxelConstants.VoxelScalar;
  }

  private void FindLampNearby()
  {
    new Vector3I(-3, -3, -3).ForUntil(new Vector3I(3, 3, 3), offset =>
    {
      var currentVoxel = (Cell)_ctrl.GetVoxel(_targetVoxel + offset);
      if (currentVoxel != Cell.Lamp1 && currentVoxel != Cell.Lamp2 && currentVoxel != Cell.Lamp3 &&
          currentVoxel != Cell.Lamp4) return false;

      _targetVoxel += offset;
      return true;
    });
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
    var currentVoxel = _ctrl.GetVoxel(_targetVoxel);
    LightEnergy = (Cell)currentVoxel switch
    {
      Cell.Lamp1 => 0,
      Cell.Lamp2 => 0.25f,
      Cell.Lamp3 => 0.75f,
      Cell.Lamp4 => 1f,
      _ => LightEnergy
    };
  }
}