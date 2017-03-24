using System;
using System.Collections.Generic;
using System.Linq;

public class TilingPerlinNoise {
	Random random = new Random();
	int[] perm = new int[256];
	KeyValuePair<double, double>[] directions = new KeyValuePair<double, double>[256];

	public void Test()
	{
		int i = 0;
		perm = perm.Select(v => i++).ToArray(); //Make array from 0 to 255
		Shuffle(perm);
		int[] tmp = new int[512];
		for (i = 0; i < 256; i++) //perm += perm
		{
			tmp[i] = perm[i];
			tmp[i + 256] = perm[i];
		}
		perm = tmp;
		i = 0;
		directions = directions.Select(d => new KeyValuePair<double, double>(Math.Cos(i * 2.0 * Math.PI / 256), Math.Sin(i++ * 2.0 * Math.PI / 256))).ToArray();
	}

	public double Noise(double x, double y, double per)
	{
		return Surflet(x, y, per) + Surflet(x + 1, y, per) + Surflet(x, y + 1, per) + Surflet(x + 1, y + 1, per);
    }

	double Surflet(double x, double y, double per)
	{
		double gridX = Math.Floor(x), gridY = Math.Floor(y);
		double distX = Math.Abs(x - gridX), distY = Math.Abs(y - gridY);
		double polyX = 1 - 6 * Math.Pow(distX, 5) + 15 * Math.Pow(distX, 4) - 10 * Math.Pow(distX, 3);
		double polyY = 1 - 6 * Math.Pow(distY, 5) + 15 * Math.Pow(distY, 4) - 10 * Math.Pow(distY, 3);
		int hashed = perm[perm[(int)(gridX % per)] + (int)(gridY % per)];
		double grad = (x - gridX) * directions[hashed].Key + (y - gridY) * directions[hashed].Value;
		return polyX * polyY * grad;
    }

	void Shuffle(int[] array) //https://www.dotnetperls.com/shuffle
	{
		List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>(array.Length);
		foreach (int o in array)
			list.Add(new KeyValuePair<int, int>(random.Next(), o));
		var sorted = from item in list orderby item.Key select item;
		int index = 0;
		foreach (KeyValuePair<int, int> pair in sorted)
		{
			array[index] = pair.Value;
			index++;
		}
	}

	public double Noise(double x, double y, double per, int octaves)
	{
		double value = 0;
        for (int i = 0; i < octaves; i++)
			value += Math.Pow(0.5, i) * Noise(x * Math.Pow(2, i), y * Math.Pow(2, i), per * Math.Pow(2, i));
		return value;
    }

	public UnityEngine.Texture2D GetTexture(int width, int height, double per, int octaves)
	{
		var texture = new UnityEngine.Texture2D(width, height);
		for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)
				texture.SetPixel(x, y, new UnityEngine.Color((float)Noise(x / 32D, y / 32D, 4D, 5), (float)Noise(x / 32D, y / 32D, 4D), (float)Noise(x, y, per, octaves)));
		return texture;
	}
}
