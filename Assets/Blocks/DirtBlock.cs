using UnityEngine;
using System.Collections;

public class DirtBlock : CubeBlock {
	static DirtBlock()
	{
		textureID = AssignTexture(0, 0);
	}

	public DirtBlock(Vector3i position) : base(position)
	{
	}

	public override int ID { get { return 1; } }
	private static int textureID;
	public override int TextureID { get { return textureID; } }
}
