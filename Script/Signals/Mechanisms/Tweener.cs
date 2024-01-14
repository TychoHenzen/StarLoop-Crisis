using Godot;
using System;

public partial class Tweener : Node
{
	
	[Signal]
	public delegate void TweenTickEventHandler(double value);
	[Export] public bool OneWay;
	[ExportGroup("Forward")]
	[Export] public double BeforeMoveTime = 1;
	[Export] public double Duration = 1;
	[Export] public Tween.EaseType EaseIn;
	[Export] public Tween.TransitionType TransitionIn;
	[ExportGroup("Backwards")]
	[Export] public double WaitOpenTime = 1;
	[Export] public double ReturnDuration = 1;
	[Export] public Tween.EaseType EaseOut;
	[Export] public Tween.TransitionType TransitionOut;
	[Export] public double AfterMoveTime = 1;
	private bool _tweenActive;
	private Tween _currentTween;
	public async void StartTween()
	{
		if (_tweenActive) return;
		_tweenActive = true;
		try
		{
			_currentTween = CreateTween();
			_currentTween.TweenMethod(Callable.From<float>(TweenStep), 0f, 1f, Duration)
				.SetDelay(BeforeMoveTime)
				.SetEase(EaseIn)
				.SetTrans(TransitionIn);
			await ToSignal(_currentTween, Tween.SignalName.Finished);
			if (OneWay) return;
			
			_currentTween = CreateTween();
			_currentTween.TweenMethod(Callable.From<float>(TweenStep), 1f, 0f, ReturnDuration)
				.SetDelay(WaitOpenTime)
				.SetEase(EaseOut)
				.SetTrans(TransitionOut);
			await ToSignal(_currentTween, Tween.SignalName.Finished);
			await ToSignal(GetTree().CreateTimer(AfterMoveTime), SceneTreeTimer.SignalName.Timeout);
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
		if (_tweenActive)
		{
			EmitSignal(SignalName.TweenTick, value);
		}
	}
}
