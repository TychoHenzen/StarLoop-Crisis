using Godot;

namespace StarLoop.Script.Voxels;

public class WireWorldRef
{
    public readonly VoxelChunk Chunk;
    public readonly Vector3 VoxelPos;
    private readonly int _index;
    private readonly byte _originalValue;
    public byte CurrentValue;
    public byte NextValue;

    public WireWorldRef(Vector3 voxelPos,  VoxelChunk chunk, int index)
    {
        VoxelPos = voxelPos;
        _originalValue =  chunk.Voxels[index];
        Chunk = chunk;
        _index = index;
        CurrentValue = _originalValue;
        NextValue = CurrentValue;
    }

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