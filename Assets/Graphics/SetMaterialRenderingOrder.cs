using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SetMaterialRenderingOrder : MonoBehaviour
{
	public int renderQueue = 0;
	new Renderer renderer;
	void Start()
	{
		renderer = GetComponent<Renderer>();
	}
	void Update()
	{
		if (Application.isEditor)
			renderer.sharedMaterial.renderQueue = renderQueue;
		else
			renderer.material.renderQueue = renderQueue;
	}
}
