using System.Globalization;
using System.IO;
using System.Linq;
using Godot;
using FileAccess = Godot.FileAccess;

namespace StarLoop.Script.Voxels;

public static class ChunkLoader
{
    public static byte[] LoadVoxelTextFile(FileAccess file)
    {
        byte[] voxels = new byte[VoxelConstants.ChunkVoxels];
        while (!file.EofReached())
        {
            string line = file.GetLine();
            if (line.Contains('#') || line.Trim().Length == 0) continue;
            string[] values = line.Split(' ');
            int x = int.Parse(values[0]) + 16;
            int y = int.Parse(values[1]) + 16;
            int z = int.Parse(values[2]);
            int color = int.Parse(values[3], NumberStyles.HexNumber);
            var index = VoxelConstants.Index(y, z, x);
            GD.Print($"Text: [{x}, {y}, {z}]: {color:X}");
            voxels[index] = HexConvert.Converter[color];
        }

        file.Close();
        return voxels;
    }

    private static Vector3[] offsets = new[]
    {
        new Vector3(0, 1, 0), //section 1, correct
        new(1, 1, 0), //section 2, correct
        new(1, 0, 0), //section 4, correct
        new(0, 0, 0), //section 3, correct
        new(0, 0, 1), //section 5, correct
        new(0, 1, 1), // section 6
        new(1, 1, 1), //section 7
        new(1, 0, 1) // section 8
    };


    public static byte[] LoadGoxFile(FileAccess file)
    {
        byte[] voxels = new byte[VoxelConstants.ChunkVoxels];
        int index = 0;
        while (!file.EofReached())
        {
            SeekString(file, "BL16");
            if (file.EofReached()) break;

            var fileSize = file.Get32();
            var pngFile = file.GetBuffer(fileSize);
            var img = new Image();
            img.LoadPngFromBuffer(pngFile);

            for (int x = 0; x < 64; x++)
            for (int y = 0; y < 64; y++)
            {
                // Calculate the corresponding x, y coordinates in the 64x64 image
                var posIndex = x + 64 * y;
                int px = posIndex % 16;
                posIndex /= 16;
                int py = posIndex % 16;
                posIndex /= 16;
                var pz = posIndex;

                var offset = offsets[index];
                int vx = (int)(px + offset.X * 16);
                int vy = (int)(py + offset.Y * 16);
                int vz = (int)(pz + offset.Z * 16);
                var color = img.GetPixel(x, y).ToArgb32();
                if ((color & 0xFF000000) == 0)
                    color = 0xff_ff_ff;
                if (HexConvert.Converter.TryGetValue((int)(color & 0x00FFFFFF), out var value))
                {
                    var voxelIndex = VoxelConstants.Index(vy, vz, vx);
                    if(voxelIndex != -1)
                        voxels[voxelIndex] = value;
                }
            }

            if (++index >= 8)
                break;
        }

        file.Close();
        return voxels;
    }

    private static void SeekString(FileAccess file, string match)
    {
        while (!file.EofReached())
        {
            for (var i = 0; i < match.Length; i++)
            {
                if (file.Get8() != match[i])
                    break;
                if (i == match.Length - 1)
                    return;
            }
        }
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
            if (voxels[i] == 0) continue;
            var pos = VoxelConstants.ReverseIndex(i);
            file.StoreLine(
                $"{pos.Z - 16} {pos.X - 16} {pos.Y} {HexConvert.Converter.First(pair => pair.Value == voxels[i]).Key:x6}");
        }

        GD.Print($"Saving {file.GetPathAbsolute()}");
        file.Close();
    }
}