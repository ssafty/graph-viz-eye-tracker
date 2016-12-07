using UnityEngine;
using System.Collections;

public class ResetPosition : MonoBehaviour {

	public KeyCode resetKey = KeyCode.Tab;
	public Vector3 resetPos = new Vector3(0f,-5f,-40f);
	public float speed = 10f;
	private bool start = false;


	
	// Update is called once per frame
	void Update () {
		goBack ();
		if (Input.GetKeyDown (resetKey)) {
			start = true;
			Debug.Log (start);
		}
	}


	private void goBack() {
		if (start) {
			
			Camera.main.transform.position = Vector3.MoveTowards (	Camera.main.transform.position, resetPos, speed * Time.deltaTime);
			start = 	Camera.main.transform.position != resetPos;
		}
	}
}
