using Godot;

namespace StarLoop.Script.Signals.Mechanisms;

public sealed partial class Pusher : Node
{
  [Signal] public delegate void HitMaxEventHandler();
  [Signal] public delegate void HitMinEventHandler();
  [Signal] public delegate void OnStepEventHandler(float level);

  [Export] private float _currentT;
  [Export] private float _lowerSpeed;
  [Export] private float _maxValue;
  [Export] private float _minValue;
  [Export] private float _raiseSpeed;

  public void Raise(float value)
  {
    _currentT += value * _raiseSpeed;
    if (_currentT >= 1)
    {
      EmitSignal(SignalName.HitMax);
      _currentT = 1;
    }

    OnSignal();
  }

  public void Lower(float value)
  {
    _currentT -= value * _lowerSpeed;
    if (_currentT <= 0)
    {
      EmitSignal(SignalName.HitMin);
      _currentT = 0;
    }

    OnSignal();
  }

  public void OnSignal()
  {
    EmitSignal(SignalName.OnStep, Mathf.Lerp(_minValue, _maxValue, _currentT));
  }
}