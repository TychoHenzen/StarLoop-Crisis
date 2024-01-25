using System.Collections.Generic;
using Godot;
using StarLoop.Script.Voxels;

namespace StarLoop.Script;

[Tool]
public partial class VoxelChunkLoader : Node
{
    [Export] private bool ShouldRun { get; set; }
    [Export] private bool ShouldClear { get; set; }

    public override void _Process(double delta)
    {
        if (!Engine.IsEditorHint()) return;
        if (ShouldClear)
        {
            if (!ShouldRun) return;

            ShouldRun = false;
            foreach (var chunk in GetChunks(GetTree().Root)) chunk.ClearChunk();

            return;
        }

        foreach (var chunk in GetChunks(GetTree().Root))
        {
            chunk.LoadIfChanged();
        }

        if (!ShouldRun) return;

        ShouldRun = false;
        foreach (var chunk in GetChunks(GetTree().Root))
        {
            chunk.LoadChunk();
        }
    }

    private IEnumerable<VoxelChunk> GetChunks(Node rootNode)
    {
        foreach (var child in rootNode.GetChildren())
        {
            // Check if the child is of type VoxelChunk
            if (child is VoxelChunk chunk)
            {
                yield return chunk;
            }

            // Recursively search through the children of the child node
            foreach (var voxelChunk in GetChunks(child))
            {
                yield return voxelChunk;
            }
        }
    }
}