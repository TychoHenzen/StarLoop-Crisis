using Godot;
using System;
using StarLoop.Script;

public partial class ToolbarItem : TextureRect
{
	[Export]
	private byte currentVal;
	[Export]
	private Label label;
	private AtlasTexture tex;
	public byte selected
	{
		get => currentVal;
		set
		{
			currentVal = value;
			label.Text = currentVal.ToString("X");
			var region =  new Rect2(VoxelBuilder.GetUVForByte(currentVal)*tex.Atlas.GetSize(), VoxelBuilder.TileSize*tex.Atlas.GetSize());
			tex.Region = region;
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tex = Texture as AtlasTexture;
		selected = currentVal;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
