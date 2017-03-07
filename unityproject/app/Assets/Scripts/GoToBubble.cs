using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToBubble : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		if (Input.GetKey(KeyCode.K)) {
			GoToBubble.Zoom ();
					

		}  
	}

	public static void Zoom() {
		GameObject bubble = GameObject.FindGameObjectWithTag ("Bubble");
		if (Bubble.REST_POS != bubble.transform.position) {
			GoToBubble.ZoomToBubble ();
		}
	}
		

	static void RotateToBubble ()
	{
		GameObject bubble = GameObject.FindGameObjectWithTag ("Bubble");
		Camera camLeft = Camera.main;
		Vector3 direction = bubble.transform.position - camLeft.transform.position;
		Quaternion toRotation = Quaternion.FromToRotation (camLeft.transform.forward, direction);
		camLeft.transform.localRotation = Quaternion.Lerp (camLeft.transform.rotation, toRotation, 5 * Time.deltaTime);
	//	camRight.transform.localRotation = Quaternion.Lerp (camLeft.transform.rotation, toRotation, 5 * Time.deltaTime);
	}

		public static  bool ZoomToBubble ()
	{
		GameObject bubble = GameObject.FindGameObjectWithTag ("Bubble");
		Camera camLeft = Camera.main;
		Vector3 pos = new Vector3 (bubble.transform.position.x, bubble.transform.position.y, bubble.transform.position.z - 10);
		if (camLeft.transform.position != pos && bubble.transform.position != Bubble.REST_POS) {	


			camLeft.transform.position = Vector3.MoveTowards (camLeft.transform.position, pos, 10 * Time.deltaTime);
		//	camRight.transform.position = Vector3.MoveTowards (camLeft.transform.position, pos, 5 * Time.deltaTime);
			RotateToBubble ();
			return false;
		} else {
			return true;
		}
	}

}
