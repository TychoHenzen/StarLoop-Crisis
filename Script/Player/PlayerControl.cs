using A_Chat_Of_Goblins.Scripts.Extensions;
using A_Chat_Of_Goblins.Scripts.Player;
using Godot;

namespace A_Chat_Of_Goblins;

public partial class PlayerControl : RigidBody3D
{
    private Camera3D _mainCamera;
    private CameraViewer _view;
    private Raycasting _raycaster;

    private PlayerWalk _walk;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AddToGroup("Player");
        Input.MouseMode = Input.MouseModeEnum.Captured;
        _walk = this.FirstChild<PlayerWalk>();
        _view = this.FirstChild<CameraViewer>();
        _raycaster = this.FirstChild<Raycasting>();
        _mainCamera = this.FirstChild<Camera3D>(true);
    }

    public override void _IntegrateForces(PhysicsDirectBodyState3D state)
    {
        _walk.HandleInputs(state, this);
    }

    public override void _Input(InputEvent toHandle)
    {
        switch (toHandle)
        {
            case InputEventMouseMotion eventMouseMotion:
                _view.HandleMouseInput(eventMouseMotion, this, _mainCamera);
                break;
            case InputEventMouseButton eventMouseButton:

                if (eventMouseButton.Pressed)
                {
                    Input.MouseMode = Input.MouseModeEnum.Captured;
                }

                break;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        GD.Print($"Looking at: {_raycaster.ChunkVoxel} - {_raycaster.VoxelChunk}");
        if (Input.IsPhysicalKeyPressed(Key.Escape))
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
    }
}