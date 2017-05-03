using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Make3DTex : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Texture3D tex = new Texture3D(64, 64, 64, TextureFormat.ARGB32, false);
		Color[] newC  = new Color[64 * 64 * 64];
		for (int i = 0; i < 64; i++)
		{
			for (int j = 0; j < 64; j++)
			{
				for (int k = 0; k < 64; k++)
				{
					newC[i + j * 64 + k * 64 * 64] = new Color(0, 0, (Mathf.PerlinNoise(i / 43F, j / 37F) * Mathf.PerlinNoise(k / 43F, i / 37F))/* + Mathf.PerlinNoise(i / 64F, k / 64F) + Mathf.PerlinNoise(j / 64F, k / 64F)) / 3F*/, /*Mathf.PerlinNoise(j / 64F,i / 64F) * Mathf.PerlinNoise(k / 64F,i / 64F) * Mathf.PerlinNoise(k / 64F,j / 64F)*/1F);
				}
			}
		}
		tex.SetPixels(newC);
		tex.Apply();
		AssetDatabase.CreateAsset(tex, "Assets/myFilename.asset");
	}

}
