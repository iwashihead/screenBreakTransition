using UnityEngine;
using System.Collections;

public class ScreenBreakController : MonoBehaviour {

	public ScreenBreak screenBreak;
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown)
		{
			screenBreak.Play();
		}
	}
}
