using Godot;

namespace StarLoop.Script.Player;

public sealed partial class CameraViewer : Node
{
  [Export] private float _sensitivityX = 0.01f;
  [Export] private float _sensitivityY = 0.02f;

  public void HandleMouseInput(InputEventMouseMotion eventMouseMotion, RigidBody3D physicsBody, Camera3D mainCamera)
  {
    var mouseMotion = eventMouseMotion.Relative;

    physicsBody.ApplyTorqueImpulse(Vector3.Up * -mouseMotion.X * _sensitivityX);

    var newRotationX = mainCamera.RotationDegrees.X - mouseMotion.Y * _sensitivityY;
    newRotationX = Mathf.Clamp(newRotationX, -90, 90);
    mainCamera.RotationDegrees = new Vector3(newRotationX, mainCamera.RotationDegrees.Y,
      mainCamera.RotationDegrees.Z);
  }
}