using UnityEngine;
using System.Collections;

public class Statistics : MonoBehaviour {
	public long totalChunkRenders, totalBlockTicks, lastFrameChunkRenders, lastFrameBlockTicks, chunkRenderCounter, blockTickCounter;
	public double chunkRenderTime, blockTickTime;

	public Statistics()
	{
		if (Global.statistics == null)
			Global.statistics = this;
		else
			throw new System.Exception("Statistics variable already assigned");
	}

	void Update ()
	{
		lastFrameChunkRenders = chunkRenderCounter;
		chunkRenderCounter = 0;
		lastFrameBlockTicks = blockTickCounter;
		blockTickCounter = 0;
	}
	
	public void ChunkRender()
	{
		chunkRenderCounter++;
		totalChunkRenders++;
    }

	public void BlockTick()
	{
		blockTickCounter++;
		totalBlockTicks++;
	}

	public void AddChunkRenderTime(double time)
	{
		chunkRenderTime += time;
	}

	public void AddBlockTickTime(double time)
	{
		blockTickTime += time;
	}
}
