using System;
using Godot;
using FileAccess = Godot.FileAccess;

namespace StarLoop.Script.Voxels;

[Tool]
public partial class VoxelChunk : MeshInstance3D
{
	public byte[] Voxels;
	public bool Dirty;

	private CollisionShape3D _collider;
	private ArrayMesh _myMesh;
	private Vector3 _chunkPos;
	private string _hash = "" ;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_collider = GetParent<CollisionShape3D>();
		_chunkPos = (GlobalPosition / (VoxelConstants.VoxelScalar/2f)).Floor();
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
		Voxels = ChunkLoader.LoadGoxFile(FileAccess.Open($"res://voxels/Chunk_{_chunkPos.X}_{_chunkPos.Y}_{_chunkPos.Z}.gox",
			FileAccess.ModeFlags.Read));
		Mesh = _myMesh = VoxelBuilder.BuildMesh(Voxels);
		VoxelBuilder.UpdateMeshTexture(_myMesh, Voxels);
		if (Engine.IsEditorHint()) return;
		
		_collider.Shape = VoxelBuilder.BuildShape(Voxels);
		GD.Print($"{DateTime.Now:O} Registering {_chunkPos}");
		GetParent().GetParent().GetParent<VoxelController>().Register(_chunkPos, this);
	}

	public void SetVoxel(Vector3 pos, byte newByte)
	{
		Voxels[VoxelConstants.Index((int)pos.X, (int)pos.Y, (int)pos.Z)] = newByte;
		Mesh = _myMesh = VoxelBuilder.BuildMesh(Voxels);
		VoxelBuilder.UpdateMeshTexture(_myMesh, Voxels);
		_collider.Shape = VoxelBuilder.BuildShape(Voxels);
	}

	public bool Redraw()
	{
		if (!Dirty) return false;
		
		VoxelBuilder.UpdateMeshTexture(_myMesh, Voxels);
		Dirty = false;
		return true;
	}
}