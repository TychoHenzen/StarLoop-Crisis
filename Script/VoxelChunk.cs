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
	public byte[] Voxels = new byte[VoxelBuilder.ChunkVoxels];

	private CollisionShape3D collider;
	private int coordX;
	private int coordY;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		collider = GetParent<CollisionShape3D>();
		coordX = (int)(GlobalPosition.X / 8);
		coordY = (int)(GlobalPosition.Y / 8);
		GD.Print("Starting");
		var file = FileAccess.Open($"res://voxels/Chunk_{coordX}_{coordY}.txt",
			FileAccess.ModeFlags.Read);
		if (file == null)
		{
			GD.PrintErr("Failed to open file: " +$"res://voxels/Chunk_{coordX}_{coordY}.txt");
			return;
		}

		while (!file.EofReached())
		{
			string line = file.GetLine();
			if(line.Contains('#') || line.Trim().Length == 0) continue;
			string[] values = line.Split(' ');
			int x = int.Parse(values[0])+ 16;
			int y = int.Parse(values[1])+ 16;
			int z = int.Parse(values[2]);
			int color = int.Parse(values[3], NumberStyles.HexNumber);
			var index = VoxelBuilder.index(y , z , x);
			Voxels[index] = HexConvert.converter[color];
		}

		file.Close();
		GD.Print("Set values");
		Mesh = VoxelBuilder.BuildMesh(Voxels);
		collider.Shape = VoxelBuilder.BuildShape(Voxels);
		GD.Print("Built mesh");
	}
}
