using UnityEngine;
using System.Collections;
using System;

public class Bubble : MonoBehaviour
{
	public udpsocket socket;
	public bool useGaze;
	//Use Gaze info from the socket to calc the bubble?

	public static Vector3 REST_POS = new Vector3 (9999, 9999, 9999);
	public GameObject camParent;
	public GameObject camLeft;
	public GameObject camRight;
	public GameObject bubble;
	public float speed = 10f;
	public float rotationSpeed = 2f;
	public float stop = 10f;
	private bool start = false;
	public float jitterMax = 10.0f;
	public float jitterMin = -10.0f;
	public float dwellTime = 0.2f;
	public float interval = 0.1f;
	public GameObject currentBubbleCenter;
	public GameObject eyepointer;
	private Vector3 lastHit;
	private int dwellCount;
	public bool rayCastAllowed = false;
	public KeyCode pushToTrack = KeyCode.Z;
	public bool pressKeyToTrack = true;

	void Start ()
	{
		lastHit = Vector3.zero;
		dwellCount = 0;
		InvokeRepeating ("doRayCast", 0.0f, interval);
	}


	void doRayCast ()
	{
		bool doIt = rayCastAllowed;
		if (pressKeyToTrack) {
			doIt &= Input.GetKey (pushToTrack);
		}

		if (doIt) {
			Debug.Log ("do raycast");
			Vector2 neu = camParent.GetComponent<udpsocket> ().LastEyeCoordinate;
			neu.x = neu.x + (Screen.width / 2);
			neu.y = neu.y + (Screen.height / 2);
			GameObject go = bestBubble (neu);
			Vector3 newPos = getPosition (go);


			if (go != null) {
				Debug.Log ("newPos " + newPos);
				if (newPos == lastHit) {
					dwellCount++;
					Debug.Log ("increasing dwell counter to " + dwellCount);
				} else {
					dwellCount = 0;
					lastHit = newPos;
					Debug.Log ("resetting dwell counter");
				}
				if (dwellCount >= (dwellTime / interval) && bubble != null) {
					Debug.Log ("drawing bubble to" + newPos);
					currentBubbleCenter = go;
					start = true;
					bubble.transform.position = newPos;
					dwellCount = 0;

				}
			}
		}
	}

	void doRayCast_correctedGaze ()
	{
		if (rayCastAllowed) {
			GameObject eyepointer_corrected = GameObject.FindGameObjectWithTag ("eyepointer_corrected");

			Vector2 neu = eyepointer.GetComponent<RectTransform> ().anchoredPosition;
			neu.x = neu.x + (Screen.width / 2);
			neu.y = neu.y + (Screen.height / 2);
			GameObject go = bestBubble (neu);
			Vector3 newPos = getPosition (go);


			if (go != null) {
				Debug.Log ("newPos " + newPos);
				if (newPos == lastHit) {
					dwellCount++;
					Debug.Log ("increasing dwell counter to " + dwellCount);
				} else {
					dwellCount = 0;
					lastHit = newPos;
					Debug.Log ("resetting dwell counter");
				}
				if (dwellCount >= (dwellTime / interval) && bubble != null) {
					Debug.Log ("drawing bubble to" + newPos);
					currentBubbleCenter = go;
					start = true;
					bubble.transform.position = newPos;
					dwellCount = 0;

				}
			}
		}
	}

	// Update is called once per frame
	void Update ()
	{

		if (start) {
			ZoomToBubble ();
		}
		if (Input.GetMouseButtonDown (0)) {
            
			GameObject go = bestBubble (Input.mousePosition);
			Vector3 newPos = getPosition (go);
			if (go != null && bubble != null) {
				start = true;
				currentBubbleCenter = go;
				bubble.transform.position = newPos;
			}
		} else if (Input.anyKeyDown) {
			start = false;
		}
	}

	void RotateToBubble ()
	{
		Vector3 direction = bubble.transform.position - camLeft.transform.position;
		Quaternion toRotation = Quaternion.FromToRotation (camLeft.transform.forward, direction);
		camLeft.transform.localRotation = Quaternion.Lerp (camLeft.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
		camRight.transform.localRotation = Quaternion.Lerp (camLeft.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
	}

	public bool ZoomToBubble ()
	{
		Vector3 pos = new Vector3 (bubble.transform.position.x, bubble.transform.position.y, bubble.transform.position.z - stop);
		if (camLeft.transform.position != pos && bubble.transform.position != REST_POS) {	


			camLeft.transform.position = Vector3.MoveTowards (camLeft.transform.position, pos, speed * Time.deltaTime);
			camRight.transform.position = Vector3.MoveTowards (camLeft.transform.position, pos, speed * Time.deltaTime);
			RotateToBubble ();
			return false;
		} else {
			return true;
		}
	}

	public void calcBubble (Vector2 pos)
	{
		GameObject go = bestBubble (pos);
		Vector3 newPos = getPosition (go);
		if (go != null) { 
			currentBubbleCenter = go;
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
			if (hit.collider.gameObject.tag == "Node") {
				positionSum = positionSum + hit.collider.gameObject.transform.position;
			}
		}
		return (positionSum / Mathf.Max (1, hits.Length));
	}

	Vector3 getPosition (GameObject go)
	{
		return go != null ? go.transform.position : REST_POS;
	}

	GameObject bestBubble (Vector2 pos)
	{
		if (pos != Vector2.zero) {
			RaycastHit[] hits;
			Ray ray = Camera.main.ScreenPointToRay (pos);

			hits = Physics.RaycastAll (ray);
			Vector3 positionSum = Vector3.zero;
			float bestShotDistance = float.MaxValue;
			GameObject bestShotNode = null;
			for (int i = 0; i < hits.Length; i++) {
				RaycastHit hit = hits [i];
				if (hit.collider.gameObject.tag == "Node") {

					float distance = Vector2.Distance (new Vector2 (hit.collider.gameObject.transform.position.x, hit.collider.gameObject.transform.position.y), new Vector2 (hit.point.x, hit.point.y));

					if (distance < bestShotDistance) {
						bestShotDistance = distance;
						bestShotNode = hit.collider.gameObject;
					}
				}
			}

			return bestShotNode;
		}
		return null;
	}

	public static void moveTo (Vector3 pos)
	{
		GameObject bubble = GameObject.FindGameObjectWithTag ("Bubble");
		bubble.transform.position = pos;
	}

	public static void changeBubbleSize (Vector3 scale)
	{
		GameObject bubble = GameObject.FindGameObjectWithTag ("Bubble");
		bubble.transform.localScale = scale;
	}

	public static void changeBubbleSize (float scale)
	{
		Vector3 vec = new Vector3 (scale * StereoScript.X_DISTORTION, scale, scale);
		changeBubbleSize (vec);
	}
}
