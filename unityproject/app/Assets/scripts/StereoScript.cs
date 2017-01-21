using UnityEngine;
using System.Collections;
using System;

public class StereoScript : MonoBehaviour
{

	public static float X_DISTORTION = 1f;
	public GameObject parent2d;
	public GameObject parent3d;

	private GameObject[] markers2d;
	private GameObject[] markers3d;
	private Camera camLeft;
	private Camera camRight;
	public KeyCode disable = KeyCode.F12;
	public KeyCode enable3d = KeyCode.F11;
	public KeyCode enable2d = KeyCode.F10;


	public float stereoDist;
	public float stereoConv;

	// Use this for initialization
	void Start ()
	{
		camLeft = GameObject.Find ("CamLeft").GetComponent<Camera> ();
		camRight = GameObject.Find ("CamRight").GetComponent<Camera> ();

		GetComponent<createMarker> ().update_markers ("markers", "marker", 1f, 1f);
		GetComponent<createMarker> ().update_markers ("markers_stereo", "marker_stereo", 0.473f, 0.95f);

		markers2d =	GameObject.FindGameObjectsWithTag ("marker");
		markers3d = GameObject.FindGameObjectsWithTag ("marker_stereo");

		InstallCam (false);
		toggle (markers2d, false);
		toggle (markers3d, false);
	}

	void InstallCam (bool stereo)
	{
		if (stereo) {
			camRight.gameObject.SetActive (true);
			camLeft.rect = new Rect (0, 0, 0.5f, 1);
			camLeft.transform.Translate (new Vector3 (-(0.5f * stereoDist), 0, 0));
			camRight.transform.Translate (new Vector3 (0.5f * stereoDist, 0, 0));
	
			camLeft.transform.Rotate (new Vector3 (0, stereoConv, 0));
			camRight.transform.Rotate (new Vector3 (0, stereoConv, 0));
			X_DISTORTION = 0.45f;
		} else {
			camLeft.rect = new Rect (0, 0, 1, 1);
			camRight.gameObject.SetActive (false);
			X_DISTORTION = 1;
		}
	}

	private void toggle (GameObject[] markers, bool state)
	{
		foreach (GameObject marker in markers) {
			marker.SetActive (state);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (enable2d)) {
			toggle (markers2d, true);
			toggle (markers3d, false);
			InstallCam (false);

		} else if (Input.GetKeyDown (enable3d)) {
			toggle (markers2d, false);
			toggle (markers3d, true);
			InstallCam (true);
		} else if (Input.GetKeyDown (disable)) {
			toggle (markers2d, false);
			toggle (markers3d, false);	
		} 

	}
}
