using System;
using Godot;

namespace StarLoop.Script;

public class WireworldRef
{
    public readonly VoxelChunk Chunk;
    public readonly Vector3 VoxelPos;
    public readonly int Index;
    public readonly byte OriginalValue;
    public byte CurrentValue;
    public byte NextValue;

    public WireworldRef(Vector3 voxelPos,  VoxelChunk chunk, int index)
    {
        VoxelPos = voxelPos;
        OriginalValue =  chunk.Voxels[index];
        Chunk = chunk;
        Index = index;
        CurrentValue = OriginalValue;
        NextValue = CurrentValue;
    }

    public void Finish()
    {
        if (CurrentValue == NextValue)
            return;
        
        Chunk.Dirty = true;
        CurrentValue = NextValue;
        Chunk.Voxels[Index] = CurrentValue;
    }

    public void PreSave()
    {
        Chunk.Dirty = true;
        Chunk.Voxels[Index] = OriginalValue;
    }
}