using System.Threading.Tasks;
using Godot;

namespace StarLoop.Script.Signals.Mechanisms;

public sealed partial class Tweener : Node
{
  [Signal] public delegate void TweenTickEventHandler(double value);

  [ExportGroup("Forward")]
  [Export] private double _beforeMoveTime = 1;
  [Export] private double _duration = 1;
  [Export] private Tween.EaseType _easeIn;
  [Export] private Tween.TransitionType _transitionIn;
  [ExportGroup("Backwards")]
  [Export] private bool _oneWay;
  [Export] private double _waitOpenTime = 1;
  [Export] private double _returnDuration = 1;
  [Export] private double _afterMoveTime = 1;
  [Export] private Tween.EaseType _easeOut;
  [Export] private Tween.TransitionType _transitionOut;

  private Tween _currentTween;
  private bool _tweenActive;

  public async Task StartTween()
  {
    if (_tweenActive) return;

    _tweenActive = true;
    try
    {
      _currentTween = CreateTween();
      _currentTween.TweenMethod(Callable.From<float>(TweenStep), 0f, 1f, _duration)
        .SetDelay(_beforeMoveTime)
        .SetEase(_easeIn)
        .SetTrans(_transitionIn);
      await ToSignal(_currentTween, Tween.SignalName.Finished);
      if (_oneWay) return;

      _currentTween = CreateTween();
      _currentTween.TweenMethod(Callable.From<float>(TweenStep), 1f, 0f, _returnDuration)
        .SetDelay(_waitOpenTime)
        .SetEase(_easeOut)
        .SetTrans(_transitionOut);
      await ToSignal(_currentTween, Tween.SignalName.Finished);
      await ToSignal(GetTree().CreateTimer(_afterMoveTime), SceneTreeTimer.SignalName.Timeout);
    }
    finally
    {
      _tweenActive = false;
    }
  }

  public void StopTween()
  {
    GD.Print("Stopping tween");
    _tweenActive = false;
    _currentTween.Stop();
  }

  private void TweenStep(float value)
  {
    if (_tweenActive) EmitSignal(SignalName.TweenTick, value);
  }
}