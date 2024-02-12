using Godot;

namespace StarLoop.Script.Signals.Triggers;

public sealed partial class Repeater : Node
{
  [Signal] public delegate void OnStepEventHandler(float delta);

  [Export] private bool _active;
  [Export] private bool _onPhysics;

  public void Enable()
  {
    _active = true;
  }

  public void Disable()
  {
    _active = false;
  }

  public override void _PhysicsProcess(double delta)
  {
    if (_onPhysics && _active)
      EmitSignal(SignalName.OnStep, (float)delta);
  }

  public override void _Process(double delta)
  {
    if (!_onPhysics && _active)
      EmitSignal(SignalName.OnStep, (float)delta);
  }
}