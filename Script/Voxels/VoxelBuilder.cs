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
	
	public static Vector2 TileSize => new(1f / tilesPerRow, 1f / tilesPerRow);

	public static ArrayMesh BuildMesh(byte[] voxelData)
	{
		ArrayMesh returned = new ArrayMesh();
		
		var arrays = new Array();
		arrays.Resize((int)ArrayMesh.ArrayType.Max);
		
		List<Vector3> vertices = new List<Vector3>();
		List<Vector3> normals = new List<Vector3>();
		List<int> indices = new List<int>();
		int vertexCount = 0;
		for (int x = 0; x < chunkSize; x++)
		{
			for (int y = 0; y < chunkSize; y++)
			{
				for (int z = 0; z < chunkSize; z++)
				{
					if (voxelData[VoxelConstants.index(x, y, z)] != 0)
						CreateCube(x,y,z,vertices,normals,indices, ref vertexCount, voxelData);
				}
			}
		}
		
		arrays[(int)Mesh.ArrayType.Vertex] = vertices.ToArray();
		arrays[(int)Mesh.ArrayType.Index] = indices.ToArray();
		arrays[(int)Mesh.ArrayType.Normal] = normals.ToArray();
		returned.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles,arrays);
		return returned;
	}

	public static void UpdateMeshTexture(ArrayMesh mesh, byte[] voxelData)
	{
		var arrays = mesh.SurfaceGetArrays(0);
		
		int vertexCount = 0;
		Vector2[] uvs = arrays[(int)Mesh.ArrayType.Vertex].AsVector2Array();
		for (int x = 0; x < chunkSize; x++)
		{
			for (int y = 0; y < chunkSize; y++)
			{
				for (int z = 0; z < chunkSize; z++)
				{
					if (voxelData[VoxelConstants.index(x, y, z)] != 0)
						CreateCube(x,y,z,uvs, ref vertexCount, voxelData);
				}
			}
		}

		arrays[(int)Mesh.ArrayType.TexUV] = uvs;
		mesh.ClearSurfaces();
		mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles,arrays);
	}
	
	public static Shape3D BuildShape(byte[] voxelData)
	{
		ConcavePolygonShape3D returned = new ConcavePolygonShape3D();
		
		var arrays = new Array();
		arrays.Resize((int)ArrayMesh.ArrayType.Max);
		
		List<Vector3> vertices = new List<Vector3>();
		List<Vector3> normals = new List<Vector3>();
		List<int> indices = new List<int>();
		int vertexCount = 0;
		for (int x = 0; x < chunkSize; x++)
		{
			for (int y = 0; y < chunkSize; y++)
			{
				for (int z = 0; z < chunkSize; z++)
				{
					if (voxelData[VoxelConstants.index(x, y, z)] != 0)
						CreateCube(x,y,z,vertices,normals,indices, ref vertexCount, voxelData);
				}
			}
		}
		
		returned.Data = indices.Select(i => vertices[i]).ToArray();
		return returned;
	}

	private static Vector2 right = new(tileWidth, 0); 
	private static Vector2 bot = new(0, tileHeight); 
	private static Vector2 botRight = new(tileWidth, tileHeight); 
	private static void CreateCube(int x, int y, int z, Vector2[] uvs, ref int vertexCount,  byte[] voxelData)
	{
		
		Vector2 faceColor = GetUVForByte(voxelData[VoxelConstants.index(x, y, z)]);

		// Helper function to add a face
		void AddFace(Vector2 Uv, ref int i)
		{
			uvs[i++] = Uv + right;       // Bottom-right
			uvs[i++] = Uv;               // Bottom-left
			uvs[i++] = Uv + botRight;    // Top-right
			uvs[i++] = Uv + bot;         // Top-left


		}
		var indexer = VoxelConstants.index(x, y, z + 1);
		if(indexer < 0 || voxelData[indexer] == 0)
		AddFace( faceColor, ref vertexCount);
		// Back face
		
		indexer = VoxelConstants.index(x, y, z - 1);
		if(indexer  < 0 || voxelData[indexer] == 0)
			AddFace( faceColor, ref vertexCount);

// Left face
		
		indexer = VoxelConstants.index(x-1, y, z);
		if(indexer  < 0 || voxelData[indexer] == 0)
			AddFace( faceColor, ref vertexCount);

// Right face
		indexer = VoxelConstants.index(x+1, y, z);
		if(indexer  < 0 || voxelData[indexer] == 0)
			AddFace( faceColor, ref vertexCount);

// Top face
		indexer = VoxelConstants.index(x, y+1, z);
		if(indexer  < 0 || voxelData[indexer] == 0)
			AddFace( faceColor, ref vertexCount);

// Bottom face
		indexer = VoxelConstants.index(x, y-1, z);
		if(indexer  < 0 || voxelData[indexer] == 0)
			AddFace( faceColor, ref vertexCount);
	}
	
	private static void CreateCube(int x, int y, int z,List<Vector3> vertices, List<Vector3> normals, List<int> indices,
		ref int vertexCount, byte[] voxelData)
	{
		
		// Helper function to add a face
		void AddFace(Vector3[] faceVertices, ref int vertexcount)
		{
			vertices.AddRange(faceVertices.Select(vector3 => vector3
			/VoxelScalar));
			var i = vertexcount;
			indices.AddRange(new[] { 2, 1, 0, 2, 3, 1 }.Select(index => index + i));
			vertexcount += faceVertices.Length;
			
			// Calculate normals using the cross product
			Vector3 edge1 = faceVertices[1] - faceVertices[0];
			Vector3 edge2 = faceVertices[2] - faceVertices[0];
			Vector3 normal = edge1.Cross(edge2).Normalized();

			normals.AddRange(new []{normal, normal, normal, normal});
		}

		var indexer = VoxelConstants.index(x, y, z + 1);
		if(indexer < 0 || voxelData[indexer] == 0)
		AddFace(new Vector3[]
			{
				new(x, y, z + 1),            // New Bottom-left
				new(x + 1, y, z + 1),        // New Top-left
				new(x, y + 1, z + 1),        // New Bottom-right
				new(x + 1, y + 1, z + 1)     // New Top-right
			}
			, ref vertexCount);
		// Back face
		
		indexer = VoxelConstants.index(x, y, z - 1);
		if(indexer  < 0 || voxelData[indexer] == 0)
		AddFace(new Vector3[]
		{
			new(x + 1, y, z),      
			new(x, y, z),          
			new(x + 1, y + 1, z),  
			new(x, y + 1, z)       
		}, ref vertexCount);

// Left face
		
		indexer = VoxelConstants.index(x-1, y, z);
		if(indexer  < 0 || voxelData[indexer] == 0)
		AddFace(new Vector3[]
		{
			new(x, y, z),
			new(x, y, z + 1),
			new(x, y + 1, z),
			new(x, y + 1, z + 1)
		},   ref vertexCount);

// Right face
		indexer = VoxelConstants.index(x+1, y, z);
		if(indexer  < 0 || voxelData[indexer] == 0)
		AddFace(new Vector3[]
		{
			new(x + 1, y, z + 1),    
			new(x + 1, y, z),        
			new(x + 1, y + 1, z + 1),
			new(x + 1, y + 1, z)     

		},   ref vertexCount);

// Top face
		indexer = VoxelConstants.index(x, y+1, z);
		if(indexer  < 0 || voxelData[indexer] == 0)
		AddFace(new Vector3[]
		{
			new(x, y + 1, z),
			new(x, y + 1, z + 1),
			new(x + 1, y + 1, z),
			new(x + 1, y + 1, z + 1)
		},   ref vertexCount);

// Bottom face
		indexer = VoxelConstants.index(x, y-1, z);
		if(indexer  < 0 || voxelData[indexer] == 0)
		AddFace(new Vector3[]
		{
			new(x, y, z),
			new(x + 1, y, z),
			new(x, y, z + 1),
			new(x + 1, y, z + 1)
		},  ref vertexCount);

	}

	public static Vector2 GetUVForByte(byte value)
	{

		int xIndex = value % tilesPerRow;
		int yIndex = value / tilesPerRow;

		float u = xIndex * tileWidth;
		float v = yIndex * tileHeight;

		return new Vector2(u, v);
	}

}