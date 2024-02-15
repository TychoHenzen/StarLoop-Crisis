#region

using System.Collections.Generic;
using Godot;
using StarLoop.Script.Voxels.Data;
using StarLoop.Script.Voxels.WireWorld;

#endregion

namespace StarLoop.Script.Voxels.Chunks;

public sealed partial class VoxelController : Node
{
    private readonly Dictionary<Vector3I, VoxelChunk> _registry = new();
    [Export] private WireWorldController _wireWorld;

    private byte[] returned = new byte[27];

    public void Register(Vector3I position, VoxelChunk chunk)
    {
        _registry.Add(position, chunk);
        _wireWorld.Register(position, chunk, this);
    }

    public static Vector3I GetChunkPos(Vector3 position)
    {
        return (Vector3I)(position / VoxelConstants.ChunkSize).Floor();
    }

    public static Vector3I GetVoxelPos(Vector3I position)
    {
        return new Vector3I(Mathf.PosMod(position.X, VoxelConstants.ChunkSize),
            Mathf.PosMod(position.Y, VoxelConstants.ChunkSize),
            Mathf.PosMod(position.Z, VoxelConstants.ChunkSize));
    }

    public void SetVoxel(Vector3I position, byte newByte)
    {
        var chunkPos = GetChunkPos(position);
        _registry[chunkPos].SetVoxel(GetVoxelPos(position), newByte);
        _wireWorld.Register(chunkPos, _registry[chunkPos], this);
    }

    public byte[] GetVoxels((Vector3I chunkIdx, int voxelIdx)[] neighbors)
    {
        for (var i = 0; i < 27; i++)
        {
            returned[i] = 0;
        }

        for (var index = 0; index < neighbors.Length; index++)
        {
            var n = neighbors[index];
            if (n.voxelIdx == -1) continue;

            returned[index] = _registry[n.chunkIdx].Voxels[n.voxelIdx];
        }

        return returned;
    }

    public byte GetVoxel(Vector3I position)
    {
        var (chunkIdx, voxelIdx) = GetVoxelAddress(position);
        if (voxelIdx == -1) return 0;
        return _registry[chunkIdx].Voxels[voxelIdx];
    }

    public (Vector3I, int) GetVoxelAddress(Vector3I position)
    {
        var chunkIdx = GetChunkPos(position);
        var voxelIdx = VoxelConstants.Index(GetVoxelPos(position));
        if (voxelIdx == -1 || !_registry.ContainsKey(chunkIdx))
            return (Vector3I.Zero, -1);

        return (chunkIdx, voxelIdx);
    }

    public void RedrawDirty()
    {
        foreach (var chunk in _registry)
        {
            chunk.Value.Redraw();
        }
    }
}