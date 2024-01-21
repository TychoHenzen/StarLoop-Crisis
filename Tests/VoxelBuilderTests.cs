using System.Collections.Generic;
using GdMUT;
using Godot;
using StarLoop.Script;
using StarLoop.Script.Voxels;

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
            (VoxelConstants.Index(0, 0, 0), 0),
            (VoxelConstants.Index(VoxelConstants.ChunkSize - 1, VoxelConstants.ChunkSize - 1, VoxelConstants.ChunkSize - 1),
                VoxelConstants.ChunkVoxels-1),
            (VoxelConstants.Index(1, 2, 3),
                VoxelConstants.ChunkSize * VoxelConstants.ChunkSize + VoxelConstants.ChunkSize * 2 + 3),
            (VoxelConstants.Index(-1, 0, 0), -1),
            (VoxelConstants.Index(0, -1, 0), -1),
            (VoxelConstants.Index(0, 0, -1), -1),
            (VoxelConstants.Index(VoxelConstants.ChunkSize, VoxelConstants.ChunkSize - 1, VoxelConstants.ChunkSize - 1), -1),
            (VoxelConstants.Index(VoxelConstants.ChunkSize - 1, VoxelConstants.ChunkSize, VoxelConstants.ChunkSize - 1), -1),
            (VoxelConstants.Index(VoxelConstants.ChunkSize - 1, VoxelConstants.ChunkSize - 1, VoxelConstants.ChunkSize), -1)
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
            (new Vector3(0, 0, 0),VoxelConstants.ReverseIndex( 0)),
            (new Vector3(VoxelConstants.ChunkSize - 1, VoxelConstants.ChunkSize - 1, VoxelConstants.ChunkSize - 1), VoxelConstants.ReverseIndex(VoxelConstants.ChunkVoxels-1)),
            (new Vector3(1, 2, 3), VoxelConstants.ReverseIndex(VoxelConstants.ChunkSize * VoxelConstants.ChunkSize + VoxelConstants.ChunkSize * 2 + 3))
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
    public static Result Transition_Equality_Works_As_Expected()
    {

        WireWorldTransition t1 =
            new WireWorldTransition(new[] { new Rule(Cell.Air, 1, 2)}, Cell.Wire, Cell.Text1, 0.1f );
        
        WireWorldTransition t2 =
            new WireWorldTransition(new[] { new Rule(Cell.Air, 1, 2)}, Cell.Wire, Cell.Text1, 0.1f );
        //Act
        //Assert
        if (t1.Equals(t2))
            return Result.Success;
        return Result.Failure;
    }

    #endif
}