using UnityEngine;
using System.Collections;

public class StereoScript : MonoBehaviour {

	public float stereoDist;
	public float stereoConv;

	private Camera camLeft;
	private Camera camRight;
	// Use this for initialization
	void Start () {
		camLeft = GameObject.Find ("CamLeft").GetComponent<Camera>();
		camRight = GameObject.Find ("CamRight").GetComponent<Camera>();

		camLeft.transform.Translate(new Vector3(- (0.5f * stereoDist), 0, 0));
		camRight.transform.Translate(new Vector3(0.5f * stereoDist, 0, 0));
		camLeft.transform.Rotate(new Vector3(0, stereoConv, 0));
		camRight.transform.Rotate(new Vector3(0, stereoConv, 0));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
