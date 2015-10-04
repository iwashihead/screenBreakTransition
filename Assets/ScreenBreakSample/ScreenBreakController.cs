using UnityEngine;
using System.Collections;

public class ScreenBreakController : MonoBehaviour {

	public ScreenBreak screenBreak;

	// Use this for initialization
	IEnumerator Start () {
		yield return new WaitForSeconds(2f);

		screenBreak.Play();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			screenBreak.Play();
		}
	}
}
