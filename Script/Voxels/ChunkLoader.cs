using Godot;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using StarLoop.Script;
using FileAccess = Godot.FileAccess;

public static class ChunkLoader
{
    public static byte[] LoadFile(Vector3 chunkPos)
    {
        byte[] voxels = new byte[VoxelBuilder.ChunkVoxels];
        
        var file = FileAccess.Open($"Chunk_{chunkPos.X}_{chunkPos.Y}_{chunkPos.Z}.txt",
            FileAccess.ModeFlags.Read);
        if (file == null || file.GetError() != Error.Ok)
        {
            GD.PrintErr(file?.GetError());
            file = FileAccess.Open($"res://voxels/Chunk_{chunkPos.X}_{chunkPos.Y}_{chunkPos.Z}.txt",
                FileAccess.ModeFlags.Read);
            if (file == null || file.GetError() != Error.Ok)
            {
                GD.PrintErr(file?.GetError());
                file = FileAccess.Open($"res://voxels/Chunk_0_0_0.txt",
                    FileAccess.ModeFlags.Read);
                if (file == null || file.GetError() != Error.Ok)
                {
                    GD.PrintErr(file?.GetError());
                    throw new FileLoadException(
                        $"Failed to open file: res://voxels/Chunk_0_0_0.txt");
                }
            }
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
            var index = VoxelConstants.index(y , z , x);
            voxels[index] = HexConvert.converter[color];
        }
        file.Close();
        return voxels;
    }
    public static void SaveFile(Vector3 chunkPos, byte[] voxels)
    {
        var file = FileAccess.Open($"Chunk_{chunkPos.X}_{chunkPos.Y}_{chunkPos.Z}.txt",
            FileAccess.ModeFlags.Write);
        if (file == null)
        {
            GD.PrintErr();
            throw new FileLoadException(
                $"Failed to open file: Chunk_{chunkPos.X}_{chunkPos.Y}_{chunkPos.Z}.txt");
        }

        for (var i = 0; i < voxels.Length; i++)
        {
            if(voxels[i] == 0) continue;
            var pos = VoxelConstants.reverseIndex(i);
            file.StoreLine($"{pos.Z-16} {pos.X-16} {pos.Y} {HexConvert.converter.First(pair => pair.Value == voxels[i]).Key:x6}");
        }
        GD.Print($"Saving {file.GetPathAbsolute()}");
        file.Close();
    }
}
