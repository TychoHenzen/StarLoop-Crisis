namespace StarLoop.Script.Voxels;

public static class VoxelBuilder
{
    private static readonly Vector2 Right = new(VoxelConstants.TileSize.X, 0);
    private static readonly Vector2 Bot = new(0, VoxelConstants.TileSize.Y);
    private static readonly Vector2 BotRight = new(VoxelConstants.TileSize.X, VoxelConstants.TileSize.Y);

    private static List<Vector3> _vertices = new();
    private static List<Vector3> _normals = new();
    private static List<int> _indices = new();


    public static void BuildMesh(Array arrays, byte[] voxelData)
    {
        _vertices.Clear();
        _normals.Clear();
        _indices.Clear();
        for (var pos = new Vector3I(); pos.X < VoxelConstants.ChunkSize; pos.X++)
        {
            for (pos.Y = 0; pos.Y < VoxelConstants.ChunkSize; pos.Y++)
            {
                for (pos.Z = 0; pos.Z < VoxelConstants.ChunkSize; pos.Z++)
                {
                    if (voxelData[VoxelConstants.Index(pos)] != 0)
                        MeshCube(pos, voxelData);
                }
            }
        }

        arrays[(int)Mesh.ArrayType.Vertex] = _vertices.ToArray();
        arrays[(int)Mesh.ArrayType.Index] = _indices.ToArray();
        arrays[(int)Mesh.ArrayType.Normal] = _normals.ToArray();
    }


    public static void UpdateMeshTexture(Array arrays, Vector2[] uvs, byte[] voxelData)
    {
        int counter = 0;
        for (var pos = new Vector3I(); pos.X < VoxelConstants.ChunkSize; pos.X++)
        {
            for (pos.Y = 0; pos.Y < VoxelConstants.ChunkSize; pos.Y++)
            {
                for (pos.Z = 0; pos.Z < VoxelConstants.ChunkSize; pos.Z++)
                {
                    if (voxelData[VoxelConstants.Index(pos)] != 0)
                        TextureCube(pos, uvs, ref counter, voxelData);
                }
            }
        }

        arrays[(int)Mesh.ArrayType.TexUV] = uvs;
    }

    public static Shape3D BuildShape(byte[] voxelData)
    {
        var returned = new ConcavePolygonShape3D();

        var arrays = new Array();
        arrays.Resize((int)Mesh.ArrayType.Max);

        _vertices = new List<Vector3>();
        _normals = new List<Vector3>();
        _indices = new List<int>();
        for (var pos = new Vector3I(); pos.X < VoxelConstants.ChunkSize; pos.X++)
        {
            for (pos.Y = 0; pos.Y < VoxelConstants.ChunkSize; pos.Y++)
            {
                for (pos.Z = 0; pos.Z < VoxelConstants.ChunkSize; pos.Z++)
                {
                    if (voxelData[VoxelConstants.Index(pos)] != 0)
                        MeshCube(pos, voxelData);
                }
            }
        }

        returned.Data = _indices.Select(i => _vertices[i]).ToArray();
        return returned;
    }


    private static void TextureCube(Vector3I pos, Vector2[] uvs, ref int counter, byte[] voxelData)
    {
        var selfIndex = VoxelConstants.Index(pos);
        var faceColor = GetUvForByte(voxelData[selfIndex]);

        var indexer = VoxelConstants.Index(pos, z: +1);
        if (ShouldRender(voxelData, indexer, selfIndex))
            TextureFace(uvs, ref counter, faceColor);
        // Back face

        indexer = VoxelConstants.Index(pos, z: -1);
        if (ShouldRender(voxelData, indexer, selfIndex))
            TextureFace(uvs, ref counter, faceColor);

// Left face
        indexer = VoxelConstants.Index(pos, -1);
        if (ShouldRender(voxelData, indexer, selfIndex))
            TextureFace(uvs, ref counter, faceColor);

// Right face
        indexer = VoxelConstants.Index(pos, +1);
        if (ShouldRender(voxelData, indexer, selfIndex))
            TextureFace(uvs, ref counter, faceColor);

// Top face
        indexer = VoxelConstants.Index(pos, y: +1);
        if (ShouldRender(voxelData, indexer, selfIndex))
            TextureFace(uvs, ref counter, faceColor);

// Bottom face
        indexer = VoxelConstants.Index(pos, y: -1);
        if (ShouldRender(voxelData, indexer, selfIndex))
            TextureFace(uvs, ref counter, faceColor);
    }

// Helper function to add a face
    private static void TextureFace(Vector2[] uvs, ref int index, Vector2 uv)
    {
        uvs[index++] = uv + Right; // Bottom-right
        uvs[index++] = uv; // Bottom-left
        uvs[index++] = uv + BotRight; // Top-right
        uvs[index++] = uv + Bot; // Top-left
    }

    private static void MeshCube(Vector3I pos, IReadOnlyList<byte> voxelData)
    {
        var selfIndex = VoxelConstants.Index(pos);
        var indexer = VoxelConstants.Index(pos, z: +1);
        if (ShouldRender(voxelData, indexer, selfIndex))
            RenderFace(new[]
            {
                pos + new Vector3(0, 0, 1),
                pos + new Vector3(1, 0, 1),
                pos + new Vector3(0, 1, 1),
                pos + new Vector3(1, 1, 1)
            });
        // Back face

        indexer = VoxelConstants.Index(pos, z: -1);
        if (ShouldRender(voxelData, indexer, selfIndex))
            RenderFace(new[]
            {
                pos + new Vector3(1, 0, 0),
                pos + new Vector3(0, 0, 0),
                pos + new Vector3(1, 1, 0),
                pos + new Vector3(0, 1, 0)
            });

// Left face

        indexer = VoxelConstants.Index(pos, -1);
        if (ShouldRender(voxelData, indexer, selfIndex))
            RenderFace(new[]
            {
                pos + new Vector3(0, 0, 0),
                pos + new Vector3(0, 0, 1),
                pos + new Vector3(0, 1, 0),
                pos + new Vector3(0, 1, 1)
            });

// Right face
        indexer = VoxelConstants.Index(pos, +1);
        if (ShouldRender(voxelData, indexer, selfIndex))
            RenderFace(new[]
            {
                pos + new Vector3(1, 0, 1),
                pos + new Vector3(1, 0, 0),
                pos + new Vector3(1, 1, 1),
                pos + new Vector3(1, 1, 0)
            });

// Top face
        indexer = VoxelConstants.Index(pos, y: +1);
        if (ShouldRender(voxelData, indexer, selfIndex))
            RenderFace(new[]
            {
                pos + new Vector3(0, 1, 0),
                pos + new Vector3(0, 1, 1),
                pos + new Vector3(1, 1, 0),
                pos + new Vector3(1, 1, 1)
            });

// Bottom face
        indexer = VoxelConstants.Index(pos, y: -1);
        if (ShouldRender(voxelData, indexer, selfIndex))
            RenderFace(new[]
            {
                pos + new Vector3(0, 0, 0),
                pos + new Vector3(1, 0, 0),
                pos + new Vector3(0, 0, 1),
                pos + new Vector3(1, 0, 1)
            });
    }

    private static void RenderFace(IReadOnlyList<Vector3> faceVertices)
    {
        _indices.AddRange(new[] { 2, 1, 0, 2, 3, 1 }.Select(index => index + _vertices.Count));
        _vertices.AddRange(faceVertices.Select(vector3 => vector3
                                                          / VoxelConstants.VoxelScalar));

        // Calculate normals using the cross product
        var edge1 = faceVertices[1] - faceVertices[0];
        var edge2 = faceVertices[2] - faceVertices[0];
        var normal = edge1.Cross(edge2).Normalized();

        _normals.AddRange(new[] { normal, normal, normal, normal });
    }

    private static bool ShouldRender(IReadOnlyList<byte> voxelData, int indexer, int selfIndex)
    {
        return indexer < 0 || selfIndex < 0 || voxelData[indexer] == (int)Cell.Air;
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