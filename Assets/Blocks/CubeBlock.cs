using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class CubeBlock : Block
{
	protected static readonly List<Vector2[][]> textureCoords = new List<Vector2[][]>();
	protected static readonly Vector3[][] blockCoordinates =
	{
		new Vector3[] {new Vector3(0,0,0),new Vector3(0,1,0),new Vector3(0,0,1),new Vector3(0,1,1)},
		new Vector3[] {new Vector3(1,0,0),new Vector3(1,1,0),new Vector3(1,0,1),new Vector3(1,1,1)},
		new Vector3[] {new Vector3(0,0,0),new Vector3(1,0,0),new Vector3(0,0,1),new Vector3(1,0,1)},
		new Vector3[] {new Vector3(0,1,0),new Vector3(1,1,0),new Vector3(0,1,1),new Vector3(1,1,1)},
		new Vector3[] {new Vector3(0,0,0),new Vector3(1,0,0),new Vector3(0,1,0),new Vector3(1,1,0)},
		new Vector3[] {new Vector3(0,0,1),new Vector3(1,0,1),new Vector3(0,1,1),new Vector3(1,1,1)},
		new Vector3[] {},
	};
	protected static readonly Color[][] colors =
	{
		new Color[] {new Color(0,1,0),new Color(0,0,1),new Color(1,0,0),new Color(1,1,1),},
		new Color[] {new Color(0,1,0),new Color(0,0,1),new Color(1,0,0),new Color(1,1,1),},
		new Color[] {new Color(0,1,0),new Color(0,0,1),new Color(1,0,0),new Color(1,1,1),},
		new Color[] {new Color(0,1,0),new Color(0,0,1),new Color(1,0,0),new Color(1,1,1),},
		new Color[] {new Color(0,1,0),new Color(0,0,1),new Color(1,0,0),new Color(1,1,1),},
		new Color[] {new Color(0,1,0),new Color(0,0,1),new Color(1,0,0),new Color(1,1,1),},
		new Color[] {},
	};
	protected static readonly Vector2[][] uvTemplate =
	{
		new Vector2[] { new Vector2(0,0), new Vector2(1,0), new Vector2(0,1), new Vector2(1,1), },
		new Vector2[] { new Vector2(0,0), new Vector2(1,0), new Vector2(0,1), new Vector2(1,1), },
		new Vector2[] { new Vector2(0,0), new Vector2(1,0), new Vector2(0,1), new Vector2(1,1), },
		new Vector2[] { new Vector2(0,0), new Vector2(1,0), new Vector2(0,1), new Vector2(1,1), },
		new Vector2[] { new Vector2(0,0), new Vector2(1,0), new Vector2(0,1), new Vector2(1,1), },
		new Vector2[] { new Vector2(0,0), new Vector2(1,0), new Vector2(0,1), new Vector2(1,1), },
		new Vector2[] {},
	};
	//-X +X -Y +Y -Z +Z 0,0 0,1 1,0 1,1
	protected static readonly int[][] blockSides =
	{
		new int[] {0,2,1,1,2,3,},
		new int[] {0,1,2,1,3,2,},
		new int[] {0,1,2,1,3,2,},
		new int[] {0,2,1,1,2,3,},
		new int[] {0,2,1,1,2,3,},
		new int[] {0,1,2,1,3,2,},
		new int[] {},
	};
	protected Vector3i position;
	public virtual float Density
	{
		get { return 1; }
	}
	public virtual bool IsSolid
	{
		get { return true; }
	}
	public virtual bool UpdateEveryTick
	{
		get { return false; }
	}
	public virtual int ID { get { return 0; } }
	public virtual Vector3i Position { get { return position; } set { position = value; } }
	private static int textureID;
	public virtual int TextureID { get { return textureID; } }

	/// <summary>
	/// Assings texture to block
	/// </summary>
	/// <param name="textureX"></param>
	/// <param name="textureY"></param>
	/// <returns>Texture ID</returns>
	protected static int AssignTexture(int textureX, int textureY)
	{
		Vector2[][] uvs = new Vector2[6][];
        for (int i = 0; i < 6; i++)
		{
			uvs[i] = new Vector2[4];
			for (int j = 0; j < 4; j++)
			{
				uvs[i][j].x = uvTemplate[i][j].x * (1 / 16F) + ((textureX) / 16F);
				uvs[i][j].y = uvTemplate[i][j].y * (1 / 16F) + (15 - textureY) / 16F;
			}
		}
		textureCoords.Add(uvs);
		return textureCoords.Count - 1;
    }

	static CubeBlock()
	{
		textureID = AssignTexture(0, 0);
	}

	public CubeBlock(Vector3i position)
	{
		this.position = position;
    }

	/*/// <summary>
	/// -X +X -Y +Y -Z +Z Always Render
	/// </summary>
	/// <param name="side">Side to get vertices of</param>
	public MeshPiece GetMeshForSide(int side)
	{
		return new MeshPiece(blockCoordinates[side], blockSides[side], colors[side]);
	}*/

	public virtual void Tick(World world, Chunk chunk,float secondsSinceLastTick)
	{
		throw new NotImplementedException();
	}

	public virtual bool IsOpaque(int side)
	{
		return true;
	}

	public void GetMesh(int freeSides, GrowableMesh growableMesh, Vector3 position)
	{
		if ((freeSides & 63) == 0) return;
		if ((freeSides & 1) == 1)
			growableMesh.AddMeshPiece(blockCoordinates[0], blockSides[0], colors[0], textureCoords[TextureID][0], position);
		if ((freeSides & 2) == 2)
			growableMesh.AddMeshPiece(blockCoordinates[1], blockSides[1], colors[1], textureCoords[TextureID][1], position);
		if ((freeSides & 4) == 4)
			growableMesh.AddMeshPiece(blockCoordinates[2], blockSides[2], colors[2], textureCoords[TextureID][2], position);
		if ((freeSides & 8) == 8)
			growableMesh.AddMeshPiece(blockCoordinates[3], blockSides[3], colors[3], textureCoords[TextureID][3], position);
		if ((freeSides & 16) == 16)
			growableMesh.AddMeshPiece(blockCoordinates[4], blockSides[4], colors[4], textureCoords[TextureID][4], position);
		if ((freeSides & 32) == 32)
			growableMesh.AddMeshPiece(blockCoordinates[5], blockSides[5], colors[5], textureCoords[TextureID][5], position);
	}
}
