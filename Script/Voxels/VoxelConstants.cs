using Godot;

namespace StarLoop.Script;

public static class VoxelConstants
{
    public static int ChunkVoxels => ChunkSize * ChunkSize * ChunkSize;
    public const int ChunkSize = 32;
    public const int VoxelScalar = 8;
    public const int TilesPerRow = 16;
    public static readonly Vector2 TileSize = new(1f / TilesPerRow, 1f / TilesPerRow);
    public static int Index(int x, int y, int z)
    {
        if (x < 0 || x >= ChunkSize || y < 0 || y >= ChunkSize || z < 0 || z >= ChunkSize) return -1; 
        return x * ChunkSize * ChunkSize + y * ChunkSize + z;
    }
    public static int Index(Vector3 pos)
    {
        return Index((int)pos.X, (int)pos.Y, (int)pos.Z);
    }
    public static Vector3 ReverseIndex(int index)
    {
        int z = index % ChunkSize;
        index /= ChunkSize;
        int y = index % ChunkSize;
        index /= ChunkSize;
        return new Vector3(index, y, z);
    }
}