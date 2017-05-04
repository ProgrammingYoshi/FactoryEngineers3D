using System;

public class StaticRandom {
	[ThreadStatic]
	static Random rnd = new Random();

	public static int Next(int max)
	{
		if (rnd == null)
			rnd = new Random();
		return rnd.Next(max);
	}
}
