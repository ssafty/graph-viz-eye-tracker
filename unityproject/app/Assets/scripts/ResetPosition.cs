﻿using UnityEngine;
using System.Collections;

public class ResetPosition : MonoBehaviour
{

	public KeyCode resetKey = KeyCode.Tab;
	public Vector3 resetPos = new Vector3 (0f, -5f, -40f);
	public float speed = 10f;
	private bool start = false;


	
	// Update is called once per frame
	void Update ()
	{
		goBack ();
		if (Input.GetKeyDown (resetKey)) {
			start = true;
			Debug.Log (start);

			Camera.main.transform.localEulerAngles = Vector3.zero;
			Camera.main.transform.position = new Vector3 (0, 0, -40);
			GameObject rightCam = GameObject.FindGameObjectWithTag ("RightCam");
			if (rightCam) {
				rightCam.transform.position = new Vector3 (0, 0, -40);
				rightCam.transform.localEulerAngles = Vector3.zero;
			}
		}
	}


	private void goBack ()
	{
		if (start) {
			GameObject go = GameObject.FindGameObjectWithTag ("metaCamera");
			go.transform.Rotate (Vector3.zero);

			//	go.transform.position = Vector3.MoveTowards (Camera.main.transform.position, resetPos, speed * Time.deltaTime);
			start = go.transform.position != resetPos;
		}
	}
}
