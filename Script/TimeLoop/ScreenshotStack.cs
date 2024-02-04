using System;
using System.Collections.Generic;
using Godot;

namespace StarLoop.Script.TimeLoop;

public partial class ScreenshotStack : Node
{
    [Signal]
    public delegate void ResetGameEventHandler(double value);

    [Signal]
    public delegate void TweenTickEventHandler(double value);

    private const int TotalDuration = 150; // 300 seconds for 5 minutes
    private const float StartSpeed = 1f; // How much to decrease the duration each time
    private const float SpeedIncreaseFactor = 0.05f; // How much to decrease the duration each time
    private readonly Stack<ImageTexture> _screenshots = new();
    private float _playbackSpeed; // Initial duration for each screenshot in seconds
    private Timer _playbackTimer;
    private int _screenshotCount;

    [Export] private TextureRect _screenshotOverlay;

    private Timer _screenshotTimer;


    private void OnTimerTimeout(float tick)
    {
        if (_screenshots.Count < tick * TotalDuration)
        {
            SaveScreenshot();
            _screenshotCount++;
        }
        else if (_screenshots.Count == TotalDuration && _playbackTimer == null)
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
        _playbackSpeed = StartSpeed;
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
            _playbackSpeed -= SpeedIncreaseFactor;
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