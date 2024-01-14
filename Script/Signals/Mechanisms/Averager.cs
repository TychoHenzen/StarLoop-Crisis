using Godot;
using System;

public partial class Averager : Node
{
	private int count = 0;
	private float sum = 0;
	[Signal]
	public delegate void AverageEventHandler(float Average);

	public void Add(float value)
	{
		sum += value;
		count++;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (count == 0)
		{
			sum++;
			count++;
		}
		EmitSignal(SignalName.Average, sum / count);
		sum = 0;
		count = 0;
	}
}
