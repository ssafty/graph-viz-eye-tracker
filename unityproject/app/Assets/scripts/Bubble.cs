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

	void RotateToBubble ()
	{
		Vector3 direction = bubble.transform.position - camera.transform.position;
		Quaternion toRotation = Quaternion.FromToRotation (camera.transform.forward, direction);
		camera.transform.localRotation = Quaternion.Lerp (camera.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
	}

	private void ZoomToBubble ()
	{
		if (start) {	

			Vector3 pos = bubble.transform.position - new Vector3 (bubble.transform.position.x, bubble.transform.position.y, bubble.transform.position.z - 20);
			camera.transform.position = Vector3.MoveTowards (camera.transform.position, pos, speed * Time.deltaTime);
			if (camera.transform.position == pos) {
				start = false;
			}
		}
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
