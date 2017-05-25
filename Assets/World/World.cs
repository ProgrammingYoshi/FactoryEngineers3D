using UnityEngine;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Concurrent;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[Serializable]
public class World : MonoBehaviour
{
	public const float gravity = 9.80665F;
	const float shift = 0.01F;
	public int chunksX = 8, chunksY = 8, chunksZ = 8, physicsDivisor = 1;
	Vector3i selectedPosition;
	bool blockSelected = false;
	MeshFilter meshFilter;
	public Material[] materials;
	int sizeX, sizeY, sizeZ;
	public Chunk[,,] chunks;
	GameObject[,,] renderGrids;
	ConcurrentQueue<KeyValuePair<Vector3i, Vector3i>> blocksToSwap = new ConcurrentQueue<KeyValuePair<Vector3i, Vector3i>>();
	public int SizeX { get { return sizeX; } }
	public int SizeY { get { return sizeY; } }
	public int SizeZ { get { return sizeZ; } }
	public Vector3 SelectedPosition { get { return selectedPosition; } }
	public Block BlockAtSelection
	{
		get
		{
			return blockSelected ? GetBlock(selectedPosition) : null;
		}
		set
		{
			if (blockSelected) SetBlock(selectedPosition, value);
		}
	}

	public Block GetBlock(Vector3i position)
	{
		if (IsInWorld(position))
			return chunks[position.x >> Chunk.log2ChunkSize, position.y >> Chunk.log2ChunkSize, position.z >> Chunk.log2ChunkSize].GetBlock(position);
		else
			return null;
	}

	public Block GetBlock(Vector3i position, Chunk chunk)
	{
		return chunk.GetBlock(position);
	}

	public void SetBlock(Vector3i position, Block block)
	{
		if (!IsInWorld(position)) throw new System.Exception("Block out of world"); //TODO: Add exception class with block etc. as variables
		int x = position.x, y = position.y, z = position.z;
		chunks[x >> Chunk.log2ChunkSize, y >> Chunk.log2ChunkSize, z >> Chunk.log2ChunkSize].SetBlock(position, block);
	}

	public bool IsSolid(Vector3i position)
	{
		if (!IsInWorld(position))
			throw new System.Exception("Block out of world");
		return chunks[position.x >> Chunk.log2ChunkSize, position.y >> Chunk.log2ChunkSize, position.z >> Chunk.log2ChunkSize].IsSolid(position);
	}

	public bool IsSolid(Vector3i position, Chunk chunk)
	{
		if (!IsInWorld(position))
			throw new System.Exception("Block out of world");
		return chunk.IsSolid(position);
	}

	public void Select(Vector3 position, Color color)
	{
		position = Global.Floor(position);
		selectedPosition = (Vector3i)position;
		Vector3[] cubeVertices = { new Vector3(-shift, -shift, -shift) + position, new Vector3(1 + shift, -shift, -shift) + position, new Vector3(-shift, 1 + shift, -shift) + position, new Vector3(1 + shift, 1 + shift, -shift) + position, new Vector3(-shift, -shift, 1 + shift) + position, new Vector3(1 + shift, -shift, 1 + shift) + position, new Vector3(-shift, 1 + shift, 1 + shift) + position, new Vector3(1 + shift, 1 + shift, 1 + shift) + position };
		meshFilter.mesh.vertices = cubeVertices;
		Color[] cubeColors = { color, color, color, color, color, color, color, color };
		meshFilter.mesh.colors = cubeColors;
		meshFilter.mesh.RecalculateBounds();
		blockSelected = true;
	}

	public void Deselect()
	{
		Select(new Vector3(-1, -1, -1), Color.clear);
		blockSelected = false;
	}

	void Start()
	{
		chunkTickCount = chunksX * chunksY * chunksZ;

		meshFilter = GetComponent<MeshFilter>();
		Vector3[] cubeVertices = { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 1), new Vector3(0, 1, 1), new Vector3(1, 1, 1) };
		meshFilter.mesh.vertices = cubeVertices;
		int[] cubeIndices = { 0, 1, 1, 3, 3, 2, 2, 0, 4, 5, 5, 7, 7, 6, 6, 4, 0, 4, 1, 5, 2, 6, 3, 7 };
		meshFilter.mesh.SetIndices(cubeIndices, MeshTopology.Lines, 0);
		Color[] cubeColors = { Color.clear, Color.clear, Color.clear, Color.clear, Color.clear, Color.clear, Color.clear, Color.clear };
		meshFilter.mesh.colors = cubeColors;

		sizeX = chunksX * Chunk.chunkSize;
		sizeY = chunksY * Chunk.chunkSize;
		sizeZ = chunksZ * Chunk.chunkSize;
		renderGrids = new GameObject[chunksX, chunksY, chunksZ];
		chunks = new Chunk[chunksX, chunksY, chunksZ];
		for (int x = 0; x < chunksX; x++)
			for (int y = 0; y < chunksY; y++)
				for (int z = 0; z < chunksZ; z++)
				{
					renderGrids[x, y, z] = new GameObject("Chunk " + (x + y * chunksX + z * chunksY * chunksX), typeof(Chunk));
					renderGrids[x, y, z].GetComponent<MeshRenderer>().materials = materials;
					renderGrids[x, y, z].transform.position = new Vector3(x * Chunk.chunkSize, y * Chunk.chunkSize, z * Chunk.chunkSize);
					Chunk chunk = renderGrids[x, y, z].GetComponent<Chunk>();
					chunk.position = new Vector3i(x, y, z);
					chunk.world = this;
					chunks[x, y, z] = chunk;
				}
		Generate();
	}

	void Generate()
	{
		for (int x = 0; x < sizeX; x++)
			for (int y = 0; y < sizeY; y++)
				for (int z = 0; z < sizeZ; z++)
				{
					if (y / 16F < Mathf.PerlinNoise(x / 32F, z / 32F + Mathf.PerlinNoise(x / 17F, z / 19F)))
						SetBlock(new Vector3i(x, y, z), new DirtBlock(new Vector3i(x, y, z)));
					if (x > 5 && y > 15 && z > 5 && x < 10 && y < 20 && z < 10)
						SetBlock(new Vector3i(x, y, z), new FluidBlock(new Vector3i(x, y, z)));
					/*if (x > 5 && y > 15 && z > 5 && x < 25 && y < 35 && z < 25)
						SetBlock(new Vector3i(x, y, z), new FluidBlock(new Vector3i(x, y, z)));
					/*if (y == 0)//if (y < 10 && Mathf.Sin(x) * Mathf.Sin(y / 2) * Mathf.Sin(z) > 0)
						SetBlock(new Vector3i(x, y, z), new DirtBlock(new Vector3i(x, y, z)));*/
				}
	}

	int cnt = 0;
	Stopwatch timer = new Stopwatch();
	Stopwatch test = new Stopwatch();
	long thisFrame = 0, lastFrame = 0;
	public int chunkTickCount;
	void Update()
	{
		timer.Start();
		if (true || cnt >= physicsDivisor)
		{
			cnt = 0;
			if (true)
			{
				Tick();
				chunkTickCount = 0;
				thisFrame = timer.ElapsedTicks;
				for (int x = 0; x < chunksX; x++)
					for (int y = 0; y < chunksY; y++)
						for (int z = 0; z < chunksZ; z++)
							chunks[x, y, z].TickChunk((Time.deltaTime/*thisFrame - lastFrame*/) /*/ 10000000F*/);
				lastFrame = thisFrame;
			}
			/*test.Start();*/
			for (int x = 0; x < chunksX; x++)
				for (int y = 0; y < chunksY; y++)
					for (int z = 0; z < chunksZ; z++)
						chunks[x, y, z].UpdateChunk();
			/*test.Stop();
			UnityEngine.Debug.Log(test.ElapsedMilliseconds);
			test.Reset();*/
		}
		cnt++;
	}

	void Tick()
	{
		/*for (int x = 0; x < sizeX; x++)
			for (int y = 0; y < sizeY; y++)
				for (int z = 0; z < sizeZ; z++)
					if (blocks[x, y, z] != null && blocks[x, y, z].UpdateEveryTick)
							blocks[x, y, z].Tick(this, 10000000F / (thisFrame - lastFrame));*/
		Vector3i a, b;
		KeyValuePair<Vector3i, Vector3i> kvp;
		//for (int i = 0; i < blocksToSwap.Count; i++)
		while(!blocksToSwap.IsEmpty)
		{
			blocksToSwap.TryDequeue(out kvp);
			a = kvp.Key;
			b = kvp.Value;
			Block tmp = GetBlock(new Vector3i(a.x, a.y, a.z));
			SetBlock(a, GetBlock(new Vector3i(b.x, b.y, b.z)));
			SetBlock(b, tmp);
		}
		/*foreach (KeyValuePair<Vector3i, Vector3i> swap in blocksToSwap)
		{
			Block tmp = GetBlock(new Vector3i(swap.Value.x, swap.Value.y, swap.Value.z));
			SetBlock(swap.Value, GetBlock(new Vector3i(swap.Key.x, swap.Key.y, swap.Key.z)));
			SetBlock(swap.Key, tmp);
		}*/
	}

	public bool CanTravel(Vector3i start, Vector3i end)
	{
		Vector3i relative = start - end;
		float distance = relative.x + relative.y + relative.z;
		for (float f = 0; f < 1f; f += 1F / distance)
			if (GetBlock(start + relative * f) != null) return false;
		return true;
	}

	public Vector3i CanTravelTo(Vector3i start, Vector3i end)
	{
		Vector3i relative = start - end;
		Vector3i lastBlock = start, block;
		float distance = relative.x + relative.y + relative.z;
		for (float f = 0; f < 1f; f += 1F / distance)
		{
			block = start + relative * f;
			if (GetBlock(block) != null)
			{
				return lastBlock;
			}
			lastBlock = block;
		}
		return end;
	}

	/// <summary>
	/// Schedules a block swap
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	public void SwapBlocks(Vector3i a, Vector3i b)
	{
		blocksToSwap.Enqueue(new KeyValuePair<Vector3i, Vector3i>(a, b));
	}

	public bool IsInWorld(Vector3i position)
	{
		return position.x > -1 && position.x < sizeX && position.y > -1 && position.y < sizeY && position.z > -1 && position.z < sizeZ;
	}

	public bool IsBlockThere(Vector3i position)
	{
		return GetBlock(position) != null;
	}

	public int GetFreeSides(Vector3i position)
	{
		if (GetBlock(position) == null)
			UnityEngine.Debug.Log(position);
		Type blockType = GetBlock(position).GetType();
		return (IsBlockThere(position + Vector3i.left) && GetBlock(position + Vector3i.left).GetType() == blockType ? 0 : 1) |
			(IsBlockThere(position + Vector3i.right) && GetBlock(position + Vector3i.right).GetType() == blockType ? 0 : 2) |
			(IsBlockThere(position + Vector3i.down) && GetBlock(position + Vector3i.down).GetType() == blockType ? 0 : 4) |
			(IsBlockThere(position + Vector3i.up) && GetBlock(position + Vector3i.up).GetType() == blockType ? 0 : 8) |
			(IsBlockThere(position + Vector3i.back) && GetBlock(position + Vector3i.back).GetType() == blockType ? 0 : 16) |
			(IsBlockThere(position + Vector3i.front) && GetBlock(position + Vector3i.front).GetType() == blockType ? 0 : 32);
	}

	public int GetFreeSides2(Vector3i position) //TODO
	{
		return (IsSolid(position + Vector3i.left) ? 0 : 1) |
			(IsSolid(position + Vector3i.right) ? 0 : 2) |
			(IsSolid(position + Vector3i.down) ? 0 : 4) |
			(IsSolid(position + Vector3i.up) ? 0 : 8) |
			(IsSolid(position + Vector3i.back) ? 0 : 16) |
			(IsSolid(position + Vector3i.front) ? 0 : 32);
	}

	public void Save(string directory)
	{
		if (!Directory.Exists(directory))
			Directory.CreateDirectory(directory);
		for (int x = 0; x < chunksX; x++)
			for (int y = 0; y < chunksY; y++)
				for (int z = 0; z < chunksZ; z++)
				{
					chunks[x, y, z].Save(string.Format("{0}\\{1},{2},{3}", directory, x.ToString(), y.ToString(), z.ToString()));
				}
	}

	public void Load(string directory)
	{
		for (int x = 0; x < chunksX; x++)
			for (int y = 0; y < chunksY; y++)
				for (int z = 0; z < chunksZ; z++)
				{
					chunks[x, y, z].Load(string.Format("{0}\\{1},{2},{3}", directory, x.ToString(), y.ToString(), z.ToString()));
				}
	}
}
