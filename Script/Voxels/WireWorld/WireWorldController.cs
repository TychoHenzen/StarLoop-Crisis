#region

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using StarLoop.Script.Voxels.Data;
using StarLoop.Script.Voxels.WireWorld.Transitions;
using VoxelChunk = StarLoop.Script.Voxels.Chunks.VoxelChunk;

#endregion

namespace StarLoop.Script.Voxels.WireWorld;

public sealed partial class WireWorldController : Node
{
  [Export] public Camera3D PlayerCam { get; private set; }

  private static readonly Vector3I[] _offsets =
  {
    new(-1, -1, -1), new(-1, -1, 0), new(-1, -1, 1),
    new(-1, 0, -1), new(-1, 0, 0), new(-1, 0, 1),
    new(-1, 1, -1), new(-1, 1, 0), new(-1, 1, 1),

    new(0, -1, -1), new(0, -1, 0), new(0, -1, 1),
    new(0, 0, -1), new(0, 0, 1),
    new(0, 1, -1), new(0, 1, 0), new(0, 1, 1),

    new(1, -1, -1), new(1, -1, 0), new(1, -1, 1),
    new(1, 0, -1), new(1, 0, 0), new(1, 0, 1),
    new(1, 1, -1), new(1, 1, 0), new(1, 1, 1)
  };

  private readonly List<WireWorldRef> _wireWorldVoxels = new();

  private readonly byte[] _neighbors = new byte[_offsets.Length];

  public static int Step { get; private set; }

  public void Register(Vector3I position, VoxelChunk chunk)
  {
    _wireWorldVoxels.RemoveAll(obj =>
      obj.Chunk == chunk);
    var offset = position * VoxelConstants.ChunkSize;
    for (var index = 0; index < chunk.Voxels.Length; index++)
    {
      var voxel = chunk.Voxels[index];
      if (TransitionRules.Transitions[voxel].Any())
      {
        _wireWorldVoxels.Add(new WireWorldRef(VoxelConstants.ReverseIndex(index) + offset, chunk, index));
      }
    }
  }

  private byte[] neighbors = new byte[26];
  public void NextStep(Chunks.VoxelController voxels)
  {
    foreach (var entry in _wireWorldVoxels)
    {
      for (var index = 0; index < _offsets.Length; index++)
        _neighbors[index] = voxels.GetVoxel(entry.VoxelPos + _offsets[index]);

      var transitions = TransitionRules.Transitions[entry.CurrentValue];

      foreach (var t in transitions)
      {
        if (!t.Odds(Random.Shared.NextDouble())) continue;

        for (int i = 0; i < _offsets.Length; i++)
        {
          neighbors[i] = voxels.GetVoxel(_offsets[i] + entry.VoxelPos);
        }
        if (!Matches(t.Neighbors, neighbors)) continue;

        entry.NextValue = t.Result;
        break;
      }
    }

    foreach (var t in _wireWorldVoxels)
    {
      t.Finish();
    }

    voxels.RedrawDirty();
    Step++;
  }
  
  private static int count = 0;
  private static int index = 0;
  private static int index2 = 0;
  private static bool Matches(Rule[] transition, byte[] neighbors)
  {
    for (index = 0; index < transition.Length; index++)
    {
      count = 0;
      for (index2 = 0; index2 < neighbors.Length; index2++)
      {
        var b = neighbors[index2];
        if (b == transition[index].Type) count++;
      }

      if (count < transition[index].Min || count > transition[index].Max)
        return false;
    }

    return true;
  }
}