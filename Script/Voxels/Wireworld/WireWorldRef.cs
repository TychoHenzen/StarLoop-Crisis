using Godot;

namespace StarLoop.Script.Voxels;

public class WireWorldRef
{
    private readonly int _index;
    public readonly VoxelChunk Chunk;
    public readonly Vector3I VoxelPos;
    private readonly Vector3 worldPos;

    public WireWorldRef(Vector3I voxelPos, VoxelChunk chunk, int index)
    {
        VoxelPos = voxelPos;
        Chunk = chunk;
        _index = index;
        CurrentValue = chunk.Voxels[index];
        NextValue = CurrentValue;
        worldPos = (VoxelPos + Vector3.One / 2) / VoxelConstants.VoxelScalar;
    }

    public byte CurrentValue { get; private set; }
    public byte NextValue { get; set; }


    public void Finish()
    {
        if (CurrentValue == NextValue)
            return;

        if (!Chunk.Dirty.Contains(worldPos))
            Chunk.Dirty.Add(worldPos);
        CurrentValue = NextValue;
        Chunk.Voxels[_index] = CurrentValue;
    }
}