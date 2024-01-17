using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StarLoop.Script;

public partial class VoxelController : Node
{
    private Dictionary<Vector3, VoxelChunk> Registry = new();
    [Export] private WireWorldController wireworld;

    public void Register(Vector3 position, VoxelChunk chunk)
    {
        Registry.Add(position, chunk);
        wireworld.Register(position, chunk);
    }

    public void Save()
    {
        foreach (var chunk in Registry)
        {
            ChunkLoader.SaveFile(chunk.Key, chunk.Value.Voxels);
        }
    }

    public static Vector3 GetChunkPos(Vector3 position)
    {
        return (position / VoxelConstants.chunkSize).Floor();
    }

    public static Vector3 GetVoxelPos(Vector3 position)
    {
        return new Vector3(Mathf.PosMod(position.X, VoxelConstants.chunkSize),
            Mathf.PosMod(position.Y, VoxelConstants.chunkSize), 
            Mathf.PosMod(position.Z, VoxelConstants.chunkSize));
    }

    public void SetVoxel(Vector3 position, byte newByte)
    {
        var chunkPos = GetChunkPos(position);
        Registry[chunkPos].SetVoxel(GetVoxelPos(position), newByte);
        wireworld.Register(chunkPos,  Registry[chunkPos]);
    }

    public byte GetVoxel(Vector3 position)
    {
        var chunkIdx = GetChunkPos(position);
        var voxelIdx = VoxelConstants.index(GetVoxelPos(position));
        if (voxelIdx == -1 || !Registry.ContainsKey(chunkIdx)) 
            return 0;
        
        return Registry[chunkIdx].Voxels[voxelIdx];
    }

    public void RedrawDirty(WireWorldController wireWorldController)
    {
        foreach (var chunk in Registry)
        {
            if(chunk.Value.Redraw())
                wireWorldController.Register(chunk.Key,chunk.Value);
        }
    }
}