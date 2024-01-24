using Godot;

namespace StarLoop.Script.DebugCamera;

public partial class CameraControl : Camera3D
{
    private float _speed = 5.0f; // Camera movement speed
    [Export] private CameraView CameraView { get; } = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured; // Capture the mouse
    }

    public override void _Input(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventMouseMotion eventMouseMotion:
                CameraView.HandleMouseInput(eventMouseMotion, this);
                break;
            case InputEventMouseButton { Pressed: true }:
            {
                Input.MouseMode = Input.MouseModeEnum.Captured;
                break;
            }
        }
    }

// Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.Escape))
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }

        var direction = new Vector3();

        // Forward and backward movement (along the global Z-axis)
        if (Input.IsKeyPressed(Key.W)) direction += Vector3.Forward;
        if (Input.IsKeyPressed(Key.S)) direction += Vector3.Back;

        // Left and right strafing movement (along the global X-axis)
        if (Input.IsKeyPressed(Key.A)) direction += Vector3.Left;
        if (Input.IsKeyPressed(Key.D)) direction += Vector3.Right;

        // Up and down movement (along the global Y-axis)
        if (Input.IsKeyPressed(Key.Space)) direction += Vector3.Up;
        if (Input.IsKeyPressed(Key.Shift)) direction += Vector3.Down;

        // Normalize the direction vector
        direction = direction.Normalized();

        // Apply the movement
        Translate(direction * _speed * (float)delta);
    }
}