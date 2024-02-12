using Godot;

namespace StarLoop.Script.Helpers;

public sealed partial class SelfDestruct : Node
{
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    QueueFree();
  }
}