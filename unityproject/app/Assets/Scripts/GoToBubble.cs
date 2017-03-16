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
		Debug.Log ("--Zoom--");
		GameObject bubble = GameObject.FindGameObjectWithTag ("Bubble");
		if (Bubble.isFarAway()) {
            GoToBubble.ZoomToBubble(Node.GetNodeWithId(0).transform.position);
        } else {
            Vector3 pos = new Vector3(bubble.transform.position.x, bubble.transform.position.y, bubble.transform.position.z - 10);
            GoToBubble.ZoomToBubble(pos);
        }
	}
		

	static void RotateToBubble (Vector3 center)
	{
		GameObject bubble = GameObject.FindGameObjectWithTag ("Bubble");
		Camera camLeft = Camera.main;
		Vector3 direction = bubble.transform.position - camLeft.transform.position;
		Quaternion toRotation = Quaternion.FromToRotation (camLeft.transform.forward, direction);
		camLeft.transform.localRotation = Quaternion.Lerp (camLeft.transform.rotation, toRotation, 5 * Time.deltaTime);



    }

		public static  bool ZoomToBubble (Vector3 pos)
	{
		Debug.Log ("--ZoomToBubble--");
        Camera camLeft = Camera.main;
        if (camLeft.transform.position != pos) {	


			camLeft.transform.position = Vector3.MoveTowards (camLeft.transform.position, pos, 10 * Time.deltaTime);
		//	camRight.transform.position = Vector3.MoveTowards (camLeft.transform.position, pos, 5 * Time.deltaTime);
			RotateToBubble (pos);
			return false;
		} else {
			return true;
		}
	}

}
