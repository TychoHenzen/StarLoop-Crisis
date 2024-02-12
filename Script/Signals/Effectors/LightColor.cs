using Godot;

namespace StarLoop.Script.Signals.Effectors;

public sealed partial class LightColor : OmniLight3D
{
  [Export] private Color _high;
  [Export] private Color _low;

  private float _value;

  public void SetValue(float value)
  {
    _value = value;
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
    LightColor = _low.Lerp(_high, _value);
  }
}