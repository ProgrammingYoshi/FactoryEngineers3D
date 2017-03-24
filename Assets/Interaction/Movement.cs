using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
	public float speedMultiplier = 0.25F;
	public float forceMultiplier = 16F;
	Rigidbody rigidbody;

	void Start()
	{
		rigidbody = GetComponent<Rigidbody>();
	}

	void Update()
	{
		Vector3 desiredVelocity = Vector3.zero;
		Vector3 eulerAngles = transform.rotation.eulerAngles;
		Quaternion rotation = Quaternion.Euler(0, eulerAngles.y, 0);
		if (Input.GetKey(KeyCode.W))
			desiredVelocity += rotation * Vector3.forward;
		if (Input.GetKey(KeyCode.S))
			desiredVelocity += rotation * Vector3.back;
		if (Input.GetKey(KeyCode.D))
			desiredVelocity += rotation * Vector3.right;
		if (Input.GetKey(KeyCode.A))
			desiredVelocity += rotation * Vector3.left;
		desiredVelocity = RemoveY(desiredVelocity);
		if (Input.GetKey(KeyCode.Space))
			desiredVelocity += Vector3.up;
		if (Input.GetKey(KeyCode.LeftShift))
			desiredVelocity += Vector3.down;
		//Vector3 delta = (desiredVelocity - rigidbody.velocity).normalized * speedMultiplier;
		rigidbody.AddForce(desiredVelocity * speedMultiplier - rigidbody.velocity, ForceMode.VelocityChange);
	}

	Vector3 RemoveY(Vector3 vector)
	{
		return new Vector3(vector.x, 0, vector.z);
	}
}
