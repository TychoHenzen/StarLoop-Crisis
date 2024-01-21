using Godot;

namespace StarLoop.Script.Signals.Triggers;

public partial class Repeater : Node
{
	[Signal]
	public delegate void OnStepEventHandler(float delta);
	
	[Export] public bool OnPhysics;
	[Export] public bool Active;
	public void Enable()
	{
		Active = true;
	}
	public void Disable()
	{
		Active = false;
	}
	public override void _PhysicsProcess(double delta)
	{
		if(OnPhysics && Active)
			EmitSignal(SignalName.OnStep, (float)delta);
	}
	public override void _Process(double delta)
	{
		if(!OnPhysics && Active)
			EmitSignal(SignalName.OnStep, (float)delta);
	}
}