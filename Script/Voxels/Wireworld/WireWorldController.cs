#region

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using StarLoop.Script.Voxels;

#endregion

namespace StarLoop.Script;

public partial class WireWorldController : Node
{
    private static readonly Vector3I[] Offsets =
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

    private byte[] neighbors = new byte[Offsets.Length];

    [Export] public Camera3D PlayerCam { get; private set; }
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
            // Parallel.ForEach(_wireWorldVoxels, entry =>
        {
            for (var index = 0; index < Offsets.Length; index++)
                neighbors[index] = voxels.GetVoxel(entry.VoxelPos + Offsets[index]);


            var transitions = TransitionRules.Transitions[entry.CurrentValue];
            var futures = new List<WireWorldTransition>();
            foreach (var t in transitions
                         .Where(transition => Matches(transition.Neighbors, neighbors)))
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
                    .FirstOrDefault(future => future.Odds(Random.Shared.NextDouble()),
                        new WireWorldTransition(null, 0, entry.NextValue)).Result;
                break;
            }
        }
    }

    private static bool Matches(Rule[] transition, byte[] neighbors)
    {
        foreach (var rule in transition)
        {
            var count = 0;
            foreach (var b in neighbors)
                if (b == rule.Type)
                    count++;

            if (count < rule.Min || count > rule.Max)
                return false;
        }

        return true;
    }
}