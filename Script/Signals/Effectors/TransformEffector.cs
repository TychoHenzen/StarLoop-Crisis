using Godot;
using StarLoop.Script.Datatypes;

namespace StarLoop.Script.Signals.Effectors;

public partial class TransformEffector : Node3D
{
    [Export] private Node3D _destinationMarker;
    private TransformDefinition _end;
    private TransformDefinition _start;

    [Export] private Node3D _transformToMove;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _start = new TransformDefinition
        {
            Position = _transformToMove.GlobalPosition,
            Rotation = _transformToMove.GlobalRotation,
            Scale = _transformToMove.Scale
        };
        _end = new TransformDefinition
        {
            Position = _destinationMarker.GlobalPosition,
            Rotation = _destinationMarker.GlobalRotation,
            Scale = _destinationMarker.Scale
        };
        _destinationMarker.QueueFree();
    }

    public void Tick(float value)
    {
        value *= value;
        _transformToMove.GlobalPosition = _start.Position.Lerp(_end.Position, value);
        _transformToMove.GlobalRotation = _start.Rotation.Lerp(_end.Rotation, value);
        _transformToMove.Scale = _start.Scale.Lerp(_end.Scale, value);
        _transformToMove.Orthonormalize();
    }
}