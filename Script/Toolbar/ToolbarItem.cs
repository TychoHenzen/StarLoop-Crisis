using Godot;
using StarLoop.Script.Voxels;

namespace StarLoop.Script.Toolbar;

public partial class ToolbarItem : TextureRect
{
    [Export] private byte _currentVal;

    [Export] private Label _label;

    private AtlasTexture _tex;

    public byte Selected
    {
        get => _currentVal;
        set
        {
            _currentVal = value;
            _label.Text = _currentVal.ToString("X");
            var region = new Rect2(VoxelBuilder.GetUvForByte(_currentVal) * _tex.Atlas.GetSize(),
                VoxelConstants.TileSize * _tex.Atlas.GetSize());
            _tex.Region = region;
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _tex = Texture as AtlasTexture;
        Selected = _currentVal;
    }
}