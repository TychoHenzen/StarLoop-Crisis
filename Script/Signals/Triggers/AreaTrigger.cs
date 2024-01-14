using Godot;

namespace A_Chat_Of_Goblins.Scripts.Tweening.Triggers;

public partial class AreaTrigger : Area3D
{
	[Signal]
	public delegate void TriggerEnteredEventHandler();
	[Signal]
	public delegate void TriggerExitedEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Connect(Area3D.SignalName.BodyEntered, Callable.From<Node>(OnBodyEntered));
		Connect(Area3D.SignalName.BodyExited, Callable.From<Node>(OnBodyExited));
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