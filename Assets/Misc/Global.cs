using UnityEngine;

public static class Global {
	public static World world;
	public static Statistics statistics;
	public static bool mouseLookEnabled = true;
	public static System.Random rnd = new System.Random();

	public static Vector3 Floor(Vector3 vector)
	{
		return new Vector3(Mathf.Floor(vector.x), Mathf.Floor(vector.y), Mathf.Floor(vector.z));
	}
	
	public static void Swap(object a, object b)
	{
		object tmp = a;
		a = b;
		b = tmp;
	}
}
