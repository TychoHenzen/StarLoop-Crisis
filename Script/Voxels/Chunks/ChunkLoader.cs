#region

using System.Collections.Generic;
using System.Globalization;
using Godot;
using FileAccess = Godot.FileAccess;

#endregion

namespace StarLoop.Script.Voxels;

public static class ChunkLoader
{
    private static readonly Vector3[] Offsets =
    {
        new(0, 1, 0), //section 1, correct
        new(1, 1, 0), //section 2, correct
        new(1, 0, 0), //section 4, correct
        new(0, 0, 0), //section 3, correct
        new(0, 0, 1), //section 5, correct
        new(0, 1, 1), // section 6
        new(1, 1, 1), //section 7
        new(1, 0, 1) // section 8
    };


    public static byte[] LoadVoxelTextFile(FileAccess file)
    {
        var voxels = new byte[VoxelConstants.ChunkVoxels];
        while (!file.EofReached())
        {
            var line = file.GetLine();
            if (line.Contains('#') || line.Trim().Length == 0) continue;

            var values = line.Split(' ');
            var x = int.Parse(values[0]) + 16;
            var y = int.Parse(values[1]) + 16;
            var z = int.Parse(values[2]);
            var color = int.Parse(values[3], NumberStyles.HexNumber);
            var tmp = x;
            x = y;
            y = z;
            z = tmp;
            var index = VoxelConstants.Index(new Vector3I(x, y, z));
            GD.Print($"Text: [{x}, {y}, {z}]: {color:X}");
            voxels[index] = HexConvert.Converter[color];
        }


        file.Close();
        return voxels;
    }


    public static byte[] LoadGoxFile(FileAccess file)
    {
        var voxels = new byte[VoxelConstants.ChunkVoxels];
        var index = 0;
        while (!file.EofReached() && index < 8)
        {
            SeekString(file, "BL16");
            if (file.EofReached()) break;

            var fileSize = file.Get32();
            var pngFile = file.GetBuffer(fileSize);
            var img = new Image();
            img.LoadPngFromBuffer(pngFile);

            LoadImage(index, img, voxels);
            index++;
        }

        file.Close();
        return voxels;
    }

    private static void LoadImage(int index, Image img, IList<byte> voxels)
    {
        for (var coordinate = new Vector2I(); coordinate.X < 64; coordinate.X++)
        {
            for (coordinate.Y = 0; coordinate.Y < 64; coordinate.Y++)
            {
                ParsePixel(index, img, voxels, coordinate);
            }
        }
    }

    private static void ParsePixel(int index, Image img, IList<byte> voxels, Vector2I coordinate)
    {
        // Calculate the corresponding x, y coordinates in the 64x64 image
        var posIndex = coordinate.X + 64 * coordinate.Y;
        var px = posIndex % 16;
        posIndex /= 16;
        var py = posIndex % 16;
        posIndex /= 16;
        var pz = posIndex;

        var offset = Offsets[index];
        var vx = (int)(px + offset.X * 16);
        var vy = (int)(py + offset.Y * 16);
        var vz = (int)(pz + offset.Z * 16);
        var color = img.GetPixel(coordinate.X, coordinate.Y).ToArgb32();
        if ((color & 0xFF000000) == 0)
            color = 0xff_ff_ff;
        if (!HexConvert.Converter.TryGetValue((int)(color & 0x00FFFFFF), out var value)) return;

        var voxelIndex = VoxelConstants.Index(vy, vz, vx);
        if (voxelIndex != -1)
            voxels[voxelIndex] = value;
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
}