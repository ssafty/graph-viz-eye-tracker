using UnityEngine;
using System.Collections;

public class CalibrationControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float factor = 0.01f;
		Vector3 outvec= Vector3.zero;
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			outvec.z = -factor;
		}
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			outvec.z = factor;
		}
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			outvec.y = -factor;
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			outvec.y= factor;
		}
		if (Input.GetKeyDown (KeyCode.PageUp)) {
			outvec.x = factor;
		}
		if (Input.GetKeyDown (KeyCode.PageDown)) {
			outvec.x = -factor;
		}
		transform.position +=outvec;
	}
}
