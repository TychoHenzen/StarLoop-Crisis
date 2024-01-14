using System.Collections.Generic;
using GdMUT;
using Godot;
using StarLoop.Script;

namespace StarLoop.Tests;

public static class VoxelBuilderTests
{
#if TOOLS
    [CSTestFunction]
    public static Result IndexingWorks()
    {
        //Arrange
        (int, int)[] values =
        {
            (VoxelBuilder.index(0, 0, 0), 0),
            (VoxelBuilder.index(VoxelBuilder.chunkSize - 1, VoxelBuilder.chunkSize - 1, VoxelBuilder.chunkSize - 1),
                VoxelBuilder.ChunkVoxels-1),
            (VoxelBuilder.index(1, 2, 3),
                VoxelBuilder.chunkSize * VoxelBuilder.chunkSize + VoxelBuilder.chunkSize * 2 + 3),
            (VoxelBuilder.index(-1, 0, 0), -1),
            (VoxelBuilder.index(0, -1, 0), -1),
            (VoxelBuilder.index(0, 0, -1), -1),
            (VoxelBuilder.index(VoxelBuilder.chunkSize, VoxelBuilder.chunkSize - 1, VoxelBuilder.chunkSize - 1), -1),
            (VoxelBuilder.index(VoxelBuilder.chunkSize - 1, VoxelBuilder.chunkSize, VoxelBuilder.chunkSize - 1), -1),
            (VoxelBuilder.index(VoxelBuilder.chunkSize - 1, VoxelBuilder.chunkSize - 1, VoxelBuilder.chunkSize), -1)
        };
        //Act
        //Assert
        int index = 0;
        foreach (var value in values)
        {
            if (value.Item1 != value.Item2)
                return new Result(false, $"{index}: {value.Item1} != {value.Item2}");
            index++;
        }
        return Result.Success;
    }

    [CSTestFunction]
    public static Result LoadingFileWorks()
    {
        var file = FileAccess.Open("res://voxels/Test.txt",
            FileAccess.ModeFlags.Read);

        return new Result(file != null);
    }
    
    #endif
}