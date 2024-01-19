using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StarLoop.Script;

public partial class WireWorldController : Node
{
    private List<WireworldRef> WireWorldVoxels = new();
    private static readonly Vector3[] offsets =
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

    private Vector3 _myPos;

    public void Register(Vector3 position, VoxelChunk chunk)
    {
        WireWorldVoxels.RemoveAll(obj =>
            obj.Chunk == chunk);
        for (var index = 0; index < chunk.Voxels.Length; index++)
        {
            // var entry = WireWorldVoxels[i];
            // Parallel.For(0, chunk.Voxels.Length, index =>
            {
                _myPos = VoxelConstants.reverseIndex(index) + position * VoxelConstants.chunkSize;


                if (TransitionRules.Transitions.Any(transition => transition.Self == chunk.Voxels[index]))
                {
                    WireWorldVoxels.Add(new WireworldRef(_myPos, chunk, index));
                }
            };
        }
    }

    public void NextStep(VoxelController voxels)
    {
        // foreach (var entry in WireWorldVoxels)
        Parallel.ForEach(WireWorldVoxels, entry =>
        {
            var neighbors = offsets
                .Select(offset => entry.VoxelPos + offset)
                .Select(voxels.GetVoxel)
                .GroupBy(b => b)
                .ToDictionary(bytes => bytes.Key, bytes => bytes.Count());
            // somehow match this with rules to determine future state
            entry.NextValue = TransitionRules.Transitions
                .Where(transition => transition.Self == entry.CurrentValue)
                .SingleOrDefault(transition => Matches(transition, neighbors),
                    new WireworldTransition(null, 0, entry.CurrentValue)).Result;
        });

        for (int index = 0; index < WireWorldVoxels.Count; index++)
        {
            WireWorldVoxels[index].Finish();
        }

        voxels.RedrawDirty(this);
    }

    private bool Matches(WireworldTransition transition, Dictionary<byte, int> neighbors)
    {
        return transition.Neighbors.All(rule =>
            neighbors.TryGetValue(rule.Type, out int count) &&
            count >= rule.Min && count <= rule.Max);
    }

    public void preSave(VoxelController voxels)
    {
        foreach (var voxel in WireWorldVoxels)
        {
            voxel.PreSave();
        }
        voxels.RedrawDirty(this);
    }
}