using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Godot.Collections;
using Godot.NativeInterop;
using StarLoop.Script;
using Array = Godot.Collections.Array;
using FileAccess = Godot.FileAccess;

public partial class VoxelChunk : MeshInstance3D
{
	public byte[] Voxels;
	public bool Dirty;

	private CollisionShape3D collider;
	private ArrayMesh myMesh;

	private Vector3 ChunkPos;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		collider = GetParent<CollisionShape3D>();
		ChunkPos = (GlobalPosition / (VoxelBuilder.VoxelScalar/2f)).Floor();
		Voxels = ChunkLoader.LoadFile(ChunkPos);
		Mesh = myMesh = VoxelBuilder.BuildMesh(Voxels);
		VoxelBuilder.UpdateMeshTexture(myMesh, Voxels);
		collider.Shape = VoxelBuilder.BuildShape(Voxels);
		GD.Print($"{DateTime.Now:O}Registering {ChunkPos}");
		GetParent().GetParent().GetParent<VoxelController>().Register(ChunkPos, this);
	}


	public void SetVoxel(Vector3 pos, byte newByte)
	{
		Voxels[VoxelConstants.index((int)pos.X, (int)pos.Y, (int)pos.Z)] = newByte;
		Mesh = myMesh = VoxelBuilder.BuildMesh(Voxels);
		VoxelBuilder.UpdateMeshTexture(myMesh, Voxels);
		collider.Shape = VoxelBuilder.BuildShape(Voxels);
	}

	public bool Redraw()
	{
		if (!Dirty) return false;
		
		VoxelBuilder.UpdateMeshTexture(myMesh, Voxels);
		Dirty = false;
		return true;
	}
}
