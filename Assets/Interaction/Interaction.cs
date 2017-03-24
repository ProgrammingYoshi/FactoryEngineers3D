using UnityEngine;
using System.Collections;

public class Interaction : MonoBehaviour
{
	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update()
	{
		RaycastHit hitInfo;
		bool result = Physics.Raycast(transform.position, transform.forward, out hitInfo);
		Vector3 point = hitInfo.point + (hitInfo.point - transform.position) * 0.00390625F; // 1/256
		Vector3 pointBefore = hitInfo.point - (hitInfo.point - transform.position) * 0.00390625F; // TODO: Make this viewer relative to block, so no diagonal block selects can happen
        if (result)
		{
			Global.world.Select(point, Color.yellow);
			if (Global.world.BlockAtSelection != null && !Global.world.BlockAtSelection.IsSolid)
				Global.world.Select(point, Color.yellow);
		}
		else
			Global.world.Deselect();
		if (Input.GetMouseButtonDown(0))
		{
			Global.world.BlockAtSelection = null;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			Global.mouseLookEnabled = true;
		}
		if (Input.GetMouseButtonDown(1))
		{
			Global.world.SetBlock((Vector3i)pointBefore, new DynamicBlock((Vector3i)pointBefore));
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			Global.mouseLookEnabled = false;
		}
	}
}
