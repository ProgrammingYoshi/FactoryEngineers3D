using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
	public const int log2ChunkSize = 4;
	public const int chunkSize = 1 << log2ChunkSize;
	public const int chunkSizeMask = chunkSize - 1;
	public World world;
	public Vector3i position; //In chunk coordinates
	public bool render;
	float sqrt3 = Mathf.Sqrt(3);
	/*bool[,,] wasSolidInLastFrame = new bool[chunkSize, chunkSize, chunkSize];*/
	MeshFilter meshFilter;
	MeshCollider meshCollider;
	Block[,,] blocks;
	bool[,,] isNotNull;
	bool[,,] solids;
	bool isEmpty = true;

	public Chunk()
	{
		blocks = new Block[chunkSize, chunkSize, chunkSize];
		solids = new bool[chunkSize, chunkSize, chunkSize];
		isNotNull = new bool[chunkSize, chunkSize, chunkSize];
	}

	void Start()
	{
		Vector3[] vertices = new Vector3[4096];
		Vector3[] normals = new Vector3[4096];
		/*for (int x = 0; x < chunkSize; x++)
			for (int y = 0; y < chunkSize; y++)
				for (int z = 0; z < chunkSize; z++)
					for (int i = 0; i < chunkSize; i++)
					{
						vertices[i + x * chunkSize + y * 64 + z * 512] = new Vector3(x, y, z) + blockCoordinates[i];
						normals[i + x * chunkSize + y * 64 + z * 512] = BlockCoordinateToNormal(blockCoordinates[i]);
					}*/
		meshFilter = GetComponent<MeshFilter>();
		meshFilter.mesh.vertices = vertices;
		meshFilter.mesh.normals = normals; //TODO: Evaluate and remove if applicable
		meshCollider = GetComponent<MeshCollider>();
		meshCollider.sharedMesh = new Mesh();
		RefreshChunk();
	}

	/*Vector3 BlockCoordinateToNormal(Vector3 coordinate) TODO: What the fuck is this?
	{
		return new Vector3(coordinate.x == 1 ? sqrt3 : -sqrt3, coordinate.y == 1 ? sqrt3 : -sqrt3, coordinate.z == 1 ? sqrt3 : -sqrt3);
	}*/
	
	public void TickChunk(float timeElapsed)
	{
		if (!isEmpty)
		{
			Thread thread = new Thread(_InternalTickChunk);
			thread.Start(timeElapsed);
		}
		else
			world.chunkTickCount++;
	}

	void _InternalTickChunk(object timeElapsed)
	{
		for (int x = 0; x < chunkSize; x++)
			for (int y = 0; y < chunkSize; y++)
				for (int z = 0; z < chunkSize; z++)
					if (isNotNull[x, y, z] && blocks[x, y, z].UpdateEveryTick)
					{
						try
						{
							blocks[x, y, z].Tick(world, this, (float)timeElapsed);
							//Global.statistics.BlockTick();
						}
						catch(Exception ex)
						{
							File.WriteAllText(DateTime.Now.Ticks.ToString(), ex.ToString());
						}
					}
		//Global.statistics.AddBlockTickTime((float)timeElapsed);
		world.chunkTickCount++;
	}

	public void UpdateChunk()
	{
		if (!isEmpty && render)
		{
			RefreshChunk();
			render = false;
		}
	}

	Stopwatch test = new Stopwatch();
	public int threadFinishCount = 0;
	void RefreshChunk()
	{
		Global.statistics.ChunkRender();
		GrowableMesh growableMesh = new GrowableMesh();
		Vector3i renderPosition;
		int index = 0;

		test.Start();
		for (int x = 0; x < chunkSize; x++) //TODO: Multithread this
			for (int y = 0; y < chunkSize; y++) //TODO: For real do multithread this
				for (int z = 0; z < chunkSize; z++)
				{
					renderPosition = new Vector3i(x, y, z);
					if (isNotNull[x, y, z])
					{
						int freeSides = world.GetFreeSides(renderPosition + (position << log2ChunkSize));
						blocks[x, y, z].GetMesh(freeSides, growableMesh, new Vector3(x, y, z));
					}
				}

		test.Stop();
		index += chunkSize;
		Mesh colliderMesh = new Mesh();
		colliderMesh.vertices = growableMesh.GetVertices();
		colliderMesh.triangles = growableMesh.GetIndices();
		meshCollider.sharedMesh = colliderMesh;
		Mesh mesh = new Mesh();
		mesh.vertices = growableMesh.GetVertices();
		mesh.triangles = growableMesh.GetIndices();
		mesh.colors = growableMesh.GetColors();
		mesh.uv = growableMesh.GetUvs();
		mesh.RecalculateNormals(); //TODO: Add normals to block class
		meshFilter.mesh = mesh;
		if (test.ElapsedMilliseconds > 0)
			UnityEngine.Debug.Log(test.ElapsedMilliseconds);
		test.Reset();
	}
	
	bool IsSideFree(Vector3i position, int side)
	{
		switch (side)
		{
			case 0:
				return world.GetBlock(position, this).GetType() != world.GetBlock(position + Vector3i.left, this).GetType();
			case 1:
				return world.GetBlock(position, this).GetType() != world.GetBlock(position + Vector3i.right, this).GetType();
			case 2:
				return world.GetBlock(position, this).GetType() != world.GetBlock(position + Vector3i.down, this).GetType();
			case 3:
				return world.GetBlock(position, this).GetType() != world.GetBlock(position + Vector3i.up, this).GetType();
			case 4:
				return world.GetBlock(position, this).GetType() != world.GetBlock(position + Vector3i.back, this).GetType();
			case 5:
				return world.GetBlock(position, this).GetType() != world.GetBlock(position + Vector3i.front, this).GetType();
			default:
				return true;
		}
	}

	/// <summary>
	/// Gets a block in this chunk.
	/// </summary>
	/// <param name="position">In world coordinates</param>
	public Block GetBlock(Vector3i position)
	{
		if (!IsInChunk(position - (this.position << log2ChunkSize)))
			throw new Exception("Block out of chunk: " + position.ToString());
		return blocks[position.x & chunkSizeMask, position.y & chunkSizeMask, position.z & chunkSizeMask];
	}

	/// <summary>
	/// Gets a block in this chunk and has no error checking
	/// </summary>
	/// <param name="position">In chunk or world coordinates</param>
	/// <returns></returns>
	public Block GetBlockNoError(Vector3i position)
	{
		return blocks[position.x & chunkSizeMask, position.y & chunkSizeMask, position.z & chunkSizeMask];
	}

	/// <summary>
	/// Sets a block in this chunk.
	/// </summary>
	/// <param name="position">In world coordinates</param>
	public void SetBlock(Vector3i position, Block block)
	{
		if(isEmpty && block != null) isEmpty = false;
		if (!IsInChunk(position - (this.position << log2ChunkSize))) throw new Exception("Block out of chunk: " + position.ToString() + transform.name); //TODO: Add exception class with block etc. as variables
		if (block != null)
			block.Position = position;
		int x = position.x & chunkSizeMask, y = position.y & chunkSizeMask, z = position.z & chunkSizeMask;
		blocks[x, y, z] = block;
		solids[x, y, z] = block == null ? false : block.IsSolid;
		isNotNull[x, y, z] = block != null;
		render = true;
		if (x % chunkSize == 0 && x >= chunkSize)
			world.chunks[x / chunkSize - 1, y / chunkSize, z / chunkSize].render = true;
		else if (x % chunkSize == chunkSize - 1 && x < world.SizeX - chunkSize)
			world.chunks[x / chunkSize + 1, y / chunkSize, z / chunkSize].render = true;

		if (y % chunkSize == 0 && y >= chunkSize)
			world.chunks[x / chunkSize, y / chunkSize - 1, z / chunkSize].render = true;
		else if (y % chunkSize == chunkSize - 1 && y < world.SizeY - chunkSize)
			world.chunks[x / chunkSize, y / chunkSize + 1, z / chunkSize].render = true;

		if (z % chunkSize == 0 && z >= chunkSize)
			world.chunks[x / chunkSize, y / chunkSize, z / chunkSize - 1].render = true;
		else if (z % chunkSize == chunkSize - 1 && z < world.SizeZ - chunkSize)
			world.chunks[x / chunkSize, y / chunkSize, z / chunkSize + 1].render = true;
	}

	/*bool isSolidSideFree(Vector3i position, int side)
	{
		switch (side)
		{
			case 0:
				return !(world.IsSolid(position, this) && world.IsSolid(position + Vector3i.left, this));
			case 1:																		
				return !(world.IsSolid(position, this) && world.IsSolid(position + Vector3i.right, this));
			case 2:																		
				return !(world.IsSolid(position, this) && world.IsSolid(position + Vector3i.down, this));
			case 3:																		
				return !(world.IsSolid(position, this) && world.IsSolid(position + Vector3i.up, this));
			case 4:																		
				return !(world.IsSolid(position, this) && world.IsSolid(position + Vector3i.back, this));
			case 5:																		
				return !(world.IsSolid(position, this) && world.IsSolid(position + Vector3i.forward, this));
			default:
				return true;
		}
	}*/

	/// <summary>
	/// In chunk coordinates
	/// </summary>
	public bool IsInChunk(Vector3i position)
	{
		return position.x > -1 && position.x < chunkSize && position.y > -1 && position.y < chunkSize && position.z > -1 && position.z < chunkSize;
	}

	/// <summary>
	/// In chunk coordinates
	/// </summary>
	public bool IsInChunk(int x, int y, int z)
	{
		return x > -1 && x < chunkSize && y > -1 && y < chunkSize && z > -1 && z < chunkSize;
	}

	public bool IsSolid(Vector3i position)
	{
		if (IsInChunk(position - (this.position << log2ChunkSize)))
		{
			return solids[position.x & chunkSizeMask, position.y & chunkSizeMask, position.z & chunkSizeMask];
		}
		else
			throw new Exception("Block out of chunk");
	}

	public void Load(string path)
	{
		IFormatter formatter = new BinaryFormatter();
		Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
		isNotNull = (bool[,,])formatter.Deserialize(stream);
		blocks = (Block[,,])formatter.Deserialize(stream);
		stream.Close();
		render = true;
	}

	public void Save(string path)
	{
		IFormatter formatter = new BinaryFormatter();
		Stream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
		formatter.Serialize(stream, isNotNull);
		formatter.Serialize(stream, blocks);
		stream.Close();
	}
}
