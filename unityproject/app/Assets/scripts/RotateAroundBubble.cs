﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking.Match;

public class RotateAroundBubble : MonoBehaviour
{

	public KeyCode rotateLeftKey;
	public KeyCode rotateRightKey;
	public KeyCode rotateUpKey;
	public KeyCode rotateDownKey;
	public Camera main;

	void Start ()
	{
		main = Camera.main;
	}


	void Update ()
	{
		
		Vector3 direction = Vector3.zero;
		if (Input.GetKey (rotateLeftKey)) {
			direction = Vector3.left; 
		} else if (Input.GetKey (rotateRightKey)) {
			direction = Vector3.right;
		} else if (Input.GetKey (rotateUpKey)) {
			direction = Vector3.up;
		} else if (Input.GetKey (rotateDownKey)) {
			direction = Vector3.down;
		} 

		GameObject l = GameObject.FindGameObjectWithTag ("Bubble");
		if (l != null && direction != Vector3.zero) {

			main.transform.Translate (direction * Time.deltaTime * 20);
			main.transform.LookAt (l.transform);


		}


	}


}