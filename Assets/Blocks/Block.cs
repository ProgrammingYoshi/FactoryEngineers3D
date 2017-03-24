using UnityEngine;
using System.Collections;

public interface Block
{
	float Density { get; }
	bool IsSolid { get; }
	bool UpdateEveryTick { get; }
	Vector3i Position { get; set; }
	void Tick(World world, Chunk chunk, float secondsSinceLastTick);
	/*/// <summary>
	/// -X +X -Y +Y -Z +Z Always Render
	/// </summary>
	/// <param name="side">Side to get vertices of</param>
	MeshPiece GetMeshForSide(int side);*/
	/// <summary>
	/// -X +X -Y +Y -Z +Z Always Render
	/// </summary>
	/// <param name="freeSides">Sides which should be rendered, 1 = -X, 2 = +X, 4 = -Y, 8 = +Y, 16 = -Z, 32 = +Z</param>
	void GetMesh(int freeSides, GrowableMesh growableMesh, Vector3 position);
	int ID { get; }
}
