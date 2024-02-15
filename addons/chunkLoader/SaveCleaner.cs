#region

using System.Collections.Generic;
using Godot;
using StarLoop.Script.Voxels.Chunks;

#endregion

namespace StarLoop.addons.chunkLoader;

[Tool]
public partial class SaveCleaner : EditorPlugin
{
    public override void _Notification(int what)
    {
        if (what == NotificationWMWindowFocusOut)
        {
            GD.Print("clearing");
            foreach (var chunk in GetChunks(GetTree().Root)) chunk.ClearChunk();
        }

        else if (what == NotificationWMWindowFocusIn)
        {
            GD.Print("loading");
            foreach (var chunk in GetChunks(GetTree().Root)) chunk.LoadChunk();
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