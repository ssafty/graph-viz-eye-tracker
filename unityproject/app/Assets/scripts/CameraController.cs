using UnityEngine;
using System.Collections;
using System;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 3f;
	public float rotationSpeed = 1.5f;
	public float strafeSpeed = 0.6f;

	public float sensitivity = 0.1f;
	public float scrollSpeed = 4.0f;

	private bool mouseActive = false;


	Vector3 targetPos;
	Vector3 targetForward = new Vector3 (0, 0, 1);

	Vector3 offset = new Vector3 (0, 0, -20);

	GameController gameController;


	void Start ()
	{
		gameController = FindObjectOfType (typeof(GameController)) as GameController;
    }

	void Update ()
	{
        //Rotation
        transform.RotateAround (transform.position, new Vector3 (0, 1.0f, 0), rotationSpeed * Input.GetAxis ("Horizontal"));

		if (transform.eulerAngles.x > 270 || transform.eulerAngles.x < 85) {
			transform.Rotate (new Vector3 (-rotationSpeed * Input.GetAxis ("Vertical"), 0.0f, 0.0f));
			transform.Rotate (new Vector3 (0.0f, sensitivity * Convert.ToInt32 (mouseActive) * Input.GetAxis ("Mouse X"), 0.0f));
		}
		if (transform.eulerAngles.x > 275 || transform.eulerAngles.x < 90) {
			transform.Rotate (new Vector3 (-rotationSpeed * Input.GetAxis ("Vertical"), 0.0f, 0.0f));
			transform.Rotate (new Vector3 (-sensitivity * Convert.ToInt32 (mouseActive) * Input.GetAxis ("Mouse Y"), 0.0f, 0.0f));
		}
        // Strafing and Zooming
        transform.Translate(new Vector3(strafeSpeed * Input.GetAxis("Horizontal2"), strafeSpeed * Input.GetAxis("Vertical2"), zoomSpeed * Input.GetAxis("Zoom") + scrollSpeed * Convert.ToInt32(mouseActive) * Input.GetAxis("Scroll")), Camera.main.transform);
      

		//Toggle Mouse
		if (Input.GetKeyUp (KeyCode.M)) {
			mouseActive = !mouseActive;
		}
	}
}
