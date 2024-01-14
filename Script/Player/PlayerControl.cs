using A_Chat_Of_Goblins.Scripts.Extensions;
using A_Chat_Of_Goblins.Scripts.Player;
using Godot;

namespace StarLoop.Script.Player;

public partial class PlayerControl : RigidBody3D
{
    [Export] private VoxelController _voxels;
    private Camera3D _mainCamera;
    private CameraViewer _view;
    private Raycasting _raycaster;
    [Export] private ToolbarControl _toolbar;

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
                    if (eventMouseButton.ButtonIndex == MouseButton.Left)
                    {
                        GD.Print($"Trying to place voxel at chunk {_raycaster.VoxelChunk(true)}, position { _raycaster.ChunkVoxel(true)}");
                        _voxels.SetVoxel(_raycaster.VoxelChunk(true), _raycaster.ChunkVoxel(true), _toolbar.ActiveVoxel);
                    }
                    if (eventMouseButton.ButtonIndex == MouseButton.Right)
                    {
                        _voxels.SetVoxel(_raycaster.VoxelChunk(false), _raycaster.ChunkVoxel(false), 0);
                    }
                    
                    if (eventMouseButton.ButtonIndex == MouseButton.Middle)
                    {
                        _toolbar.Select(_voxels.GetVoxel(_raycaster.VoxelChunk(false), _raycaster.ChunkVoxel(false)));
                    }
                }
            break;
            case InputEventKey keyEvent:

                if (keyEvent.KeyLabel == Key.Escape && keyEvent.Pressed)
                {
                    Input.MouseMode = Input.MouseModeEnum.Visible;
                }
                if (keyEvent.KeyLabel == Key.F12 && keyEvent.Pressed)
                {
                    _voxels.Save();
                }
                break;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}