using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

	//Keyboard parameters
	public float zoomSpeed = 3f;
	public float rotationSpeed = 1.5f;
	public float strafeSpeed = 0.6f;

	//mouse parameters
	public float sensitivity;
	public float scrollSpeed;



	//Scrolling sets target Position to make it smooth. Is there a better way?
	private Vector3 targetPos;

	void Start ()
	{
		targetPos = transform.position;
	}	

	void Update ()
	{
		//Rotation
		transform.RotateAround (transform.position, new Vector3 (0, 1.0f, 0), rotationSpeed * Input.GetAxis ("Horizontal"));
		// X movement
		transform.RotateAround (transform.position, new Vector3 (0, 1.0f, 0), sensitivity * Input.GetAxis ("Mouse X"));

		//This up/down split is necessary, because the boundaries need to be different to avoid getting stuck because of rounding errors 
		if (transform.eulerAngles.x > 270 || transform.eulerAngles.x < 85) {
				transform.Rotate (new Vector3 (-rotationSpeed * Input.GetAxis ("Vertical"), 0.0f, 0.0f));
				transform.Rotate (new Vector3 (-sensitivity * Input.GetAxis ("Mouse Y"), 0.0f, 0.0f));
		}
		if (transform.eulerAngles.x > 275 || transform.eulerAngles.x < 90) {
				transform.Rotate (new Vector3 (-rotationSpeed * Input.GetAxis ("Vertical"), 0.0f, 0.0f));
				transform.Rotate (new Vector3 (-sensitivity * Input.GetAxis ("Mouse Y"), 0.0f, 0.0f));
		}

		// Strafing and Zooming
		//transform.Translate (new Vector3 (strafeSpeed * Input.GetAxis ("Horizontal2"), strafeSpeed * Input.GetAxis ("Vertical2"), zoomSpeed * Input.GetAxis ("Zoom")));
		targetPos = targetPos + transform.forward * (scrollSpeed * Input.GetAxis ("Scroll") + zoomSpeed * Input.GetAxis ("Zoom"))
			+ transform.up * strafeSpeed * Input.GetAxis ("Vertical2") * 1.5f
			+ transform.right * strafeSpeed * Input.GetAxis ("Horizontal2") * 1.5f;
		// transform.Translate can't be used with this smooth scrolling
		transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime*500);

		//Reset position and angle
		if (Input.GetKey (KeyCode.R)) {
			transform.position = new Vector3 (0, 1.0f, -10.0f);
			transform.rotation = new Quaternion (0, 0, 0, 0);
		}
	}



}
