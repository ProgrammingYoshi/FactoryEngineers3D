using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GrowableMesh
{
	LinkedList<Vector3> vertices = new LinkedList<Vector3>(); //TODO: Check Query vs LinkedList vs List in speed
	LinkedList<int> indices = new LinkedList<int>();
	LinkedList<Color> colors = new LinkedList<Color>();
	LinkedList<Vector2> uvs = new LinkedList<Vector2>();
	int lastIndex = 0;

	public Vector3[] GetVertices()
	{
		return vertices.ToArray();
	}
	public int[] GetIndices()
	{
		return indices.ToArray();
	}
	public Color[] GetColors()
	{
		return colors.ToArray();
	}
	public Vector2[] GetUvs()
	{
		return uvs.ToArray();
	}
	public MeshPiece ToMeshPiece()
	{
		return new MeshPiece(GetVertices(), GetIndices(), GetColors(), GetUvs());
	}
	public void AddMeshPiece(MeshPiece meshPiece, Vector3 position)
	{
		if (meshPiece.vertices.Length > 0 && meshPiece.indices.Length > 0)
		{
			for (int i = 0; i < meshPiece.vertices.Length; i++)
				vertices.AddLast(meshPiece.vertices[i] + position);
			for (int i = 0; i < meshPiece.indices.Length; i++)
				indices.AddLast(meshPiece.indices[i] + lastIndex);
			for (int i = 0; i < meshPiece.colors.Length; i++)
				colors.AddLast(meshPiece.colors[i]);
			for (int i = 0; i < meshPiece.uvs.Length; i++)
				uvs.AddLast(meshPiece.uvs[i]);
			lastIndex += meshPiece.vertices.Length;
		}
	}
	public void AddMeshPiece(Vector3[] vertices, int[] indices, Color[] colors, Vector2[] uvs)
	{
		if (vertices.Length > 0 && indices.Length > 0)
		{
			for (int i = 0; i < vertices.Length; i++)
				this.vertices.AddLast(vertices[i]);
			for (int i = 0; i < indices.Length; i++)
				this.indices.AddLast(indices[i] + lastIndex);
			for (int i = 0; i < colors.Length; i++)
				this.colors.AddLast(colors[i]);
			for (int i = 0; i < uvs.Length; i++)
				this.uvs.AddLast(uvs[i]);
			lastIndex += vertices.Length;
		}
	}
	public void AddMeshPiece(Vector3[] vertices, int[] indices, Color[] colors, Vector2[] uvs, Vector3 position)
	{
		if (vertices.Length > 0 && indices.Length > 0)
		{
			for (int i = 0; i < vertices.Length; i++)
				this.vertices.AddLast(vertices[i] + position);
			for (int i = 0; i < indices.Length; i++)
				this.indices.AddLast(indices[i] + lastIndex);
			for (int i = 0; i < colors.Length; i++)
				this.colors.AddLast(colors[i]);
			for (int i = 0; i < uvs.Length; i++)
				this.uvs.AddLast(uvs[i]);
			lastIndex += vertices.Length;
		}
	}
}

public class MeshPiece
{
	public Vector3[] vertices;
	public int[] indices;
	public Color[] colors;
	public Vector2[] uvs;

	public MeshPiece(Vector3[] vertices, int[] indices, Color[] colors, Vector2[] uvs)
	{
		this.vertices = vertices;
		this.indices = indices;
		this.colors = colors;
		this.uvs = uvs;
	}
}