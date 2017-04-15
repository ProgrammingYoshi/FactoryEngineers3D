using UnityEngine;
using System.Collections;
using System.IO;

public class NoiseTest : MonoBehaviour {
	OpenSimplexNoise noise = new OpenSimplexNoise();

	void Start() {
		int[] perm = new int[16];
		for (int i = 0; i < 16; i++)
			perm[i] = Random.Range(1, 16);
		var texture = new Texture2D(512, 512);
		for (int x = 0; x < 512; x++)
			for (int y = 0; y < 512; y++)
			{
				Color color = Color.black;
				for (int i = 0; i < 16; i++)
					color += Color.HSVToRGB(i / 16F, 1, (float)OctaveNoise(x / (float)(/*1 <<*/ perm[i]) + perm[i] * 16, y / (float)(/*1 <<*/ perm[i]) + perm[i] * 16, 8, i / 16D));
				texture.SetPixel(x, y, color / 4F);// new Color((float)OctaveNoise(x, y, 8), (float)OctaveNoise(x / 2D, y / 2D, 8), (float)OctaveNoise(x / 4D, y / 4D, 8)));
			}
		Save(texture);
	}

	double OctaveNoise(double x, double y, int octaves)
	{
		double result = 0;
		int save = 0;
		if (octaves < 32)
			for (int i = 1; i < (1 << octaves); i = (i << 1))
			{
				save++;
				if (save > 100) break;
				result += noise.Evaluate(x / i, y / i);
	        }
		return result / octaves;
	}

	double OctaveNoise(double x, double y, int octaves, double bias)
	{
		double result = 0;
		int save = 0;
		if (octaves < 32)
			for (int i = 1; i < (1 << octaves); i = (i << 1))
			{
				save++;
				if (save > 100) break;
				result += noise.Evaluate(x / i, y / i, bias);
			}
		return result / octaves;
	}

	void Update() {

	}
	public void Save(Texture2D texture)
	{
		byte[] bytes = texture.EncodeToPNG();
		File.WriteAllBytes("Z:\\testNoise2.png", bytes);
	}
}
