#region

using System;
using Godot;
using StarLoop.Script.Voxels.WireWorld;

#endregion

namespace StarLoop.Script.Voxels.Data;

public static class VoxelConstants
{
    public const int ChunkSize = 32;
    public const int VoxelScalar = 8;
    public const int TilesPerRow = 16;
    public const int TextureMapSize = 256;
    public static readonly Vector2 TileSize = new(1f / TextureMapSize, 1f / TextureMapSize);

    public static readonly Vector3I[] VoxelOffsets =
    {
        new(-1, -1, -1), new(-1, -1, 0), new(-1, -1, 1),
        new(-1, 0, -1), new(-1, 0, 0), new(-1, 0, 1),
        new(-1, 1, -1), new(-1, 1, 0), new(-1, 1, 1),

        new(0, -1, -1), new(0, -1, 0), new(0, -1, 1),
        new(0, 0, -1), new(0, 0, 1),
        new(0, 1, -1), new(0, 1, 0), new(0, 1, 1),

        new(1, -1, -1), new(1, -1, 0), new(1, -1, 1),
        new(1, 0, -1), new(1, 0, 0), new(1, 0, 1),
        new(1, 1, -1), new(1, 1, 0), new(1, 1, 1)
    };

    public static Rule[] Always => Array.Empty<Rule>();
    public static Func<double, bool> AlwaysOdds => static _ => true;
    public static int ChunkVoxels => ChunkSize * ChunkSize * ChunkSize;

    public static Vector3I VoxelMax => new(ChunkSize, ChunkSize, ChunkSize);

    public static int Index(int x, int y, int z)
    {
        if (x < 0 || x >= ChunkSize ||
            y < 0 || y >= ChunkSize ||
            z < 0 || z >= ChunkSize) return -1;

        return x * ChunkSize * ChunkSize + y * ChunkSize + z;
    }

    public static int Index(Vector3I pos,
        int x = 0, int y = 0, int z = 0)
    {
        pos += new Vector3I(x, y, z);
        if (pos.X < 0 || pos.X >= ChunkSize ||
            pos.Y < 0 || pos.Y >= ChunkSize ||
            pos.Z < 0 || pos.Z >= ChunkSize) return -1;

        return pos.X * ChunkSize * ChunkSize + pos.Y * ChunkSize + pos.Z;
    }

    public static Vector3I ReverseIndex(int index)
    {
        var z = index % ChunkSize;
        index /= ChunkSize;
        var y = index % ChunkSize;
        index /= ChunkSize;
        return new Vector3I(index, y, z);
    }
}