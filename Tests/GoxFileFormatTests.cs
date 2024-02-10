#region

using System;
using System.Linq;
using System.Text;
using GdMUT;
using Godot;
using StarLoop.Script.Voxels;

#endregion

namespace StarLoop.Tests;

public static class GoxFileFormatTests
{
#if TOOLS
    [CSTestFunction]
    public static Result Loading_Text_File_Works()
    {
        //Arrange
        var textFile = FileAccess.Open($"Tests/TestVoxels/Test.txt",
            FileAccess.ModeFlags.Read);
        var goxFile = FileAccess.Open($"Tests/TestVoxels/Test.gox",
            FileAccess.ModeFlags.Read);
        //Act
        var textVoxels = ChunkLoader.LoadVoxelTextFile(textFile);
        var goxVoxels = ChunkLoader.LoadGoxFile(goxFile);
        //Assert
        if (textVoxels.SequenceEqual(goxVoxels))
        {
            return Result.Success;
        }


        StringBuilder diffBuilder = new StringBuilder();
        int minLength = Math.Min(textVoxels.Length, goxVoxels.Length);
        for (int i = 0; i < minLength; i++)
        {
            if (!textVoxels[i].Equals(goxVoxels[i]))
            {
                diffBuilder.AppendLine(
                    $"Difference at index {i}: TextVoxels = {textVoxels[i]}, GoxVoxels = {goxVoxels[i]}");
            }
        }

        if (textVoxels.Length != goxVoxels.Length)
        {
            diffBuilder.AppendLine(
                $"Length mismatch: TextVoxels = {textVoxels.Length}, GoxVoxels = {goxVoxels.Length}");
        }

        return new Result(false, diffBuilder.ToString());
    }
#endif
}