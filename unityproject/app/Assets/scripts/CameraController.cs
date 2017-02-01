using UnityEngine;
using System.Collections;
using System;

public class CameraController : MonoBehaviour
{

    public Camera camLeft, camRight;

    public float zoomSpeed = 3f;
	public float rotationSpeed = 1.5f;
	public float strafeSpeed = 0.6f;

	public float sensitivity = 0.1f;
	public float scrollSpeed = 4.0f;

	private bool mouseActive = false;

	bool isCameraTransitionRunning = false;
	float CameraTransitionsmoothing = 5f;
	Vector3 targetPos;
	Vector3 targetForward = new Vector3 (0, 0, 1);
	//private Camera camLeft, camRight;

	Vector3 offset = new Vector3 (0, 0, -20);

	GameController gameController;


	void Start ()
	{
		gameController = FindObjectOfType (typeof(GameController)) as GameController;
		//camLeft = GameObject.Find ("CamLeft").GetComponent<Camera> ();
		//camRight = GameObject.Find ("CamRight").GetComponent<Camera> ();
    }

	void Update ()
	{

        //Rotation
        transform.RotateAround (transform.position, new Vector3 (0, 1.0f, 0), rotationSpeed * Input.GetAxis ("Horizontal"));

		if (transform.eulerAngles.x > 270 || transform.eulerAngles.x < 85) {
			camLeft.transform.Rotate (new Vector3 (-rotationSpeed * Input.GetAxis ("Vertical"), 0.0f, 0.0f));
			camRight.transform.Rotate (new Vector3 (0.0f, sensitivity * Convert.ToInt32 (mouseActive) * Input.GetAxis ("Mouse X"), 0.0f));
		}
		if (transform.eulerAngles.x > 275 || transform.eulerAngles.x < 90) {
			camLeft.transform.Rotate (new Vector3 (-rotationSpeed * Input.GetAxis ("Vertical"), 0.0f, 0.0f));
			camRight.transform.Rotate (new Vector3 (-sensitivity * Convert.ToInt32 (mouseActive) * Input.GetAxis ("Mouse Y"), 0.0f, 0.0f));
		}
		// Strafing and Zooming
		camLeft.transform.Translate (new Vector3 (strafeSpeed * Input.GetAxis ("Horizontal2"), strafeSpeed * Input.GetAxis ("Vertical2"), zoomSpeed * Input.GetAxis ("Zoom") + scrollSpeed * Convert.ToInt32 (mouseActive) * Input.GetAxis ("Scroll")));
		camRight.transform.Translate (new Vector3 (strafeSpeed * Input.GetAxis ("Horizontal2"), strafeSpeed * Input.GetAxis ("Vertical2"), zoomSpeed * Input.GetAxis ("Zoom") + scrollSpeed * Convert.ToInt32 (mouseActive) * Input.GetAxis ("Scroll")));



		//Toggle Mouse
		if (Input.GetKeyUp (KeyCode.M)) {
			mouseActive = !mouseActive;
		}
	}
}
