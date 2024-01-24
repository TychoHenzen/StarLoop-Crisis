namespace StarLoop.Script.Voxels;

[Tool]
public partial class VoxelChunk : MeshInstance3D
{
    private Godot.Collections.Array _arrays;
    [Export] private Vector3I _chunkPos;

    private CollisionShape3D _collider;
    private string _hash = "";
    private ArrayMesh _myMesh;
    private Vector2[] _uvs;
    public bool Dirty { get; set; } = true;
    public byte[] Voxels { get; private set; } = Array.Empty<byte>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _collider = GetParent<CollisionShape3D>();
        _chunkPos = (Vector3I)(GlobalPosition / (VoxelConstants.VoxelScalar / 2f)).Floor();
        _arrays = new Godot.Collections.Array();
        _arrays.Resize((int)Mesh.ArrayType.Max);
        _myMesh = new ArrayMesh();
        Mesh = _myMesh;
        LoadChunk();
    }

    public void LoadIfChanged()
    {
        if (!Engine.IsEditorHint()) return;
        if (!FileAccess.FileExists($"res://voxels/Chunk_{_chunkPos.X}_{_chunkPos.Y}_{_chunkPos.Z}.gox"))
        {
            var read = FileAccess.Open($"res://voxels/Chunk_0_0_0.gox",
                FileAccess.ModeFlags.Read);
            var write = FileAccess.Open($"res://voxels/Chunk_{_chunkPos.X}_{_chunkPos.Y}_{_chunkPos.Z}.gox",
                FileAccess.ModeFlags.Write);
            write.StoreBuffer(read.GetBuffer((long)read.GetLength()));
            read.Close();
            write.Close();
        }

        var newHash = FileAccess.GetSha256($"res://voxels/Chunk_{_chunkPos.X}_{_chunkPos.Y}_{_chunkPos.Z}.gox");
        if (newHash == _hash) return;

        LoadChunk();
        _hash = newHash;
    }

    public void LoadChunk()
    {
        Voxels = ChunkLoader.LoadGoxFile(FileAccess.Open(
            $"res://voxels/Chunk_{_chunkPos.X}_{_chunkPos.Y}_{_chunkPos.Z}.gox",
            FileAccess.ModeFlags.Read));
        VoxelBuilder.BuildMesh(_arrays, Voxels);
        _uvs = _arrays[(int)Mesh.ArrayType.Vertex].AsVector2Array();
        VoxelBuilder.UpdateMeshTexture(_arrays, _uvs, Voxels);
        _myMesh.ClearSurfaces();
        _myMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, _arrays);
        if (Engine.IsEditorHint()) return;

        _collider.Shape = VoxelBuilder.BuildShape(Voxels);
        GD.Print($"{DateTime.Now:O} Registering {_chunkPos}");
        GetParent().GetParent().GetParent<VoxelController>().Register(_chunkPos, this);
    }

    public void SetVoxel(Vector3 pos, byte newByte)
    {
        Voxels[VoxelConstants.Index((int)pos.X, (int)pos.Y, (int)pos.Z)] = newByte;
        VoxelBuilder.BuildMesh(_arrays, Voxels);

        _uvs = _arrays[(int)Mesh.ArrayType.Vertex].AsVector2Array();
        VoxelBuilder.UpdateMeshTexture(_arrays, _uvs, Voxels);
        _myMesh.ClearSurfaces();
        _myMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, _arrays);
        _collider.Shape = VoxelBuilder.BuildShape(Voxels);
    }

    public bool Redraw()
    {
        if (!Dirty) return false;

        VoxelBuilder.UpdateMeshTexture(_arrays, _uvs, Voxels);
        _myMesh.ClearSurfaces();
        _myMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, _arrays);
        Dirty = false;
        return true;
    }

    public void ClearChunk()
    {
        Mesh = null;
        _myMesh = null;
    }
}