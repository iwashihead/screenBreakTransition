using UnityEngine;
using System.Collections;

/// <summary>
/// こいつ・・・動くぞ！
/// </summary>
public class SimpleMove : MonoBehaviour {
	float initial_X;
	// Use this for initialization
	void Start () {
		initial_X = transform.localPosition.x;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = transform.localPosition;
		transform.localPosition = new Vector3(Mathf.Sin(Time.time * 1f) * 1f + initial_X, pos.y, pos.z);
	}
}
