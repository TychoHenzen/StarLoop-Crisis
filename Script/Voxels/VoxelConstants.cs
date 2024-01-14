using Godot;

namespace StarLoop.Script;

public static class VoxelConstants
{
    public static int ChunkVoxels => chunkSize * chunkSize * chunkSize;
    public const int chunkSize = 32;
    public const int VoxelScalar = 8;
    public const int tilesPerRow = 16;
    const float tileWidth = 1.0f / tilesPerRow;
    const float tileHeight = 1.0f / tilesPerRow;
    public static int index(int x, int y, int z)
    {
        if (x < 0 || x >= chunkSize || y < 0 || y >= chunkSize || z < 0 || z >= chunkSize) return -1; 
        return x * chunkSize * chunkSize + y * chunkSize + z;
    }
    public static int index(Vector3 pos)
    {
        return index((int)pos.X, (int)pos.Y, (int)pos.Z);
    }
    public static Vector3 reverseIndex(int index)
    {
        int z = index % chunkSize;
        index /= chunkSize;
        int y = index % chunkSize;
        index /= chunkSize;
        return new Vector3(index, y, z);
    }
}