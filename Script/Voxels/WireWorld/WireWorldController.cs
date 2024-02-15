#region

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using StarLoop.Script.Voxels.Chunks;
using StarLoop.Script.Voxels.Data;
using VoxelChunk = StarLoop.Script.Voxels.Chunks.VoxelChunk;

#endregion

namespace StarLoop.Script.Voxels.WireWorld;

public sealed partial class WireWorldController : Node
{
    private static readonly int[] _neighborCounts = new int[256];


    private readonly List<WireWorldRef> _wireWorldVoxels = new();
    [Export] public Camera3D PlayerCam { get; private set; }

    public static int Step { get; private set; }

    public void Register(Vector3I position, VoxelChunk chunk, VoxelController voxels)
    {
        _wireWorldVoxels.RemoveAll(obj =>
            obj.Chunk == chunk);
        var offset = position * VoxelConstants.ChunkSize;
        for (var index = 0; index < chunk.Voxels.Length; index++)
        {
            var voxel = chunk.Voxels[index];
            if (TransitionRules.Transitions[voxel].Any())
            {
                _wireWorldVoxels.Add(
                    new WireWorldRef(VoxelConstants.ReverseIndex(index) + offset, chunk, index, voxels));
            }
        }
    }

    public void NextStep(VoxelController voxels)
    {
        foreach (var entry in _wireWorldVoxels)
        {
            for (var i = 0; i < 256; i++)
            {
                _neighborCounts[i] = 0;
            }

            foreach (var neighbor in voxels.GetVoxels(entry.NeighborCoordinates))
            {
                _neighborCounts[neighbor]++;
            }

            var transitions = TransitionRules.Transitions[entry.CurrentValue];
            foreach (var t in transitions)
            {
                if (!t.Odds(Random.Shared.NextDouble())) continue;
                if (!Matches(t.Neighbors)) continue;

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

    private static bool Matches(Rule[] transition)
    {
        foreach (var rule in transition)
        {
            var count = _neighborCounts[rule.Type];
            if (count < rule.Min || count > rule.Max)
                return false;
        }

        return true;
    }
}