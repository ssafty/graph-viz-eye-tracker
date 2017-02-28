using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class HideCrosshair : MonoBehaviour
{
	private Image pointer;
	private Image pointer_green;
	private bool nextState = false;
	public KeyCode key = KeyCode.P;

	// Use this for initialization
	void Start ()
	{
		GameObject go_red = GameObject.FindGameObjectWithTag ("eyepointer");
		GameObject go_green = GameObject.FindGameObjectWithTag ("eyepointer_corrected");

		pointer = go_red.GetComponent<Image> ();
		pointer_green = go_green.GetComponent<Image> ();

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (key)) {
			pointer.enabled = nextState;
			pointer_green.enabled = nextState;
			nextState = !nextState;
		}	
	}
}
