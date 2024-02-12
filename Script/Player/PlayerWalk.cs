using Godot;
using Godot.Collections;

namespace StarLoop.Script.Player;

public sealed partial class PlayerWalk : Node
{
  [Export] private float _damp = 10;
  [Export] private float _jumpForce = 3;
  [Export] private float _moveSpeed = 10;
  [Export] private float _sprintSpeed = 20;

  private bool _isGrounded;
  private const float _brakingForce = -0.5f;

  public void HandleInputs(PhysicsDirectBodyState3D state, RigidBody3D mainCamera)
  {
    var forward = -mainCamera.GlobalTransform.Basis.Z.Normalized();
    var right = mainCamera.GlobalTransform.Basis.X.Normalized();
    var currentVelocity = state.LinearVelocity;
    var targetVelocity = Vector3.Zero;

    targetVelocity = HandleInput(targetVelocity, forward, right);

    // Extract the current forward and lateral velocities
    var currentForwardVelocity = forward * forward.Dot(currentVelocity);
    var currentLateralVelocity = right * right.Dot(currentVelocity);

    // Apply a more immediate change in direction if the input is opposite to the current velocity
    currentForwardVelocity = BrakeVelocity(currentForwardVelocity, forward, right, ref currentLateralVelocity);
    // Combine the adjusted velocities
    Dampening(state, currentForwardVelocity, currentLateralVelocity, currentVelocity, targetVelocity);

    // Ground check and jumping
    _isGrounded = IsOnFloor(state);
    if (_isGrounded && Input.IsActionJustPressed("jump"))
    {
      // Override the Y velocity for jumping
      state.LinearVelocity = new Vector3(state.LinearVelocity.X, _jumpForce, state.LinearVelocity.Z);
    }
  }

  private Vector3 HandleInput(Vector3 targetVelocity, Vector3 forward, Vector3 right)
  {
    // Calculate the target velocity based on input
    if (Input.IsActionPressed("move_forward"))
      targetVelocity += forward * _moveSpeed;
    if (Input.IsActionPressed("move_backward"))
      targetVelocity -= forward * _moveSpeed;
    if (Input.IsActionPressed("move_right"))
      targetVelocity += right * _moveSpeed;
    if (Input.IsActionPressed("move_left"))
      targetVelocity -= right * _moveSpeed;

    // Apply sprint speed if sprinting
    if (Input.IsActionPressed("sprint"))
      targetVelocity *= _sprintSpeed / _moveSpeed;
    return targetVelocity;
  }

  private void Dampening(PhysicsDirectBodyState3D state,
    Vector3 currentForwardVelocity,
    Vector3 currentLateralVelocity,
    Vector3 currentVelocity,
    Vector3 targetVelocity)
  {
    var adjustedVelocity = currentForwardVelocity + currentLateralVelocity;
    adjustedVelocity.Y = currentVelocity.Y; // Maintain the Y velocity including gravity
    var finalVelocity = adjustedVelocity.Lerp(targetVelocity, _damp * state.Step);
    state.LinearVelocity = new Vector3(finalVelocity.X, state.LinearVelocity.Y, finalVelocity.Z);
  }

  private static Vector3 BrakeVelocity(Vector3 currentForwardVelocity,
    Vector3 forward,
    Vector3 right,
    ref Vector3 currentLateralVelocity)
  {
    if ((Input.IsActionPressed("move_forward") && currentForwardVelocity.Dot(forward) < 0) || (Input.IsActionPressed("move_backward") && currentForwardVelocity.Dot(forward) > 0))
      currentForwardVelocity *= _brakingForce;

    if ((Input.IsActionPressed("move_right") && currentLateralVelocity.Dot(right) < 0) || (Input.IsActionPressed("move_left") && currentLateralVelocity.Dot(right) > 0))
      currentLateralVelocity *= _brakingForce;

    return currentForwardVelocity;
  }


  private bool IsOnFloor(PhysicsDirectBodyState3D state)
  {
    var rayStart = state.Transform.Origin;
    var rayEnd = rayStart + Vector3.Down * 1.0f; // Length of the ray, adjust as needed

    // Perform the ray cast
    var parameters = new PhysicsRayQueryParameters3D
    {
      From = rayStart,
      To = rayEnd,
      Exclude = new Array<Rid> { new(this) },
      CollisionMask = 1,
      CollideWithBodies = true,
      HitBackFaces = false
    };
    var result = state.GetSpaceState()
      .IntersectRay(parameters);

    // Check if the ray cast hit anything
    return result.Count > 0;
  }
}