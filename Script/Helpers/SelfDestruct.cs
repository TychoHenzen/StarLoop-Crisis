using Godot;

namespace StarLoop.Script;

public partial class SelfDestruct : Node
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        QueueFree();
    }
}