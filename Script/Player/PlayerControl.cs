using System;
using Godot;
using StarLoop.Script.Extensions;

namespace StarLoop.Script.Player;

public sealed partial class PlayerControl : RigidBody3D
{
  private static bool _wireWorldRunning = true;

  [Export] private GUI.ToolbarControl _toolbar;
  [Export] private Voxels.Chunks.VoxelController _voxels;
  [Export] private Voxels.WireWorld.WireWorldController _wireWorld;

  private Camera3D _mainCamera;
  private RayCasting _rayCaster;
  private CameraViewer _view;
  private PlayerWalk _walk;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    AddToGroup("Player");
    Input.MouseMode = Input.MouseModeEnum.Captured;

    _walk = this.FirstChild<PlayerWalk>();
    _view = this.FirstChild<CameraViewer>();
    _rayCaster = this.FirstChild<RayCasting>();
    _mainCamera = this.FirstChild<Camera3D>(true);
  }

  public override void _IntegrateForces(PhysicsDirectBodyState3D state)
  {
    _walk.HandleInputs(state, this);
  }

  public override void _Input(InputEvent @event)
  {
    switch (@event)
    {
      case InputEventMouseMotion eventMouseMotion:
        _view.HandleMouseInput(eventMouseMotion, this, _mainCamera);
        break;
      case InputEventMouseButton eventMouseButton:

        HandleMouse(eventMouseButton);
        break;
      case InputEventKey keyEvent:

        HandleKeys(keyEvent);
        break;
    }
  }

  private void HandleKeys(InputEventKey keyEvent)
  {
    switch (keyEvent.KeyLabel)
    {
      case Key.Escape when keyEvent.Pressed:
        Input.MouseMode = Input.MouseModeEnum.Visible;
        break;
      case Key.Period when keyEvent.Pressed:
        GD.Print("Performing step");
        _wireWorld.NextStep(_voxels);
        break;
      case Key.Comma when keyEvent.Pressed:
        ToggleWireWorld();
        break;
    }
  }

  private static void ToggleWireWorld()
  {
    _wireWorldRunning = !_wireWorldRunning;
  }

  private void HandleMouse(InputEventMouseButton eventMouseButton)
  {
    if (!eventMouseButton.Pressed) return;

    Input.MouseMode = Input.MouseModeEnum.Captured;
    switch (eventMouseButton.ButtonIndex)
    {
      case MouseButton.Left:
      {
        GD.Print($"Trying to place voxel at position {_rayCaster.LookedAt(true)} -> " + $"{Voxels.Chunks.VoxelController.GetChunkPos(_rayCaster.LookedAt(true))} / " +
          $"{Voxels.Chunks.VoxelController.GetVoxelPos((Vector3I)_rayCaster.LookedAt(true))}");

        var target = _rayCaster.LookedAt(true);
        if (Math.Abs(target.X - -0.5) > 0.001f)
          _voxels.SetVoxel((Vector3I)target, _toolbar.ActiveVoxel);
        break;
      }
      case MouseButton.Right:
        _voxels.SetVoxel((Vector3I)_rayCaster.LookedAt(false), 0);
        break;
      case MouseButton.Middle:
        _toolbar.Select(_voxels.GetVoxel((Vector3I)_rayCaster.LookedAt(false)));
        break;
    }
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
    if (_wireWorldRunning)
      _wireWorld.NextStep(_voxels);
  }
}