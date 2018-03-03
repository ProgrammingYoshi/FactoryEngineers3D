using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FpsDisplay : MonoBehaviour {
	Text text;
	float min = float.MaxValue, max = float.MinValue;
	
	void Start () {
		text = GetComponent<Text>();
	}
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.R))
		{
			min = float.MaxValue;
			max = float.MinValue;
		}
		float value = (1 / Time.deltaTime);
		min = Mathf.Min(min, value);
		max = Mathf.Max(max, value);
		text.text = value.ToString() + "\n" + min.ToString() + "\n" + max.ToString();
	}
}
