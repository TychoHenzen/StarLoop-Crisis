using Godot;

namespace StarLoop.Script.Signals.Mechanisms;

public partial class Averager : Node
{
	private int _count;
	private float _sum;
	[Signal]
	public delegate void AverageEventHandler(float average);

	public void Add(float value)
	{
		_sum += value;
		_count++;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_count == 0)
		{
			_sum++;
			_count++;
		}
		EmitSignal(SignalName.Average, _sum / _count);
		_sum = 0;
		_count = 0;
	}
}