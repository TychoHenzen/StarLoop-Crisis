using Godot;
using Godot.Collections;

namespace A_Chat_Of_Goblins.Scripts.Player;

public partial class PlayerWalk : Node
{
    private const float BreakingForce = -0.5f;
    private bool _isGrounded;
    [Export] public float Damp = 10;
    [Export] public float JumpForce = 3;
    [Export] public float MoveSpeed = 10;
    [Export] public float SprintSpeed = 20;

    public void HandleInputs(PhysicsDirectBodyState3D state, RigidBody3D mainCamera)
    {
        var forward = -mainCamera.GlobalTransform.Basis.Z.Normalized();
        var right = mainCamera.GlobalTransform.Basis.X.Normalized();
        var currentVelocity = state.LinearVelocity;
        var targetVelocity = Vector3.Zero;

        // Calculate the target velocity based on input
        if (Input.IsActionPressed("move_forward"))
            targetVelocity += forward * MoveSpeed;
        if (Input.IsActionPressed("move_backward"))
            targetVelocity -= forward * MoveSpeed;
        if (Input.IsActionPressed("move_right"))
            targetVelocity += right * MoveSpeed;
        if (Input.IsActionPressed("move_left"))
            targetVelocity -= right * MoveSpeed;

        // Apply sprint speed if sprinting
        if (Input.IsActionPressed("sprint"))
            targetVelocity *= SprintSpeed / MoveSpeed;

        // Extract the current forward and lateral velocities
        var currentForwardVelocity = forward * forward.Dot(currentVelocity);
        var currentLateralVelocity = right * right.Dot(currentVelocity);

        // Apply a more immediate change in direction if the input is opposite to the current velocity
        if (Input.IsActionPressed("move_forward") && currentForwardVelocity.Dot(forward) < 0 ||
            Input.IsActionPressed("move_backward") && currentForwardVelocity.Dot(forward) > 0)
        {
            currentForwardVelocity *= BreakingForce; 
        }
        if (Input.IsActionPressed("move_right") && currentLateralVelocity.Dot(right) < 0 ||
            Input.IsActionPressed("move_left") && currentLateralVelocity.Dot(right) > 0)
        {
            currentLateralVelocity *= BreakingForce; 
        }
        // Combine the adjusted velocities
        var adjustedVelocity = currentForwardVelocity + currentLateralVelocity;
        adjustedVelocity.Y = currentVelocity.Y; // Maintain the Y velocity including gravity
        var finalVelocity = adjustedVelocity.Lerp(targetVelocity, Damp * state.Step);
        state.LinearVelocity = new Vector3(finalVelocity.X, state.LinearVelocity.Y, finalVelocity.Z);

        // Ground check and jumping
        _isGrounded = IsOnFloor(state);
        if (_isGrounded && Input.IsActionJustPressed("jump"))
        {
            // Override the Y velocity for jumping
            state.LinearVelocity = new Vector3(state.LinearVelocity.X, JumpForce, state.LinearVelocity.Z);
        }
    }


    private bool IsOnFloor(PhysicsDirectBodyState3D state)
    {
        var rayStart = state.Transform.Origin;
        var rayEnd = rayStart + Vector3.Down * 1.0f; // Length of the ray, adjust as needed

        // Perform the raycast
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

        // Check if the raycast hit anything
        return result.Count > 0;
    }
}