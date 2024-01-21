using Godot;

namespace StarLoop.Script;

public partial class ToolbarControl : Node
{
    
    public byte ActiveVoxel => Elements[Selected].Selected;
    [ExportGroup("Toolbar items")]
    [Export] private Toolbar.ToolbarItem _element1;
    [Export] private Toolbar.ToolbarItem _element2;
    [Export] private Toolbar.ToolbarItem _element3;
    [Export] private Toolbar.ToolbarItem _element4;
    [Export] private Toolbar.ToolbarItem _element5;
    [Export] private Toolbar.ToolbarItem _element6;
    [Export] private Toolbar.ToolbarItem _element7;
    [Export] private Toolbar.ToolbarItem _element8;
    [Export] private Toolbar.ToolbarItem _element9;
    
    
    [ExportGroup("Other")]
    [Export] private Node _indicator;
    
    private Toolbar.ToolbarItem[] Elements => new []{_element1, _element2, _element3, _element4, _element5, _element6, _element7, _element8};
    private int _selectedIndex;
    private int Selected
    {
        get => _selectedIndex;
        set
        {
            _selectedIndex = value;
            _indicator.Reparent(Elements[_selectedIndex], false);
        }
    }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is not InputEventMouseButton eventMouseButton) return;
        
        switch (eventMouseButton.ButtonIndex)
        {
            case MouseButton.WheelUp when eventMouseButton.Pressed:
                Elements[Selected].Selected+=1;
                break;
            case MouseButton.WheelDown when eventMouseButton.Pressed:
                Elements[Selected].Selected-=1;
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