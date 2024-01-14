using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace StarLoop.Script;

public class VoxelBuilder
{
	public static int ChunkVoxels => chunkSize * chunkSize * chunkSize;
	public const int chunkSize = 32;
	public const int VoxelScalar = 8;
	public const int tilesPerRow = 16;
	const float tileWidth = 1.0f / tilesPerRow;
	const float tileHeight = 1.0f / tilesPerRow;

	

	public static Mesh BuildMesh(byte[] voxelData)
	{
		ArrayMesh returned = new ArrayMesh();
		
		var arrays = new Array();
		arrays.Resize((int)ArrayMesh.ArrayType.Max);
		
		List<Vector3> vertices = new List<Vector3>();
		List<Vector3> normals = new List<Vector3>();
		List<int> indices = new List<int>();
		List<Vector2> Uvs = new List<Vector2>();
		int vertexCount = 0;
		for (int x = 0; x < chunkSize; x++)
		{
			for (int y = 0; y < chunkSize; y++)
			{
				for (int z = 0; z < chunkSize; z++)
				{
					if (voxelData[VoxelConstants.index(x, y, z)] != 0)
						CreateCube(x,y,z,vertices,normals,indices,Uvs, ref vertexCount, voxelData);
				}
			}
		}
		
		arrays[(int)ArrayMesh.ArrayType.Vertex] = vertices.ToArray();
		arrays[(int)ArrayMesh.ArrayType.Index] = indices.ToArray();
		arrays[(int)ArrayMesh.ArrayType.TexUV] = Uvs.ToArray();
		arrays[(int)ArrayMesh.ArrayType.Normal] = normals.ToArray();
		returned.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles,arrays);
		return returned;
	}

	
	public static Shape3D BuildShape(byte[] voxelData)
	{
		ConcavePolygonShape3D returned = new ConcavePolygonShape3D();
		
		var arrays = new Array();
		arrays.Resize((int)ArrayMesh.ArrayType.Max);
		
		List<Vector3> vertices = new List<Vector3>();
		List<Vector3> normals = new List<Vector3>();
		List<int> indices = new List<int>();
		List<Vector2> Uvs = new List<Vector2>();
		int vertexCount = 0;
		for (int x = 0; x < chunkSize; x++)
		{
			for (int y = 0; y < chunkSize; y++)
			{
				for (int z = 0; z < chunkSize; z++)
				{
					if (voxelData[VoxelConstants.index(x, y, z)] != 0)
						CreateCube(x,y,z,vertices,normals,indices,Uvs, ref vertexCount, voxelData);
				}
			}
		}
		
		returned.Data = indices.Select(i => vertices[i]).ToArray();
		return returned;
	}
	private static void CreateCube(int x, int y, int z,List<Vector3> vertices, List<Vector3> normals, List<int> indices,
		List<Vector2> Uvs, ref int vertexCount, byte[] voxelData)
	{
		
		// Helper function to add a face
		void AddFace(Vector3[] faceVertices, Vector2 Uv, ref int vertexcount)
		{
			vertices.AddRange(faceVertices.Select(vector3 => vector3
			/VoxelScalar));
			var i = vertexcount;
			indices.AddRange(new[] { 0, 1, 2, 1, 3, 2 }.Select(index => index + i));
			Uvs.AddRange(new []{Uv,Uv+new Vector2(tileWidth, 0),Uv+new Vector2(0, tileHeight),Uv+new Vector2(tileWidth, tileHeight)});
			vertexcount += faceVertices.Length;
			
			// Calculate normals using the cross product
			Vector3 edge1 = faceVertices[1] - faceVertices[0];
			Vector3 edge2 = faceVertices[2] - faceVertices[0];
			Vector3 normal = edge1.Cross(edge2).Normalized();

			normals.AddRange(new []{normal, normal, normal, normal});
		}
		Vector2 faceColor = GetUVForByte(voxelData[VoxelConstants.index(x, y, z)]);

		var indexer = VoxelConstants.index(x, y, z + 1);
		if(indexer < 0 || voxelData[indexer] == 0)
		AddFace(new Vector3[]
		{
			new(x, y, z + 1),
			new(x, y + 1, z + 1),
			new(x + 1, y, z + 1),
			new(x + 1, y + 1, z + 1)
		}, faceColor, ref vertexCount);
		// Back face
		
		indexer = VoxelConstants.index(x, y, z - 1);
		if(indexer  < 0 || voxelData[indexer] == 0)
		AddFace(new Vector3[]
		{
			new(x, y, z),
			new(x + 1, y, z),
			new(x, y + 1, z),
			new(x + 1, y + 1, z)
		}, faceColor, ref vertexCount);

// Left face
		
		indexer = VoxelConstants.index(x-1, y, z);
		if(indexer  < 0 || voxelData[indexer] == 0)
		AddFace(new Vector3[]
		{
			new(x, y, z),
			new(x, y + 1, z),
			new(x, y, z + 1),
			new(x, y + 1, z + 1)
		},  faceColor, ref vertexCount);

// Right face
		indexer = VoxelConstants.index(x+1, y, z);
		if(indexer  < 0 || voxelData[indexer] == 0)
		AddFace(new Vector3[]
		{
			new(x + 1, y, z),
			new(x + 1, y, z + 1),
			new(x + 1, y + 1, z),
			new(x + 1, y + 1, z + 1)
		},  faceColor, ref vertexCount);

// Top face
		indexer = VoxelConstants.index(x, y+1, z);
		if(indexer  < 0 || voxelData[indexer] == 0)
		AddFace(new Vector3[]
		{
			new(x, y + 1, z),
			new(x + 1, y + 1, z),
			new(x, y + 1, z + 1),
			new(x + 1, y + 1, z + 1)
		},  faceColor, ref vertexCount);

// Bottom face
		indexer = VoxelConstants.index(x, y-1, z);
		if(indexer  < 0 || voxelData[indexer] == 0)
		AddFace(new Vector3[]
		{
			new(x, y, z),
			new(x, y, z + 1),
			new(x + 1, y, z),
			new(x + 1, y, z + 1)
		}, faceColor, ref vertexCount);

	}

	public static Vector2 GetUVForByte(byte value)
	{

		int xIndex = value % tilesPerRow;
		int yIndex = value / tilesPerRow;

		float u = xIndex * tileWidth;
		float v = yIndex * tileHeight;

		return new Vector2(u, v);
	}

	public static Vector2 TileSize => new(1f / tilesPerRow, 1f / tilesPerRow);
}