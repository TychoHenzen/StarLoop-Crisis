using Godot;

namespace StarLoop.Script.Signals.Triggers;

public sealed partial class AreaTrigger : Area3D
{
  [Signal] public delegate void TriggerEnteredEventHandler();

  [Signal] public delegate void TriggerExitedEventHandler();

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    Connect(SignalName.BodyEntered, Callable.From<Node>(OnBodyEntered));
    Connect(SignalName.BodyExited, Callable.From<Node>(OnBodyExited));
  }

  private void OnBodyEntered(Node body)
  {
    if (!body.IsInGroup("Player")) return;

    EmitSignal(SignalName.TriggerEntered);
  }

  private void OnBodyExited(Node body)
  {
    if (!body.IsInGroup("Player")) return;

    EmitSignal(SignalName.TriggerExited);
  }
}