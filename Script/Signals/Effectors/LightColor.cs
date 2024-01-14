using Godot;
using System;

public partial class LightColor : OmniLight3D
{
	[Export]public Color High;
	[Export]public Color Low;

	private float _value;
	public void SetValue(float value)
	{
		_value = value;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		LightColor = Low.Lerp(High, _value);
	}
}
