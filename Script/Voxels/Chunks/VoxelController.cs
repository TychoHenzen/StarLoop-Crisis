using System.Collections.Generic;
using Godot;
using StarLoop.Script.Voxels.Data;

namespace StarLoop.Script.Voxels.Chunks;

public sealed partial class VoxelController : Node
{
  [Export] private WireWorld.WireWorldController _wireWorld;

  private readonly Dictionary<Vector3I, VoxelChunk> _registry = new();

  public void Register(Vector3I position, VoxelChunk chunk)
  {
    _registry.Add(position, chunk);
    _wireWorld.Register(position, chunk);
  }

  public static Vector3I GetChunkPos(Vector3 position) =>
    (Vector3I)(position / VoxelConstants.ChunkSize).Floor();

  public static Vector3I GetVoxelPos(Vector3I position) =>
    new(Mathf.PosMod(position.X, VoxelConstants.ChunkSize),
      Mathf.PosMod(position.Y, VoxelConstants.ChunkSize),
      Mathf.PosMod(position.Z, VoxelConstants.ChunkSize));

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

  public void RedrawDirty()
  {
    foreach (var chunk in _registry)
    {
      chunk.Value.Redraw();
    }
  }
}