using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeCalc : MonoBehaviour {

	public GameObject GazeCollider;
	public GameObject GazeTracker;
	public GameObject GazeMarker;

	public Vector2 DisplayDim;	//Display dimension in meters
	public Vector3 HeadPosition;	//Position of the head from the center of the screen in meters
	public Vector3 EyeOffset;
	public float MarginMultiplier;

	private GameObject LeftRay;
	private GameObject RightRay;


	//public float Margin;
	private float Margin;
	public float MaxDistance;


	// Use this for initialization
	void Start () {

		Margin = EyeOffset.magnitude * MarginMultiplier;
		GazeCollider.transform.localScale = new Vector3(Margin, MaxDistance, Margin);

		LeftRay = GazeCollider;
		LeftRay.GetComponent<MeshRenderer>().enabled = false;
		RightRay = GameObject.Instantiate(LeftRay);
		LeftRay.name = "Left Gaze Collider";
		RightRay.name = "Right Gaze Collider";

		LeftRay.transform.position = transform.position + HeadPosition - EyeOffset;
		RightRay.transform.position = transform.position + HeadPosition + EyeOffset;

		LeftRay.transform.up = (Vector3.zero - LeftRay.transform.position);
		RightRay.transform.up = (Vector3.zero - RightRay.transform.position);

		GazeMarker.GetComponent<Renderer>().material.color = Color.magenta;
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 l_pos = GazeTracker.GetComponent<PupilGazeTracker>().LeftEyePos;	//These two must be from -1 to 1 in both dimenstions. (1, 1) being upper right cornor
		Vector2 r_pos = GazeTracker.GetComponent<PupilGazeTracker>().RightEyePos;
		//print(l_pos);

		//DEBUG:	REMOVE FOR EYETRACKER
		l_pos = new Vector2(-0.002f, 0);
		r_pos = new Vector2(0.002f, 0);

		Vector3 gaze3Dleft = new Vector3(l_pos.x * DisplayDim.x * 0.5f, l_pos.y * DisplayDim.y * 0.5f, 0);
		Vector3 gaze3Dright = new Vector3(r_pos.x * DisplayDim.x * 0.5f, r_pos.y * DisplayDim.y * 0.5f, 0);

		LeftRay.transform.up = (gaze3Dleft - LeftRay.transform.position);
		RightRay.transform.up = (gaze3Dright - RightRay.transform.position);

		if(LeftRay.GetComponent<CollisionGetter>().collision != null)
		{
			GazeMarker.transform.position = LeftRay.GetComponent<CollisionGetter>().collision.contacts[0].point;
		}
	}


}
