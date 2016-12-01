using UnityEngine;
using System.Collections;
using System;

public class Bubble : MonoBehaviour
{

	private Camera camera;
	public GameObject bubble;
	public float speed = 10f;
	public float rotationSpeed = 2f;
	public float stop = 10f;
	private bool start = false;

	void Start ()
	{
		camera = Camera.main;
	}
	
	// Update is called once per frame
	void Update ()
	{
		ZoomToBubble ();
		if (Input.GetMouseButtonDown (0)) {
			
			Vector3 newPos = bestBubble ();
			if (newPos != Vector3.zero && bubble != null) {
				start = true;
				bubble.transform.position = newPos;



			}
		}

	}

	private void ZoomToBubble ()
	{
		
		Vector3 direction = bubble.transform.position - camera.transform.position;
		Quaternion toRotation = Quaternion.FromToRotation (camera.transform.forward, direction);
		camera.transform.localRotation = Quaternion.Lerp (camera.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);



//		float step = speed * Time.deltaTime;
//		Vector3 pos = bubble.transform.position - (Vector3.one * 10);
//		camera.transform.position = Vector3.MoveTowards (camera.transform.position, pos, step);
//		if (camera.transform.position == pos) {
//			camera.transform.LookAt (bubble.transform.position);
//		}
	
//		//Handles the camera Position
//		if (isCameraTransitionRunning && transform.position != targetPos) {
//			float distCovered = (Time.time - startTime) * 2.0f; //speed = 1.0f
//			float fracJourney = distCovered / journeyLength;
//			transform.position = Vector3.Lerp (transform.position, targetPos, fracJourney);
//
//			if ((transform.position - targetPos).magnitude < 0.01 && (transform.forward - targetForward).magnitude < 0.01) {
//				isCameraTransitionRunning = false;
//			}
//		}
//
//		//Handles the Camera Orientation
//		if (isCameraTransitionRunning && transform.forward != targetForward) {
//
//			float distCovered = (Time.time - startTime) * 0.035f; //speed = 1.0f
//			float fracJourney = distCovered / journeyLengthForward;
//
//			transform.forward = (new Vector3 (
//				Mathf.LerpAngle (transform.forward.x, targetForward.x, fracJourney),
//				Mathf.LerpAngle (transform.forward.y, targetForward.y, fracJourney),
//				Mathf.LerpAngle (transform.forward.z, targetForward.z, fracJourney)
//			));
//
//			if ((transform.position - targetPos).magnitude < 0.01 && (transform.forward - targetForward).magnitude < 0.01) {
//				isCameraTransitionRunning = false;
//			}
//		}
//
//		//when clicked on a node, zoom in
//		GameObject targetNode = MouseClickUtil.checkMouseClick (MouseClickUtil.LEFT_BTN, 10000);
//
//		if (targetNode != null) {
//			Vector3 CameraToTargetVector = (transform.position - targetNode.transform.position);
//			CameraToTargetVector.y = 0; //get Point on the same Y level of the target
//			CameraToTargetVector = CameraToTargetVector.normalized;
//
//			targetPos = targetNode.transform.position + (CameraToTargetVector * 25f);
//			targetForward = (targetNode.transform.position - targetPos).normalized;
//
//			startTime = Time.time;
//			journeyLength = Vector3.Distance (transform.position, targetPos);
//			journeyLengthForward = Vector3.Distance (transform.forward, targetForward);
//
//			isCameraTransitionRunning = true;
//
//		}
	}

	public void calcBubble (Vector2 pos)
	{

		Vector3 newPos = bestBubble ();
		if (newPos != Vector3.zero) {
			bubble.transform.position = newPos;
		}
	}

	Vector3 simpleBubble ()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (ray, out hit)) {
			return hit.point;
		}
		return Vector3.zero;
	}

	Vector3 meanBubble ()
	{
		RaycastHit[] hits;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		hits = Physics.RaycastAll (ray);
		Vector3 positionSum = Vector3.zero;
		for (int i = 0; i < hits.Length; i++) {
			RaycastHit hit = hits [i];
			Debug.Log (hit);
			if (hit.collider.gameObject.tag == "Node") {
				positionSum = positionSum + hit.collider.gameObject.transform.position;
			}
		}
		return (positionSum / Mathf.Max (1, hits.Length));
	}

	Vector3 bestBubble ()
	{
		RaycastHit[] hits;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		hits = Physics.RaycastAll (ray);
		Vector3 positionSum = Vector3.zero;
		float bestShotDistance = float.MaxValue;
		GameObject bestShotNode = null;
		for (int i = 0; i < hits.Length; i++) {
			RaycastHit hit = hits [i];
			Debug.Log (hit);

			if (hit.collider.gameObject.tag == "Node") {
				float distance = Vector2.Distance (new Vector2 (hit.collider.gameObject.transform.position.x, hit.collider.gameObject.transform.position.y), new Vector2 (hit.point.x, hit.point.y));

				if (distance < bestShotDistance) {
					bestShotDistance = distance;
					bestShotNode = hit.collider.gameObject;
				}
			}
		}
		return bestShotNode == null ? Vector3.zero : bestShotNode.transform.position;
	}
}
