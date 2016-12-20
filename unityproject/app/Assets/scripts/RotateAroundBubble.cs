using UnityEngine;
using System.Collections;
using UnityEngine.Networking.Match;

public class RotateAroundBubble : MonoBehaviour
{

	public KeyCode rotateLeftKey = KeyCode.Q;
	public KeyCode rotateRightKey = KeyCode.W;
	public KeyCode rotateUpKey = KeyCode.X;
	public KeyCode rotateDownKey = KeyCode.Y;

	public GameObject main;

	void Start ()
	{
		//main = Camera.main;
	}


	void Update ()
	{
		
		Vector3 direction = Vector3.zero;
		if (Input.GetKey (rotateLeftKey)) {
			direction = Vector3.up; 
		} else if (Input.GetKey (rotateRightKey)) {
			direction = Vector3.down;
		} else if (Input.GetKey (rotateUpKey)) {
			direction = Vector3.left;
		} else if (Input.GetKey (rotateDownKey)) {
			direction = Vector3.right;
		} 

		GameObject l = GameObject.FindGameObjectWithTag ("Bubble");
		Vector3 pos = l.transform.position == Bubble.REST_POS ? Vector3.zero : l.transform.position ;
		if (l != null && direction != Vector3.zero ) {

			main.transform.RotateAround (pos, direction, 1);



		}


	}


}
