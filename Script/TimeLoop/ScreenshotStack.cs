using System;
using System.Collections.Generic;
using Godot;

namespace StarLoop.Script.TimeLoop;

public partial class ScreenshotStack : Node
{
    private const int TotalDuration = 300; // 300 seconds for 5 minutes
    private const float StartSpeed = 0.5f; // How much to decrease the duration each time
    private const float SpeedIncreaseFactor = 0.01f; // How much to decrease the duration each time
    private readonly Stack<ImageTexture> _screenshots = new();
    private float _playbackSpeed; // Initial duration for each screenshot in seconds
    private Timer _playbackTimer;
    private int _screenshotCount;

    private Timer _screenshotTimer;

    [Export] private TextureRect ScreenshotOverlay;

    public override void _Ready()
    {
        _screenshotTimer = new Timer();
        AddChild(_screenshotTimer);
        _screenshotTimer.WaitTime = 1.0f; // 1 second interval
        _screenshotTimer.Connect(Timer.SignalName.Timeout, Callable.From(OnTimerTimeout));
        _screenshotTimer.Start();
    }

    private void OnTimerTimeout()
    {
        if (_screenshotCount < TotalDuration)
        {
            SaveScreenshot();
            _screenshotCount++;
        }
        else
        {
            _screenshotTimer.Stop();
            PlayScreenshotsInReverse();
        }
    }

    public ImageTexture CaptureScreenshot()
    {
        var image = GetViewport().GetTexture().GetImage();
        image.FlipY(); // Since the image might be upside down

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
    }

    private void OnPlaybackTimerTimeout()
    {
        if (_screenshots.Count > 0)
        {
            var screenshot = _screenshots.Pop();
            DisplayScreenshot(screenshot); // Method to display screenshot in TextureRect

            // Adjust the playback speed
            _playbackSpeed -= SpeedIncreaseFactor;
            _playbackSpeed = Math.Max(_playbackSpeed, 0.1f); // Set a minimum speed to prevent it from becoming too fast
            _playbackTimer.Start(_playbackSpeed);
        }
        else
        {
            _playbackTimer.Stop();
            ScreenshotOverlay.Texture = null;
            // Optionally, trigger an event to indicate playback completion
        }
    }

    private void DisplayScreenshot(ImageTexture screenshot)
    {
        // Assuming you have a TextureRect named screenshotOverlay
        ScreenshotOverlay.Texture = screenshot;
    }
}