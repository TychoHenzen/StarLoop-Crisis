using Godot;
using System;
using System.Collections.Generic;
using StarLoop.Script;

public partial class VoxelController : Node
{
    private Dictionary<Vector3, VoxelChunk> Registry = new();
    public void Register(Vector3 position, VoxelChunk chunk) => Registry.Add(position, chunk);
    
    public void Save()
    {
        foreach (var chunk in Registry)
        {
            ChunkLoader.SaveFile(chunk.Key, chunk.Value.Voxels);
        }
    }

    public void SetVoxel(Vector3 chunkPos, Vector3 voxelPos, byte newByte) =>
        Registry[chunkPos].SetVoxel(voxelPos, newByte);
    public byte GetVoxel(Vector3 chunkPos, Vector3 voxelPos) =>
        Registry[chunkPos].Voxels[VoxelConstants.index(voxelPos)];
}
