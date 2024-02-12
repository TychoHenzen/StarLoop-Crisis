using Godot;

namespace StarLoop.Script.Helpers;

public sealed partial class Rotator : CsgTorus3D
{
  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
    RotateZ((float)delta * 2);
  }
}