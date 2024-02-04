using Godot;

public partial class AudioTween : Node
{
    [Signal]
    public delegate void TweenTickEventHandler(double value);

    [Export] private AudioStreamPlayer audio;

    public ulong LastReset;

    public override void _Process(double delta)
    {
        // var msSinceStart = Time.Singleton.GetTicksMsec() - LastReset;
        var fractionOfFiveMinutes = audio.GetPlaybackPosition() / audio.Stream.GetLength();
        EmitSignal(SignalName.TweenTick, fractionOfFiveMinutes);
    }

    public override void _Ready()
    {
        LastReset = Time.Singleton.GetTicksMsec();
    }
}