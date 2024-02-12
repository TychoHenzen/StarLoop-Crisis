using System;
using System.Collections.Generic;
using Godot;

namespace StarLoop.Script.TimeLoop;

public sealed partial class ScreenshotStack : Node
{
  [Signal] public delegate void ResetGameEventHandler(double value);
  [Signal] public delegate void TweenTickEventHandler(double value);

  [Export] private TextureRect _screenshotOverlay;

  private const int _totalDuration = 150; // 300 seconds for 5 minutes
  private const float _startSpeed = 1f; // How much to decrease the duration each time
  private const float _speedIncreaseFactor = 0.05f; // How much to decrease the duration each time
  private readonly Stack<ImageTexture> _screenshots = new();
  private float _playbackSpeed; // Initial duration for each screenshot in seconds
  private Timer _playbackTimer;
  private int _screenshotCount;
  private Timer _screenshotTimer;

  private void OnTimerTimeout(float tick)
  {
    if (_screenshots.Count < tick * _totalDuration)
    {
      SaveScreenshot();
      _screenshotCount++;
    }
    else if (_screenshots.Count == _totalDuration && _playbackTimer == null)
    {
      PlayScreenshotsInReverse();
    }
  }

  public ImageTexture CaptureScreenshot()
  {
    var image = GetViewport().GetTexture().GetImage();
    var texture = new ImageTexture();
    texture.SetImage(image);

    return texture;
  }

  public void SaveScreenshot()
  {
    var screenshot = CaptureScreenshot();
    _screenshots.Push(screenshot);
  }

  public void PlayScreenshotsInReverse()
  {
    _playbackSpeed = _startSpeed;
    _playbackTimer = new Timer();
    AddChild(_playbackTimer);
    _playbackTimer.Connect(Timer.SignalName.Timeout, Callable.From(OnPlaybackTimerTimeout));
    _playbackTimer.Start(_playbackSpeed);
    DisplayScreenshot(_screenshots.Peek());
  }

  private void OnPlaybackTimerTimeout()
  {
    if (_screenshots.Count > 0)
    {
      DisplayScreenshot(_screenshots.Pop()); // Method to display screenshot in TextureRect

      // Adjust the playback speed
      _playbackSpeed -= _speedIncreaseFactor;
      _playbackSpeed =
        Math.Max(_playbackSpeed, 0.05f); // Set a minimum speed to prevent it from becoming too fast
      _playbackTimer.Start(_playbackSpeed);
    }
    else
    {
      _playbackTimer.Stop();
      GetTree().ReloadCurrentScene();
      _screenshotOverlay.Texture = null;
      // Optionally, trigger an event to indicate playback completion
    }
  }

  private void DisplayScreenshot(ImageTexture screenshot)
  {
    // Assuming you have a TextureRect named screenshotOverlay
    _screenshotOverlay.Texture = screenshot;
  }
}