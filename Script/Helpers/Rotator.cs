using Godot;

namespace StarLoop.Script;

public partial class Rotator : CsgTorus3D
{
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        RotateZ((float)delta * 2);
    }
}