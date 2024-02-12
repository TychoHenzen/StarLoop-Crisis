#region

using System;
using System.Collections.Generic;
using Godot;
using Array = Godot.Collections.Array;

#endregion

namespace StarLoop.Script.Voxels;

[Tool]
public partial class VoxelChunk : MeshInstance3D
{
    private const string ShaderPath = "res://Shaders/VoxelMapping.gdshader";
    private Array _arrays;
    [Export] private Vector3I _chunkPos;

    private CollisionShape3D _collider;
    private string _hash = "";
    private ImageTexture _mappingTexture;
    private ShaderMaterial _myMat;
    private ArrayMesh _myMesh;
    private Image _renderTarget;
    private Vector2[] _uvs;

    [Export] public Texture2D TileSet;
    public List<Vector3> Dirty { get; } = new();
    public byte[] Voxels { get; private set; } = System.Array.Empty<byte>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _collider = GetParent<CollisionShape3D>();
        _chunkPos = (Vector3I)(GlobalPosition / (VoxelConstants.VoxelScalar / 2f)).Floor();
        _arrays = new Array();
        _arrays.Resize((int)Mesh.ArrayType.Max);
        _myMesh = new ArrayMesh();
        Mesh = _myMesh;
        LoadChunk();
    }

    public void LoadIfChanged()
    {
        if (!Engine.IsEditorHint()) return;

        var newHash = FileAccess.GetSha256($"res://voxels/Chunk_{_chunkPos.X}_{_chunkPos.Y}_{_chunkPos.Z}.gox");
        if (newHash == _hash) return;

        LoadChunk();
        _hash = newHash;
    }

    public void LoadChunk()
    {
        Mesh = _myMesh;
        Voxels = ChunkLoader.LoadGoxFile(FileAccess.Open(
            $"res://voxels/Chunk_{_chunkPos.X}_{_chunkPos.Y}_{_chunkPos.Z}.gox",
            FileAccess.ModeFlags.Read));
        VoxelBuilder.BuildMesh(_arrays, Voxels);
        _uvs = _arrays[(int)Mesh.ArrayType.Vertex].AsVector2Array();
        VoxelBuilder.UpdateMeshTexture(_arrays, _uvs, Voxels);
        _myMesh.ClearSurfaces();
        _myMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, _arrays);

        _renderTarget = new Image();
        _renderTarget.SetData(
            VoxelConstants.TextureMapSize,
            VoxelConstants.TextureMapSize,
            false,
            Image.Format.Rgba8,
            new byte[VoxelConstants.TextureMapSize * VoxelConstants.TextureMapSize * 4]
        );

        Vector3I.Zero.ForUntil(VoxelConstants.VoxelMax, pos =>
        {
            var index = VoxelConstants.Index(pos);
            if (Voxels[index] == 0) return true;
            var uv = VoxelBuilder.VoxelUv(index);

            _renderTarget.SetPixelv(uv,
                Color.Color8(Voxels[index], 0, 0)
            );
            return true;
        });
        _mappingTexture = new ImageTexture();
        _mappingTexture.SetImage(_renderTarget);

        _myMat = new ShaderMaterial();
        _myMat.Shader = GD.Load<Shader>(ShaderPath);
        _myMat.SetShaderParameter("tiles_texture", TileSet);
        _myMat.SetShaderParameter("mapping_texture", _mappingTexture);
        SetSurfaceOverrideMaterial(0, _myMat);


        if (Engine.IsEditorHint()) return;

        _collider.Shape = VoxelBuilder.BuildShape(Voxels);
        GD.Print($"{DateTime.Now:O} Registering {_chunkPos}");
        GetParent().GetParent().GetParent<VoxelController>().Register(_chunkPos, this);
    }

    public void SetVoxel(Vector3I pos, byte newByte)
    {
        Voxels[VoxelConstants.Index(pos)] = newByte;
        VoxelBuilder.BuildMesh(_arrays, Voxels);

        _uvs = _arrays[(int)Mesh.ArrayType.Vertex].AsVector2Array();
        VoxelBuilder.UpdateMeshTexture(_arrays, _uvs, Voxels);
        _myMesh.ClearSurfaces();
        _myMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, _arrays);
        _collider.Shape = VoxelBuilder.BuildShape(Voxels);
    }

    public bool Redraw()
    {
        if (Dirty.Count == 0) return false;

        if (!VisibilityTester.AnyVoxelsVisible(Dirty, GetViewport())) return false;

        VoxelBuilder.UpdateMeshTexture(_arrays, _uvs, Voxels);
        _myMesh.ClearSurfaces();
        _myMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, _arrays);
        Dirty.Clear();
        return true;
    }


    public void ClearChunk()
    {
        _myMesh.ClearSurfaces();
        Mesh = null;
    }
}