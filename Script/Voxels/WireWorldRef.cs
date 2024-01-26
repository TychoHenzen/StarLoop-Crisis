using Godot;

namespace StarLoop.Script.Voxels;

public class WireWorldRef
{
    private readonly int _index;
    public readonly VoxelChunk Chunk;
    public readonly Vector3I VoxelPos;
    public byte CurrentValue { get; private set; }
    public byte NextValue { get; set; }

    public WireWorldRef(Vector3I voxelPos, VoxelChunk chunk, int index)
    {
        VoxelPos = voxelPos;
        Chunk = chunk;
        _index = index;
        CurrentValue = chunk.Voxels[index];
        NextValue = CurrentValue;
    }


    public void Finish()
    {
        if (CurrentValue == NextValue)
            return;

        Chunk.Dirty.Add((VoxelPos+ Vector3.One / 2) / VoxelConstants.VoxelScalar);
        CurrentValue = NextValue;
        Chunk.Voxels[_index] = CurrentValue;
    }
}