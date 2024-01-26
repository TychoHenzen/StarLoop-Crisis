using Godot;

public partial class SlowLerp : CsgSphere3D
{
    [Signal]
    public delegate void TweenTickEventHandler(double value);

    public ulong LastReset;

    public override void _Process(double delta)
    {
        var msSinceStart = Time.Singleton.GetTicksMsec() - LastReset;
        var fractionOfFiveMinutes = msSinceStart / (1000.0 * 60 * 5);
        EmitSignal(SignalName.TweenTick, fractionOfFiveMinutes);
    }

    public void Reset()
    {
        LastReset = Time.Singleton.GetTicksMsec();
    }
}