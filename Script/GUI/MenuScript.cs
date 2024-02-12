#region

using System.Threading.Tasks;
using Godot;

#endregion

namespace StarLoop.Script.GUI;

public sealed partial class MenuScript : ColorRect
{
  [Signal] public delegate void SplashScreensCompletedEventHandler(double value);

  [Export] private TextureRect _splash1;
  [Export] private TextureRect _splash2;
  [Export] private TextureRect _splash3;
  [Export] private PackedScene _target;


  // Called when the node enters the scene tree for the first time.
  public override async void _Ready()
  {
    _splash1.Modulate = new Color(1, 1, 1, 0);
    _splash2.Modulate = new Color(1, 1, 1, 0);
    _splash3.Modulate = new Color(1, 1, 1, 0);
    await StartSplashSequence();
  }


  private async Task StartSplashSequence()
  {
    // Fade in and out each splash screen
    await Fade(_splash1, 1, 0, 1f);
    await FadeInAndOut(_splash2, 2);
    await FadeInAndOut(_splash3, 6);

    // Emit the signal
    GetTree().ChangeSceneToPacked(_target);
  }

  private async Task FadeInAndOut(TextureRect splash, float pause)
  {
    // Duration for each phase (fade in and fade out)

    // Fade in
    await Fade(splash, 0, 1, 1);

    await ToSignal(GetTree().CreateTimer(pause), Timer.SignalName.Timeout);

    // Fade out
    await Fade(splash, 1, 0, 1);
  }

  private async Task Fade(CanvasItem splash,
    float from,
    float to,
    float phaseDuration)
  {
    var elapsedTime = 0.0;
    while (elapsedTime < phaseDuration)
    {
      var alpha = Mathf.Lerp(from, to, elapsedTime / phaseDuration);
      splash.Modulate = new Color(1, 1, 1, (float)alpha);
      await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
      elapsedTime += GetProcessDeltaTime();
    }

    // Ensure it's fully opaque at the end of fade-in
    splash.Modulate = new Color(1, 1, 1, to);
  }
}