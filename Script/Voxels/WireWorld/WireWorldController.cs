#region

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using StarLoop.Script.Voxels.Chunks;
using StarLoop.Script.Voxels.Data;
using StarLoop.Script.Voxels.WireWorld.Transitions;
using VoxelChunk = StarLoop.Script.Voxels.Chunks.VoxelChunk;

#endregion

namespace StarLoop.Script.Voxels.WireWorld;

public sealed partial class WireWorldController : Node
{
  [Export] public Camera3D PlayerCam { get; private set; }

  private readonly List<WireWorldRef> _wireWorldVoxels = new();

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

  public void NextStep(VoxelController voxels)
  {
    foreach (var entry in _wireWorldVoxels)
    {
      var transitions = TransitionRules.Transitions[entry.CurrentValue];

      var neighbors = entry.Neighbors.Select(voxels.GetVoxel);
      var neighborArray = neighbors as byte[] ?? neighbors.ToArray();
      foreach (var t in transitions)
      {
        if (!t.Odds(Random.Shared.NextDouble())) continue;
        if (!Matches(t.Neighbors, neighborArray)) continue;

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

  private static bool Matches(Rule[] transition, byte[] neighbors)
  {
    foreach (var rule in transition)
    {
      var count = 0;
      foreach (var b in neighbors)
      {
        if (b == rule.Type) count++;
      }
      if (count < rule.Min || count > rule.Max) return true;
    }
    return false;
  }
}