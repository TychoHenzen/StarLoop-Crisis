#region

using Godot;

#endregion

namespace StarLoop.Script.Skybox;

public partial class FollowPlayerCamera : Camera3D
{
    [Export] private Camera3D _mainCamera;
    private Vector3 _mainCameraStartingPos;
    private Vector3 _myStartingPos;
    private Vector3 RelativePos => _mainCamera.GlobalPosition - _mainCameraStartingPos;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _mainCameraStartingPos = _mainCamera.GlobalPosition;
        _myStartingPos = Position;
    }


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Get the global rotation of the main camera
        var globalRotation = _mainCamera.GlobalRotation;

        // Negate the pitch (x component of the rotation vector)
        globalRotation.Y = -globalRotation.Y;
        globalRotation.Z += Mathf.Pi;

        // Set the adjusted rotation to the GlobalRotation of the skybox camera
        GlobalRotation = globalRotation;
        Position = _myStartingPos - new Vector3(RelativePos.X, -RelativePos.Y, -RelativePos.Z) / 30f;
    }
}