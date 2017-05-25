using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class FluidBlock : CubeBlock
{
	public override bool IsSolid
	{
		get
		{
			return false;
		}
	}

	public override bool UpdateEveryTick
	{
		get
		{
			return true;
		}
	}

	static FluidBlock()
	{
		textureID = AssignTexture(1, 0);
	}

	public FluidBlock(Vector3i position) : base(position)
	{
	}

	public override int ID { get { return 3; } }
	private static int textureID;
	public override int TextureID { get { return textureID; } }

	public override void Tick(World world, Chunk chunk, float secondsSinceLastTick)
	{
		//if (position.y > 16) Debug.Log("Pos over 16");
		/*int i = Random.Range(-1, 2);
		int j = Random.Range(-1, 2);
		Block blockUnderThis = world.GetBlock(x - i, y - 1, z - j);
		Block blockBesidesThis = world.GetBlock(x - i, y, z - j);
		if (world.IsInWorld(x - i, y - 1, z - j) && (blockUnderThis == null || !blockUnderThis.IsSolid && Density > blockUnderThis.Density))
			world.SwapBlocks(x, y, z, x - i, y - 1, z - j);
		else if (world.IsInWorld(x - i, y, z - j) && (blockBesidesThis == null ||!blockBesidesThis.IsSolid && Density > blockBesidesThis.Density))
			world.SwapBlocks(x, y, z, x - i, y, z - j);*/
		Vector3i BlockUnderThisPosition = position + Vector3i.down;
		Vector3i BlockBesidesThisPosition = Vector3i.zero;
		Block blockUnderThis = world.GetBlock(BlockUnderThisPosition);
		if (world.IsInWorld(BlockUnderThisPosition) && (blockUnderThis == null || !blockUnderThis.IsSolid && Density > blockUnderThis.Density))
		{
			world.SwapBlocks(position, BlockUnderThisPosition);
			return;
		}

		List<Vector3i> freeSides = new List<Vector3i>(4);
		Vector3i tempPosition;
		tempPosition = position + Vector3i.right;
		if (world.IsInWorld(tempPosition) && world.GetBlock(tempPosition) == null)
			freeSides.Add(tempPosition);
		tempPosition = position + Vector3i.left;
		if (world.IsInWorld(tempPosition) && world.GetBlock(tempPosition) == null)
			freeSides.Add(tempPosition);
		tempPosition = position + Vector3i.front;
		if (world.IsInWorld(tempPosition) && world.GetBlock(tempPosition) == null)
			freeSides.Add(tempPosition);
		tempPosition = position + Vector3i.back;
		if (world.IsInWorld(tempPosition) && world.GetBlock(tempPosition) == null)
			freeSides.Add(tempPosition);
		if (freeSides.Count >= 1)
			world.SwapBlocks(position, freeSides[StaticRandom.Next(freeSides.Count - 1)]);
		else if (freeSides.Count == 1)
			world.SwapBlocks(position, freeSides[0]);
	}
}
