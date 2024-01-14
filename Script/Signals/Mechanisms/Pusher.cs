using Godot;
using System;

public partial class Pusher : Node
{
    [Signal]
    public delegate void OnStepEventHandler(float level);

    [Signal]
    public delegate void HitMinEventHandler();

    [Signal]
    public delegate void HitMaxEventHandler();

    [Export] public float MaxValue;
    [Export] public float MinValue;

    [Export] public float RaiseSpeed;
    [Export] public float LowerSpeed;

    [Export] public float CurrentT;

    public void Raise(float value)
    {
        CurrentT += value * RaiseSpeed;
        if (!(CurrentT < 1))
        {
            EmitSignal(SignalName.HitMax);
            CurrentT = 1;
        }

        OnSignal();
    }

    public void Lower(float value)
    {
        CurrentT -= value * LowerSpeed;
        if (!(CurrentT > 0))
        {
            EmitSignal(SignalName.HitMin);
            CurrentT = 0;
        }

        OnSignal();
    }

    public void OnSignal()
    {
        EmitSignal(SignalName.OnStep, Mathf.Lerp(MinValue, MaxValue, CurrentT));
    }
}