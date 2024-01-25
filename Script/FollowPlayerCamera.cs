using Godot;

public partial class FollowPlayerCamera : Camera3D
{
    private Vector3 _mainCameraStartingPos;
    private Vector3 _myStartingPos;
    [Export] public Camera3D MainCamera;
    private Vector3 _relativePos => MainCamera.GlobalPosition - _mainCameraStartingPos;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _mainCameraStartingPos = MainCamera.GlobalPosition;
        _myStartingPos = GlobalPosition;
    }


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Get the global rotation of the main camera
        var globalRotation = MainCamera.GlobalRotation;

        // Negate the pitch (x component of the rotation vector)
        globalRotation.Y = -globalRotation.Y;
        globalRotation.Z += Mathf.Pi;

        // Set the adjusted rotation to the GlobalRotation of the skybox camera
        GlobalRotation = globalRotation;
        GlobalPosition = _myStartingPos - new Vector3(_relativePos.X, -_relativePos.Y, -_relativePos.Z);
    }
}