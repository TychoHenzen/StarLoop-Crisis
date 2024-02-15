#region

using System.Linq;
using Godot;
using StarLoop.Script.Voxels.Chunks;
using StarLoop.Script.Voxels.Data;

#endregion

namespace StarLoop.Script.Voxels.WireWorld;

public sealed class WireWorldRef
{
    private readonly int _index;

    private readonly Vector3 _worldPos;
    public readonly VoxelChunk Chunk;
    public readonly Vector3I VoxelPos;

    public WireWorldRef(Vector3I voxelPos, VoxelChunk chunk, int index, VoxelController voxels)
    {
        VoxelPos = voxelPos;
        Chunk = chunk;
        _index = index;
        CurrentValue = chunk.Voxels[index];
        NextValue = CurrentValue;
        _worldPos = (VoxelPos + Vector3.One / 2) / VoxelConstants.VoxelScalar;
        NeighborCoordinates =
            VoxelConstants.VoxelOffsets.Select(i => i + voxelPos).Select(voxels.GetVoxelAddress).ToArray();
    }

    public (Vector3I, int)[] NeighborCoordinates { get; }

    public byte CurrentValue { get; private set; }
    public byte NextValue { get; set; }


    public void Finish()
    {
        if (CurrentValue == NextValue)
            return;

        if (!Chunk.Dirty.Contains((_index, _worldPos)))
            Chunk.Dirty.Add((_index, _worldPos));
        CurrentValue = NextValue;
        Chunk.Voxels[_index] = CurrentValue;
    }
}