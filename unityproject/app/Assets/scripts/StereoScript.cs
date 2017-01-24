using UnityEngine;
using System.Collections;

public class StereoScript : MonoBehaviour {

	public static float X_DISTORTION = 0.5f;

	public bool stereoscopic3D;
	public float stereoDist;
	public float stereoConv;

	// Use this for initialization
	void Start () {

		Camera camLeft = GameObject.Find ("CamLeft").GetComponent<Camera>();
		Camera camRight = GameObject.Find ("CamRight").GetComponent<Camera>();

		if(stereoscopic3D)
		{
			camLeft.transform.Translate(new Vector3(- (0.5f * stereoDist), 0, 0));
			camRight.transform.Translate(new Vector3(0.5f * stereoDist, 0, 0));
			camLeft.transform.Rotate(new Vector3(0, stereoConv, 0));
			camRight.transform.Rotate(new Vector3(0, stereoConv, 0));
		}
		else
		{
			camLeft.rect = new Rect(0,0,1,1);
			camRight.gameObject.SetActive(false);
			X_DISTORTION = 1;
		}

		//GetComponent<createMarker>().update_markers();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

   

