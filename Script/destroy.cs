using Godot;
using System;

public partial class destroy : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		QueueFree();
	}
}
