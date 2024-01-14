using Godot;

namespace A_Chat_Of_Goblins.Scripts.Player;

public partial class CameraViewer : Node
{
    [Export] public float SensitivityX = 0.01f;
    [Export] public float SensitivityY = 0.02f;

    public void HandleMouseInput(InputEventMouseMotion eventMouseMotion, RigidBody3D physicsBody, Camera3D mainCamera)
    {
        var mouseMotion = eventMouseMotion.Relative;

        physicsBody.ApplyTorqueImpulse(Vector3.Up * -mouseMotion.X * SensitivityX);

        var newRotationX = mainCamera.RotationDegrees.X - mouseMotion.Y * SensitivityY;
        newRotationX = Mathf.Clamp(newRotationX, -90, 90);
        mainCamera.RotationDegrees = new Vector3(newRotationX, mainCamera.RotationDegrees.Y,
            mainCamera.RotationDegrees.Z);
    }
}