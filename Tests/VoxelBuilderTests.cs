using GdMUT;
using Godot;
using StarLoop.Script.Voxels.Data;
using StarLoop.Script.Voxels.WireWorld;
using StarLoop.Script.Voxels.WireWorld.Transitions;

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
        VoxelConstants.ChunkVoxels - 1),
      (VoxelConstants.Index(1, 2, 3),
        VoxelConstants.ChunkSize * VoxelConstants.ChunkSize + VoxelConstants.ChunkSize * 2 + 3),
      (VoxelConstants.Index(-1, 0, 0), -1),
      (VoxelConstants.Index(0, -1, 0), -1),
      (VoxelConstants.Index(0, 0, -1), -1),
      (VoxelConstants.Index(VoxelConstants.ChunkSize, VoxelConstants.ChunkSize - 1, VoxelConstants.ChunkSize - 1),
        -1),
      (VoxelConstants.Index(VoxelConstants.ChunkSize - 1, VoxelConstants.ChunkSize, VoxelConstants.ChunkSize - 1),
        -1),
      (VoxelConstants.Index(VoxelConstants.ChunkSize - 1, VoxelConstants.ChunkSize - 1, VoxelConstants.ChunkSize),
        -1)
    };
    //Act
    //Assert
    var index = 0;
    foreach (var value in values)
    {
      if (value.Item1 != value.Item2)
        return new Result(false, $"{index}: {value.Item1} != {value.Item2}");

      index++;
    }

    return Result.Success;
  }

  [CSTestFunction]
  public static Result ReverseIndexing_Works()
  {
    (Vector3, Vector3)[] values =
    {
      (new Vector3(0, 0, 0), VoxelConstants.ReverseIndex(0)),
      (new Vector3(VoxelConstants.ChunkSize - 1, VoxelConstants.ChunkSize - 1, VoxelConstants.ChunkSize - 1),
        VoxelConstants.ReverseIndex(VoxelConstants.ChunkVoxels - 1)),
      (new Vector3(1, 2, 3),
        VoxelConstants.ReverseIndex(VoxelConstants.ChunkSize * VoxelConstants.ChunkSize +
          VoxelConstants.ChunkSize * 2 + 3))
    };
    //Act
    //Assert
    var index = 0;
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
    var t1 = new WireWorldTransition(
      new[] { new Rule(Cell.Air, 1, 2) },
      Cell.Wire, Cell.Text1, static v => v < 0.1f);

    var t2 = new WireWorldTransition(
      new[] { new Rule(Cell.Air, 1, 2) },
      Cell.Wire, Cell.Text1, static v => v < 0.1f);
    //Act
    //Assert
    return t1.Equals(t2) ? Result.Success : Result.Failure;
  }

#endif
}