using UnityEngine;
using System.Collections;
using System;

public class CameraController : MonoBehaviour
{

	//Keyboard parameters
	public float zoomSpeed = 3f;
	public float rotationSpeed = 1.5f;
	public float strafeSpeed = 0.6f;

	//mouse parameters
	public float sensitivity = 0.1f;
	public float scrollSpeed = 4.0f;
	
	private float actualSens;



	//Scrolling sets target Position to make it smooth. Is there a better way?
	private Vector3 targetPos;
	private Vector3 targetPosMove;
	
	
	bool isCameraTransitionRunning = false;
	float CameraTransitionsmoothing = 5f;
	Vector3 targetForward = new Vector3 (0, 0, 1);
	float startTime;
	float journeyLength;
	float journeyLengthForward;

	Vector3 offset = new Vector3 (0, 0, -20);

	void Start ()
	{
		targetPosMove = transform.position;
		actualSens = sensitivity;
	}
	
	void Update ()
	{
		//Handles the camera Position
		if (isCameraTransitionRunning && transform.position != targetPos) {
			float distCovered = (Time.time - startTime) * 2.0f; //speed = 1.0f
			float fracJourney = distCovered / journeyLength;
			transform.position = Vector3.Lerp (transform.position, targetPos, fracJourney);

			if ((transform.position - targetPos).magnitude < 0.01 && (transform.forward - targetForward).magnitude < 0.01) {
				isCameraTransitionRunning = false;
			}
		}

		//Handles the Camera Orientation
		if (isCameraTransitionRunning && transform.forward != targetForward) {
			Debug.Log ("inside loop");
			float distCovered = (Time.time - startTime) * 0.035f; //speed = 1.0f
			float fracJourney = distCovered / journeyLengthForward;

			transform.forward = (new Vector3 (
				Mathf.LerpAngle (transform.forward.x, targetForward.x, fracJourney),
				Mathf.LerpAngle (transform.forward.y, targetForward.y, fracJourney),
				Mathf.LerpAngle (transform.forward.z, targetForward.z, fracJourney)
			));

			if ((transform.position - targetPos).magnitude < 0.01 && (transform.forward - targetForward).magnitude < 0.01) {
				isCameraTransitionRunning = false;
			}
		}

		//when clicked on a node, zoom in

		Node targetNode = MouseClickUtil.checkMouseClick (MouseClickUtil.LEFT_BTN, 10000);
		if (targetNode != null) {


			Vector3 CameraToTargetVector = (transform.position - targetNode.transform.position);
			CameraToTargetVector.y = 0; //get Point on the same Y level of the target
			CameraToTargetVector = CameraToTargetVector.normalized;


			targetPos = targetNode.transform.position + (CameraToTargetVector * 25f);
			targetForward = (targetNode.transform.position - targetPos).normalized;


			startTime = Time.time;
			journeyLength = Vector3.Distance(transform.position, targetPos);
			journeyLengthForward = Vector3.Distance (transform.forward,  targetForward);

			isCameraTransitionRunning = true;




//
//			//get Point on the same Z level of the target
////					Vector3 CameraToTargetVector = (new Vector3(
////						transform.position.x,
////						transform.position.y,
////						targetNode.transform.position.z) - targetNode.transform.position ).normalized;
//			targetPos = targetNode.transform.position + offset;
////					targetPos = targetNode.transform.position + CameraToTargetVector * 10f;
//			startTime = Time.time;
//			journeyLength = Vector3.Distance (transform.position, targetPos);
//			journeyLengthForward = Vector3.Distance (transform.forward, targetNode.transform.position - transform.position);
//			isCameraTransitionRunning = true;

		}

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
		targetPosMove = targetPosMove + transform.forward * (scrollSpeed * Input.GetAxis ("Scroll") + zoomSpeed * Input.GetAxis ("Zoom"))
			+ transform.up * strafeSpeed * Input.GetAxis ("Vertical2") * 1.5f
			+ transform.right * strafeSpeed * Input.GetAxis ("Horizontal2") * 1.5f;
		// transform.Translate can't be used with this smooth scrolling
		transform.position = Vector3.MoveTowards(transform.position, targetPosMove, Time.deltaTime*500);

		//Reset position and angle
		if (Input.GetKey (KeyCode.R)) {
			transform.position = new Vector3 (0, 1.0f, -10.0f);
			transform.rotation = new Quaternion (0, 0, 0, 0);
		}


	}



}
