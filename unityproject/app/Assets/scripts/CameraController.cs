using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

	public float zoomSpeed = 3f;
	public float rotationSpeed = 1.5f;
	public float strafeSpeed = 0.6f;

	void Update ()
	{
		//Rotation
		transform.RotateAround (transform.position, new Vector3 (0, 1.0f, 0), rotationSpeed * Input.GetAxis ("Horizontal"));
		//This up/down split is necessary, because the boundaries need to be different to avoid getting stuck because of rounding errors 
		if (Input.GetKey (KeyCode.DownArrow)) {
			if (transform.eulerAngles.x > 270 || transform.eulerAngles.x < 85) {
				transform.Rotate (new Vector3 (-rotationSpeed * Input.GetAxis ("Vertical"), 0.0f, 0.0f));
			}
		}
		if (Input.GetKey (KeyCode.UpArrow)) {
			if (transform.eulerAngles.x > 275 || transform.eulerAngles.x < 90) {
				transform.Rotate (new Vector3 (-rotationSpeed * Input.GetAxis ("Vertical"), 0.0f, 0.0f));
			}
		}
		// Strafing and Zooming
		transform.Translate (new Vector3 (strafeSpeed * Input.GetAxis ("Horizontal2"), strafeSpeed * Input.GetAxis ("Vertical2"), zoomSpeed * Input.GetAxis ("Zoom")));
        
		//Reset position and angle
		if (Input.GetKey (KeyCode.R)) {
			transform.position = new Vector3 (0, 1.0f, -10.0f);
			transform.rotation = new Quaternion (0, 0, 0, 0);
		}
	}



}
