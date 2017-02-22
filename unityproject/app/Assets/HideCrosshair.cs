using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class HideCrosshair : MonoBehaviour
{
	private Image pointer;
	private bool nextState = false;
	public KeyCode key = KeyCode.P;

	// Use this for initialization
	void Start ()
	{
		GameObject go = GameObject.FindGameObjectWithTag ("eyepointer");
		pointer = go.GetComponent<Image> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (key)) {
			pointer.enabled = nextState;
			nextState = !nextState;
		}	
	}
}
