using Godot;
using System;

public partial class ToolbarItem : Node
{
	[Export] public TextureRect tex;
	[Export] public byte selected;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		tex.Set("region_enabled", true);
		tex.Set("region_rect", true);
	}
}
