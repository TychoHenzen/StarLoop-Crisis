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
            (VoxelConstants.index(0, 0, 0), 0),
            (VoxelConstants.index(VoxelConstants.chunkSize - 1, VoxelConstants.chunkSize - 1, VoxelConstants.chunkSize - 1),
                VoxelConstants.ChunkVoxels-1),
            (VoxelConstants.index(1, 2, 3),
                VoxelConstants.chunkSize * VoxelConstants.chunkSize + VoxelConstants.chunkSize * 2 + 3),
            (VoxelConstants.index(-1, 0, 0), -1),
            (VoxelConstants.index(0, -1, 0), -1),
            (VoxelConstants.index(0, 0, -1), -1),
            (VoxelConstants.index(VoxelConstants.chunkSize, VoxelConstants.chunkSize - 1, VoxelConstants.chunkSize - 1), -1),
            (VoxelConstants.index(VoxelConstants.chunkSize - 1, VoxelConstants.chunkSize, VoxelConstants.chunkSize - 1), -1),
            (VoxelConstants.index(VoxelConstants.chunkSize - 1, VoxelConstants.chunkSize - 1, VoxelConstants.chunkSize), -1)
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
        var file = FileAccess.Open("res://voxels/Chunk_0_0_0.txt",
            FileAccess.ModeFlags.Read);

        return new Result(file != null);
    }
    
    [CSTestFunction]
    public static Result ReverseIndexing_Works()
    {
        (Vector3, Vector3)[] values =
        {
            (new Vector3(0, 0, 0),VoxelConstants.reverseIndex( 0)),
            (new Vector3(VoxelConstants.chunkSize - 1, VoxelConstants.chunkSize - 1, VoxelConstants.chunkSize - 1), VoxelConstants.reverseIndex(VoxelConstants.ChunkVoxels-1)),
            (new Vector3(1, 2, 3), VoxelConstants.reverseIndex(VoxelConstants.chunkSize * VoxelConstants.chunkSize + VoxelConstants.chunkSize * 2 + 3))
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
    
    #endif
}