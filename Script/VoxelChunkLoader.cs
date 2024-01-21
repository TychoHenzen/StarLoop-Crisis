using System.Collections.Generic;
using System.Linq;
using Godot;

namespace StarLoop.Script;

[Tool]
public partial class VoxelChunkLoader : Node
{
    [Export] public bool ShouldRun { get; set; }

    public override void _Process(double delta)
    {
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

    private IEnumerable<Voxels.VoxelChunk> GetChunks(Node rootNode)
    {
        foreach (var child in rootNode.GetChildren())
        {
            // Check if the child is of type VoxelChunk
            if (child is Voxels.VoxelChunk chunk)
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