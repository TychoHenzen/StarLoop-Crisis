using Godot;
using System;

public partial class TransformEffector : Node3D
{
	private TransformDefinition _start;
	private TransformDefinition _end;

	[Export] public Node3D TransformToMove;
	[Export] public Node3D DestinationMarker;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_start = new TransformDefinition()
		{
			Position = TransformToMove.GlobalPosition,
			Rotation = TransformToMove.GlobalRotation,
			Scale = TransformToMove.Scale
		};
		_end = new TransformDefinition()
		{
			Position = DestinationMarker.GlobalPosition,
			Rotation = DestinationMarker.GlobalRotation,
			Scale = DestinationMarker.Scale
		};
		DestinationMarker.QueueFree();
	}

	public void Tick(float value)
	{
		GD.Print($"Ticking: {value}");
		TransformToMove.GlobalPosition = _start.Position.Lerp(_end.Position, value);
		TransformToMove.GlobalRotation = _start.Rotation.Lerp(_end.Rotation, value);
		TransformToMove.Scale = _start.Scale.Lerp(_end.Scale, value);
		TransformToMove.Orthonormalize();
	}
}
