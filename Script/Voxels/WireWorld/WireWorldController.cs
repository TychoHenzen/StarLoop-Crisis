#region

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using StarLoop.Script.Voxels.Data;
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

  public void NextStep(Chunks.VoxelController voxels)
  {
    foreach (var entry in _wireWorldVoxels)
    {
      for (var index = 0; index < _offsets.Length; index++)
        _neighbors[index] = voxels.GetVoxel(entry.VoxelPos + _offsets[index]);

      var transitions = TransitionRules.Transitions[entry.CurrentValue];
      var futures = new List<WireWorldTransition>();
      foreach (var t in transitions
                 .Where(transition => Matches(transition.Neighbors, _neighbors)))
        futures.Add(t);

      ParseTransition(futures, entry);
    }

    foreach (var t in _wireWorldVoxels)
    {
      t.Finish();
    }

    voxels.RedrawDirty();
    Step++;
  }

  private static void ParseTransition(IReadOnlyList<WireWorldTransition> futures, WireWorldRef entry)
  {
    switch (futures.Count)
    {
      case 0:
        // No change to entry.NextValue
        break;
      case 1:
      {
        var future = futures[0];
        if (future.Odds(Random.Shared.NextDouble())) entry.NextValue = future.Result;
        break;
      }
      default:
      {
        entry.NextValue = futures
          .FirstOrDefault(static future => future.Odds(Random.Shared.NextDouble()),
            new WireWorldTransition(null, 0, entry.NextValue))
          .Result;
        break;
      }
    }
  }

  private static bool Matches(Rule[] transition, byte[] neighbors)
  {
    foreach (var rule in transition)
    {
      var count = neighbors.Count(b => b == rule.Type);

      if (count < rule.Min || count > rule.Max)
        return false;
    }

    return true;
  }
}