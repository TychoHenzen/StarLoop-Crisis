using Godot;
using System;
using Godot.Collections;

public partial class ToolbarControl : Node
{
    
    public byte ActiveVoxel => elements[selected].selected;
    [ExportGroup("Toolbar items")]
    [Export] private ToolbarItem element1;
    [Export] private ToolbarItem element2;
    [Export] private ToolbarItem element3;
    [Export] private ToolbarItem element4;
    [Export] private ToolbarItem element5;
    [Export] private ToolbarItem element6;
    [Export] private ToolbarItem element7;
    [Export] private ToolbarItem element8;
    [Export] private ToolbarItem element9;
    
    
    [ExportGroup("Other")]
    [Export] private Node Indicator;
    
    private ToolbarItem[] elements => new []{element1, element2, element3, element4, element5, element6, element7, element8};
    private int _selectedIndex;
    private int selected
    {
        get => _selectedIndex;
        set
        {
            _selectedIndex = value;
            Indicator.Reparent(elements[_selectedIndex], false);
        }
    }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton eventMouseButton)
        {
            if (eventMouseButton.ButtonIndex == MouseButton.WheelUp && eventMouseButton.Pressed)
            {
                elements[selected].selected+=1;
            }

            if (eventMouseButton.ButtonIndex == MouseButton.WheelDown && eventMouseButton.Pressed)
            {
                elements[selected].selected-=1;
            }
            
        }
    }

    public void Select(byte newValue)
    {
        elements[selected].selected = newValue;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.Key1))
        {
            selected = 0;
        }

        if (Input.IsKeyPressed(Key.Key2))
        {
            selected = 1;
        }

        if (Input.IsKeyPressed(Key.Key3))
        {
            selected = 2;
        }

        if (Input.IsKeyPressed(Key.Key4))
        {
            selected = 3;
        }

        if (Input.IsKeyPressed(Key.Key5))
        {
            selected = 4;
        }

        if (Input.IsKeyPressed(Key.Key6))
        {
            selected = 5;
        }

        if (Input.IsKeyPressed(Key.Key7))
        {
            selected = 6;
        }

        if (Input.IsKeyPressed(Key.Key8))
        {
            selected = 7;
        }

        if (Input.IsKeyPressed(Key.Key9))
        {
            selected = 8;
        }
    }
}