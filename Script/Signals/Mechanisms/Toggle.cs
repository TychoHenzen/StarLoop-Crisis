using Godot;

namespace StarLoop.Script.Signals.Mechanisms;

public sealed partial class Toggle : Node
{
  [Signal] public delegate void ActiveEventHandler(float delta);
  [Signal] public delegate void InactiveEventHandler(float delta);

  private bool _active;

  public void Enable()
  {
    _active = true;
  }

  public void Disable()
  {
    _active = false;
  }

  public void OnSignal(float delta)
  {
    EmitSignal(_active ? SignalName.Active : SignalName.Inactive, delta);
  }
}