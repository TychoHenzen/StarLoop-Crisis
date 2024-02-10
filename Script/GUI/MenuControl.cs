#region

using Godot;

#endregion

namespace StarLoop.Script.GUI;

public partial class MenuControl : Control
{
    [Export] private TextureRect _menuEmpty;
    [Export] private TextureRect _menuExit;
    [Export] private TextureRect _menuSettings;
    [Export] private TextureRect _menuStart;
    [Export] private AcceptDialog _settingsPopup;
    [Export] private PackedScene _target;

    public void HoverStartEnter()
    {
        _menuEmpty.Visible = false;
        _menuStart.Visible = true;
    }

    public void HoverStartExit()
    {
        _menuEmpty.Visible = true;
        _menuStart.Visible = false;
    }

    public void ClickStart()
    {
        GetTree().ChangeSceneToPacked(_target);
    }

    public void HoverSettingsEnter()
    {
        _menuEmpty.Visible = false;
        _menuSettings.Visible = true;
    }

    public void HoverSettingsExit()
    {
        _menuEmpty.Visible = true;
        _menuSettings.Visible = false;
    }

    public void ClickSettings()
    {
        _settingsPopup.PopupCentered();
    }

    public void HoverStopEnter()
    {
        _menuEmpty.Visible = false;
        _menuExit.Visible = true;
    }

    public void HoverStopExit()
    {
        _menuEmpty.Visible = true;
        _menuExit.Visible = false;
    }

    public void ClickStop()
    {
        GetTree().Quit();
    }
}