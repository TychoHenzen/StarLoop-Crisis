using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using StarLoop.Script.Voxels;

namespace StarLoop.Script;

public partial class WireWorldController : Node
{
    private readonly List<WireWorldRef> _wireWorldVoxels = new();

    private static readonly Vector3[] Offsets =
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

    public void Register(Vector3 position, VoxelChunk chunk)
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
        // foreach (var entry in WireWorldVoxels)
        Parallel.ForEach(_wireWorldVoxels, entry =>
        {
            var neighbors = Offsets
                .Select(offset => entry.VoxelPos + offset)
                .Select(voxels.GetVoxel)
                .GroupBy(b => b)
                .ToDictionary(bytes => bytes.Key, bytes => bytes.Count());
            // somehow match this with rules to determine future state
            var futures = TransitionRules.Transitions[entry.CurrentValue]
                .Where(transition => Matches(transition, neighbors)).ToList();

            switch (futures.Count)
            {
                case 0:
                    // No change to entry.NextValue
                    break;
                case 1:
                {
                    var future = futures[0];
                    if (Random.Shared.NextDouble() < future.Odds)
                    {
                        entry.NextValue = future.Result;
                    }

                    // Otherwise, no change to entry.NextValue
                    break;
                }
                default:
                {
                    foreach (var future in futures
                                 .Where(future => !(Random.Shared.NextDouble() > future.Odds)))
                    {
                        entry.NextValue = future.Result;
                        break;
                    }

                    // Roll the odds again for the selected transition
                    // Otherwise, no change to entry.NextValue
                    break;
                }
            }
        });

        foreach (var t in _wireWorldVoxels)
        {
            t.Finish();
        }

        voxels.RedrawDirty(this);
    }

    private static bool Matches(WireWorldTransition transition, Dictionary<byte, int> neighbors)
    {
        foreach (var rule in transition.Neighbors)
        {
            if (!neighbors.TryGetValue(rule.Type, out int count) || count < rule.Min || count > rule.Max) return false;
        }

        return true;
    }

    public void PreSave(VoxelController voxels)
    {
        foreach (var voxel in _wireWorldVoxels)
        {
            voxel.PreSave();
        }

        voxels.RedrawDirty(this);
    }
}