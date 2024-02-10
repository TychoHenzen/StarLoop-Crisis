#region

using Godot;

#endregion

namespace StarLoop.Script.Signals.Mechanisms;

public partial class AudioTween : Node
{
    [Signal]
    public delegate void TweenTickEventHandler(double value);

    [Export] private AudioStreamPlayer _audio;

    private ulong _lastReset;

    public override void _Process(double delta)
    {
        var fractionOfFiveMinutes = _audio.GetPlaybackPosition() / _audio.Stream.GetLength();
        EmitSignal(SignalName.TweenTick, fractionOfFiveMinutes);
    }

    public override void _Ready()
    {
        _lastReset = Time.Singleton.GetTicksMsec();
    }
}