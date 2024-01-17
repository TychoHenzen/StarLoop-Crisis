using System;
using A_Chat_Of_Goblins.Scripts.Extensions;
using A_Chat_Of_Goblins.Scripts.Player;
using Godot;

namespace StarLoop.Script.Player;

public partial class PlayerControl : RigidBody3D
{
    [Export] private VoxelController _voxels;
    [Export] private WireWorldController _wireWorld;
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
                        GD.Print($"Trying to place voxel at position { _raycaster.LookedAt(true)} -> {VoxelController.GetChunkPos(_raycaster.LookedAt(true))} / {VoxelController.GetVoxelPos(_raycaster.LookedAt(true))}");

                        var target = _raycaster.LookedAt(true);
                        if(Math.Abs(target.X - -0.5) > 0.001f)
                        _voxels.SetVoxel(target, _toolbar.ActiveVoxel);
                    }
                    if (eventMouseButton.ButtonIndex == MouseButton.Right)
                    {
                        _voxels.SetVoxel(_raycaster.LookedAt(false), 0);
                    }
                    
                    if (eventMouseButton.ButtonIndex == MouseButton.Middle)
                    {
                        _toolbar.Select(_voxels.GetVoxel(_raycaster.LookedAt(false)));
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
                    _wireWorld.preSave(_voxels);
                    _voxels.Save();
                }
                
                if (keyEvent.KeyLabel == Key.Period && keyEvent.Pressed)
                {
                    
                    GD.Print($"Performing step");
                    _wireWorld.NextStep(_voxels);
                }
                
                if (keyEvent.KeyLabel == Key.Comma && keyEvent.Pressed)
                {
                    WireWorldRunning = !WireWorldRunning;
                }
                break;
        }
    }

    private static bool WireWorldRunning = false;
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if(WireWorldRunning)
            _wireWorld.NextStep(_voxels);
    }
}