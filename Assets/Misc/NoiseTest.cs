using UnityEngine;
using System.Collections;
using System.IO;

public class NoiseTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		TilingPerlinNoise noise = new TilingPerlinNoise();
		noise.Test();
		Save(noise.GetTexture(256, 256, 3, 6));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void Save(Texture2D texture)
	{
		byte[] bytes = texture.EncodeToPNG();
		File.WriteAllBytes("Z:\\testNoise.png", bytes);
	}
}
