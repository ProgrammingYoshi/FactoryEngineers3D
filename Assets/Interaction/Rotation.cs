using UnityEngine;

public class Rotation : MonoBehaviour
{
	public float speed = 0.2F;
	Vector3 mousePosition, lastMousePosition;

	void Start()
	{
		lastMousePosition = Input.mousePosition;
	}

	void Update()
	{
		mousePosition += Input.mousePosition - lastMousePosition;
		transform.rotation = Quaternion.Euler(new Vector3(0 - mousePosition.y, mousePosition.x, 0) * speed);
		lastMousePosition = Input.mousePosition;
	}
}