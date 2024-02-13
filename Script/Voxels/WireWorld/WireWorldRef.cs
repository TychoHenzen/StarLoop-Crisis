using Godot;
using StarLoop.Script.Voxels.Data;

namespace StarLoop.Script.Voxels.WireWorld;

public sealed class WireWorldRef
{
  private readonly int _index;
  public readonly Chunks.VoxelChunk Chunk;
  public readonly Vector3I VoxelPos;

  private readonly Vector3 _worldPos;
  
  public Vector3I[] Neighbors { get; }

  public WireWorldRef(Vector3I voxelPos, Chunks.VoxelChunk chunk, int index)
  {
    VoxelPos = voxelPos;
    Chunk = chunk;
    _index = index;
    CurrentValue = chunk.Voxels[index];
    NextValue = CurrentValue;
    _worldPos = (VoxelPos + Vector3.One / 2) / VoxelConstants.VoxelScalar;
    
    Neighbors = new[]
    {
      VoxelPos + new Vector3I(-1, -1, -1),
      VoxelPos + new Vector3I(-1, -1, 0),
      VoxelPos + new Vector3I(-1, -1, 1),
      VoxelPos + new Vector3I(-1, 0, -1),
      VoxelPos + new Vector3I(-1, 0, 0),
      VoxelPos + new Vector3I(-1, 0, 1),
      VoxelPos + new Vector3I(-1, 1, -1),
      VoxelPos + new Vector3I(-1, 1, 0),
      VoxelPos + new Vector3I(-1, -1, -1),
      VoxelPos + new Vector3I(0, -1, -1),
      VoxelPos + new Vector3I(0, -1, 0),
      VoxelPos + new Vector3I(0, -1, 1),
      VoxelPos + new Vector3I(0, 0, -1),
      VoxelPos + new Vector3I(0, 0, 1),
      VoxelPos + new Vector3I(0, 1, -1),
      VoxelPos + new Vector3I(0, 1, 0),
      VoxelPos + new Vector3I(0, 1, 1),
      VoxelPos + new Vector3I(1, -1, -1),
      VoxelPos + new Vector3I(1, -1, 0),
      VoxelPos + new Vector3I(1, -1, 1),
      VoxelPos + new Vector3I(1, 0, -1),
      VoxelPos + new Vector3I(1, 0, 0),
      VoxelPos + new Vector3I(1, 0, 1),
      VoxelPos + new Vector3I(1, 1, -1),
      VoxelPos + new Vector3I(1, 1, 0),
      VoxelPos + new Vector3I(1, 1, 1),
    };
  }

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