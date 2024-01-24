using System.Collections.Generic;
using Godot;
using StarLoop.Script.Voxels;

namespace StarLoop.Script;

public partial class VoxelController : Node
{
    private readonly Dictionary<Vector3I, VoxelChunk> _registry = new();
    [Export] private WireWorldController _wireWorld;

    public void Register(Vector3I position, VoxelChunk chunk)
    {
        _registry.Add(position, chunk);
        _wireWorld.Register(position, chunk);
    }

    public void Save()
    {
        foreach (var chunk in _registry)
        {
            ChunkLoader.SaveFile(chunk.Key, chunk.Value.Voxels);
        }
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
        _wireWorld.Register(chunkPos, _registry[chunkPos]);
    }

    public byte GetVoxel(Vector3I position)
    {
        var chunkIdx = GetChunkPos(position);
        var voxelIdx = VoxelConstants.Index(GetVoxelPos(position));
        if (voxelIdx == -1 || !_registry.ContainsKey(chunkIdx))
            return 0;

        return _registry[chunkIdx].Voxels[voxelIdx];
    }

    public void RedrawDirty(WireWorldController wireWorldController)
    {
        foreach (var chunk in _registry)
        {
            if (chunk.Value.Redraw())
            {
                // wireWorldController.Register(chunk.Key, chunk.Value);
            }
        }
    }
}