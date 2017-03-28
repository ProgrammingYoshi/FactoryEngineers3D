using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct Vector3i
{
	public static readonly Vector3i zero = new Vector3i(0, 0, 0), right = new Vector3i(1, 0, 0), left = new Vector3i(-1, 0, 0), up = new Vector3i(0, 1, 0), down = new Vector3i(0, -1, 0), forward = new Vector3i(0, 0, 1), back = new Vector3i(0, 0, -1);
    public int x, y, z;
	
	public Vector3i(int x, int y, int z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public static Vector3i operator +(Vector3i a, Vector3i b)
	{
		return new Vector3i(a.x + b.x, a.y + b.y, a.z + b.z);
	}

	public static Vector3i operator -(Vector3i a, Vector3i b)
	{
		return new Vector3i(a.x - b.x, a.y - b.y, a.z - b.z);
	}

	public static Vector3i operator *(Vector3i a, Vector3i b)
	{
		return new Vector3i(a.x * b.x, a.y * b.y, a.z * b.z);
	}

	public static Vector3i operator *(Vector3i a, int b)
	{
		return new Vector3i(a.x * b, a.y * b, a.z * b);
	}

	public static Vector3i operator *(Vector3i a, float b)
	{
		return new Vector3i((int)(a.x * b), (int)(a.y * b), (int)(a.z * b));
	}

	public static Vector3i operator /(Vector3i a, Vector3i b)
	{
		return new Vector3i(a.x / b.x, a.y / b.y, a.z / b.z);
	}

	public static Vector3i operator &(Vector3i a, int b)
	{
		return new Vector3i(a.x & b, a.y & b, a.z & b);
	}

	public static Vector3i operator |(Vector3i a, int b)
	{
		return new Vector3i(a.x | b, a.y | b, a.z | b);
	}

	public static Vector3i operator >>(Vector3i a, int b)
	{
		return new Vector3i(a.x >> b, a.y >> b, a.z >> b);
	}

	public static Vector3i operator <<(Vector3i a, int b)
	{
		return new Vector3i(a.x << b, a.y << b, a.z << b);
	}

	public static bool operator ==(Vector3i a, Vector3i b)
	{
		return a.x == b.x && a.y == b.y && a.z == b.z;
	}

	public static bool operator !=(Vector3i a, Vector3i b)
	{
		return !(a.x == b.x && a.y == b.y && a.z == b.z);
	}

	public static implicit operator Vector3(Vector3i value)
	{
		return new Vector3(value.x, value.y, value.z);
	}

	public static explicit operator Vector3i(Vector3 value)
	{
		return new Vector3i((int)value.x, (int)value.y, (int)value.z);
	}

	public override string ToString()
	{
		return string.Format("{0} {1} {2}", x, y, z);
	}
}
