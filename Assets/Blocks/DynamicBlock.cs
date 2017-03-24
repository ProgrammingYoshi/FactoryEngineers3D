using UnityEngine;
using System.Collections;

public class DynamicBlock : CubeBlock
{
	Vector3 precisePosition;
	Vector3 velocity;

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
	public override Vector3i Position { get { return position; } set { position = value; precisePosition = value; } }
	public override int ID { get { return 2; } }

	public DynamicBlock(Vector3i position) : base(position)
	{
		for (int i = 0; i < 6; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				colors[i][j].r = Random.value;
				colors[i][j].g = Random.value;
				colors[i][j].b = Random.value;
			}
		}
	}

	public override void Tick(World world, Chunk chunk, float secondsSinceLastTick)
	{
		velocity.y += World.gravity * secondsSinceLastTick;
		Vector3i endPosition = position + (Vector3i)velocity * secondsSinceLastTick;
		if (position != endPosition)
		{
			if(world.CanTravel(position, endPosition))
			{
				precisePosition += velocity;
				world.SwapBlocks(position, endPosition);
			}
			else
			{
				endPosition = world.CanTravelTo(position, endPosition);
				precisePosition = endPosition;
				world.SwapBlocks(position, endPosition);
				velocity = Vector3.zero;
			}
		}
		//Do setblock and swap stuff here
	}
}
