using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace StarLoop.Script.Voxels;

public static class VoxelBuilder
{
    // public const int VoxelScalar = 8;
    // public static int ChunkVoxels => ChunkSize * ChunkSize * ChunkSize;
    // 
    //
    // private const int ChunkSize = 32;
    // private const int TilesPerRow = 16;
    // private const float TileWidth = 1.0f / VoxelConstants.TilesPerRow;
    // private const float TileHeight = 1.0f / TilesPerRow;

    private static readonly Vector2 Right = new(VoxelConstants.TileSize.X, 0);
    private static readonly Vector2 Bot = new(0, VoxelConstants.TileSize.Y);
    private static readonly Vector2 BotRight = new(VoxelConstants.TileSize.X, VoxelConstants.TileSize.Y);

    public static ArrayMesh BuildMesh(byte[] voxelData)
    {
        var returned = new ArrayMesh();

        var arrays = new Array();
        arrays.Resize((int)Mesh.ArrayType.Max);

        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        var indices = new List<int>();
        var vertexCount = 0;
        for (var x = 0; x < VoxelConstants.ChunkSize; x++)
        {
            for (var y = 0; y < VoxelConstants.ChunkSize; y++)
            {
                for (var z = 0; z < VoxelConstants.ChunkSize; z++)
                {
                    if (voxelData[VoxelConstants.Index(x, y, z)] != 0)
                        CreateCube(x, y, z, vertices, normals, indices, ref vertexCount, voxelData);
                }
            }
        }

        arrays[(int)Mesh.ArrayType.Vertex] = vertices.ToArray();
        arrays[(int)Mesh.ArrayType.Index] = indices.ToArray();
        arrays[(int)Mesh.ArrayType.Normal] = normals.ToArray();
        returned.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
        return returned;
    }

    private static Vector2[] _uvs;

    public static void UpdateMeshTexture(ArrayMesh mesh, byte[] voxelData)
    {
        var arrays = mesh.SurfaceGetArrays(0);

        var vertexCount = 0;
        _uvs = arrays[(int)Mesh.ArrayType.Vertex].AsVector2Array();
        for (var x = 0; x < VoxelConstants.ChunkSize; x++)
        {
            for (var y = 0; y < VoxelConstants.ChunkSize; y++)
            {
                for (var z = 0; z < VoxelConstants.ChunkSize; z++)
                {
                    if (voxelData[VoxelConstants.Index(x, y, z)] != 0)
                        CreateCube(x, y, z, _uvs, ref vertexCount, voxelData);
                }
            }
        }

        arrays[(int)Mesh.ArrayType.TexUV] = _uvs;
        mesh.ClearSurfaces();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
    }

    public static Shape3D BuildShape(byte[] voxelData)
    {
        var returned = new ConcavePolygonShape3D();

        var arrays = new Array();
        arrays.Resize((int)Mesh.ArrayType.Max);

        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        var indices = new List<int>();
        var vertexCount = 0;
        for (var x = 0; x < VoxelConstants.ChunkSize; x++)
        {
            for (var y = 0; y < VoxelConstants.ChunkSize; y++)
            {
                for (var z = 0; z < VoxelConstants.ChunkSize; z++)
                {
                    if (voxelData[VoxelConstants.Index(x, y, z)] != 0)
                        CreateCube(x, y, z, vertices, normals, indices, ref vertexCount, voxelData);
                }
            }
        }

        returned.Data = indices.Select(i => vertices[i]).ToArray();
        return returned;
    }


    private static void CreateCube(int x, int y, int z, Vector2[] uvs, ref int vertexCount, byte[] voxelData)
    {
        var selfIndex = VoxelConstants.Index(x, y, z);
        var faceColor = GetUvForByte(voxelData[selfIndex]);

        var indexer = VoxelConstants.Index(x, y, z + 1);
        if (ShouldRender(voxelData, indexer, selfIndex))
            AddFace(faceColor, ref vertexCount);
        // Back face

        indexer = VoxelConstants.Index(x, y, z - 1);
        if (ShouldRender(voxelData, indexer, selfIndex))
            AddFace(faceColor, ref vertexCount);

// Left face
        indexer = VoxelConstants.Index(x - 1, y, z);
        if (ShouldRender(voxelData, indexer, selfIndex))
            AddFace(faceColor, ref vertexCount);

// Right face
        indexer = VoxelConstants.Index(x + 1, y, z);
        if (ShouldRender(voxelData, indexer, selfIndex))
            AddFace(faceColor, ref vertexCount);

// Top face
        indexer = VoxelConstants.Index(x, y + 1, z);
        if (ShouldRender(voxelData, indexer, selfIndex))
            AddFace(faceColor, ref vertexCount);

// Bottom face
        indexer = VoxelConstants.Index(x, y - 1, z);
        if (ShouldRender(voxelData, indexer, selfIndex))
            AddFace(faceColor, ref vertexCount);
        return;

        // Helper function to add a face
        void AddFace(Vector2 uv, ref int i)
        {
            uvs[i++] = uv + Right; // Bottom-right
            uvs[i++] = uv; // Bottom-left
            uvs[i++] = uv + BotRight; // Top-right
            uvs[i++] = uv + Bot; // Top-left
        }
    }

    private static void CreateCube(int x, int y, int z, List<Vector3> vertices, List<Vector3> normals,
        List<int> indices,
        ref int vertexCount, byte[] voxelData)
    {
        var selfIndex = VoxelConstants.Index(x, y, z);
        var indexer = VoxelConstants.Index(x, y, z + 1);
        if (ShouldRender(voxelData, indexer, selfIndex))
            AddFace(new Vector3[]
                {
                    new(x, y, z + 1), // New Bottom-left
                    new(x + 1, y, z + 1), // New Top-left
                    new(x, y + 1, z + 1), // New Bottom-right
                    new(x + 1, y + 1, z + 1) // New Top-right
                }
                , ref vertexCount);
        // Back face

        indexer = VoxelConstants.Index(x, y, z - 1);
        if (ShouldRender(voxelData, indexer, selfIndex))
            AddFace(new Vector3[]
            {
                new(x + 1, y, z),
                new(x, y, z),
                new(x + 1, y + 1, z),
                new(x, y + 1, z)
            }, ref vertexCount);

// Left face

        indexer = VoxelConstants.Index(x - 1, y, z);
        if (ShouldRender(voxelData, indexer, selfIndex))
            AddFace(new Vector3[]
            {
                new(x, y, z),
                new(x, y, z + 1),
                new(x, y + 1, z),
                new(x, y + 1, z + 1)
            }, ref vertexCount);

// Right face
        indexer = VoxelConstants.Index(x + 1, y, z);
        if (ShouldRender(voxelData, indexer, selfIndex))
            AddFace(new Vector3[]
            {
                new(x + 1, y, z + 1),
                new(x + 1, y, z),
                new(x + 1, y + 1, z + 1),
                new(x + 1, y + 1, z)
            }, ref vertexCount);

// Top face
        indexer = VoxelConstants.Index(x, y + 1, z);
        if (ShouldRender(voxelData, indexer, selfIndex))
            AddFace(new Vector3[]
            {
                new(x, y + 1, z),
                new(x, y + 1, z + 1),
                new(x + 1, y + 1, z),
                new(x + 1, y + 1, z + 1)
            }, ref vertexCount);

// Bottom face
        indexer = VoxelConstants.Index(x, y - 1, z);
        if (ShouldRender(voxelData, indexer, selfIndex))
            AddFace(new Vector3[]
            {
                new(x, y, z),
                new(x + 1, y, z),
                new(x, y, z + 1),
                new(x + 1, y, z + 1)
            }, ref vertexCount);
        return;

        // Helper function to add a face
        void AddFace(Vector3[] faceVertices, ref int vertexCount)
        {
            vertices.AddRange(faceVertices.Select(vector3 => vector3
                                                             / VoxelConstants.VoxelScalar));
            var i = vertexCount;
            indices.AddRange(new[] { 2, 1, 0, 2, 3, 1 }.Select(index => index + i));
            vertexCount += faceVertices.Length;

            // Calculate normals using the cross product
            var edge1 = faceVertices[1] - faceVertices[0];
            var edge2 = faceVertices[2] - faceVertices[0];
            var normal = edge1.Cross(edge2).Normalized();

            normals.AddRange(new[] { normal, normal, normal, normal });
        }
    }

    private static bool ShouldRender(byte[] voxelData, int indexer, int selfIndex)
    {
        return indexer < 0 || voxelData[indexer] == (int)Cell.Air ||
               (voxelData[indexer] == (int)Cell.Glass && voxelData[selfIndex] != (int)Cell.Glass);
    }

    public static Vector2 GetUvForByte(byte value)
    {
        var xIndex = value % VoxelConstants.TilesPerRow;
        var yIndex = value / VoxelConstants.TilesPerRow;

        var u = xIndex * VoxelConstants.TileSize.X;
        var v = yIndex * VoxelConstants.TileSize.Y;

        return new Vector2(u, v);
    }
}