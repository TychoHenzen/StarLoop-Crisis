using Godot;

namespace StarLoop.Script.Voxels;

public class WireWorldRef
{
    private readonly int _index;
    private readonly byte _originalValue;
    public readonly VoxelChunk Chunk;
    public readonly Vector3I VoxelPos;

    public WireWorldRef(Vector3I voxelPos, VoxelChunk chunk, int index)
    {
        VoxelPos = voxelPos;
        _originalValue = chunk.Voxels[index];
        Chunk = chunk;
        _index = index;
        CurrentValue = _originalValue;
        NextValue = CurrentValue;
    }

    public byte CurrentValue { get; private set; }
    public byte NextValue { get; set; }

    public void Finish()
    {
        if (CurrentValue == NextValue)
            return;

        Chunk.Dirty = true;
        CurrentValue = NextValue;
        Chunk.Voxels[_index] = CurrentValue;
    }

    public void PreSave()
    {
        Chunk.Dirty = true;
        Chunk.Voxels[_index] = _originalValue;
    }
}