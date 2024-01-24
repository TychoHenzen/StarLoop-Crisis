using Godot;

namespace StarLoop.Script.DebugCamera;

public partial class CameraView : Node
{
    [Export] private float _sensitivityX = 0.01f;
    [Export] private float _sensitivityY = 0.02f;

    public void HandleMouseInput(InputEventMouseMotion eventMouseMotion, Camera3D mainCamera)
    {
        var mouseMotion = eventMouseMotion.Relative;

        // Calculate the new Y rotation (yaw) for the camera
        var newRotationY = mainCamera.RotationDegrees.Y - mouseMotion.X * _sensitivityX;

        // Calculate the new X rotation (pitch) for the camera
        var newRotationX = mainCamera.RotationDegrees.X - mouseMotion.Y * _sensitivityY;
        newRotationX = Mathf.Clamp(newRotationX, -90, 90); // Clamp to avoid flipping

        // Update camera rotation
        mainCamera.RotationDegrees = new Vector3(newRotationX, newRotationY, mainCamera.RotationDegrees.Z);
    }
}