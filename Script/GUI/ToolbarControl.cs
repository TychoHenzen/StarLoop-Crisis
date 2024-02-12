using Godot;
using StarLoop.Script.Toolbar;

namespace StarLoop.Script.GUI;

public sealed partial class ToolbarControl : Node
{
  [ExportGroup("Toolbar items")]
  [Export] private ToolbarItem _element1;
  [Export] private ToolbarItem _element2;
  [Export] private ToolbarItem _element3;
  [Export] private ToolbarItem _element4;
  [Export] private ToolbarItem _element5;
  [Export] private ToolbarItem _element6;
  [Export] private ToolbarItem _element7;
  [Export] private ToolbarItem _element8;
  [Export] private ToolbarItem _element9;

  [ExportGroup("Other")]
  [Export] private Node _indicator;

  private int _selectedIndex;

  public byte ActiveVoxel => Elements[Selected].Selected;

  private ToolbarItem[] Elements => new[]
    { _element1, _element2, _element3, _element4, _element5, _element6, _element7, _element8 };

  private int Selected
  {
    get => _selectedIndex;
    set
    {
      _selectedIndex = value;
      _indicator.Reparent(Elements[_selectedIndex], false);
    }
  }

  public override void _Input(InputEvent @event)
  {
    if (@event is not InputEventMouseButton eventMouseButton) return;

    switch (eventMouseButton.ButtonIndex)
    {
      case MouseButton.WheelUp when eventMouseButton.Pressed:
        Elements[Selected].Selected += 1;
        break;
      case MouseButton.WheelDown when eventMouseButton.Pressed:
        Elements[Selected].Selected -= 1;
        break;
    }
  }

  public void Select(byte newValue)
  {
    Elements[Selected].Selected = newValue;
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
    if (Input.IsKeyPressed(Key.Key1))
    {
      Selected = 0;
    }

    if (Input.IsKeyPressed(Key.Key2))
    {
      Selected = 1;
    }

    if (Input.IsKeyPressed(Key.Key3))
    {
      Selected = 2;
    }

    if (Input.IsKeyPressed(Key.Key4))
    {
      Selected = 3;
    }

    if (Input.IsKeyPressed(Key.Key5))
    {
      Selected = 4;
    }

    if (Input.IsKeyPressed(Key.Key6))
    {
      Selected = 5;
    }

    if (Input.IsKeyPressed(Key.Key7))
    {
      Selected = 6;
    }

    if (Input.IsKeyPressed(Key.Key8))
    {
      Selected = 7;
    }

    if (Input.IsKeyPressed(Key.Key9))
    {
      Selected = 8;
    }
  }
}