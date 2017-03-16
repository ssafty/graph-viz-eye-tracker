using UnityEngine;
using System.Collections;
using System;

public class Bubble : MonoBehaviour
{
	public udpsocket socket;
	public bool useGaze;
    //Use Gaze info from the socket to calc the bubble?
    private  bool bubbleActive = true;
    public GameObject bubbleGo;
    public static Vector3 REST_POS = new Vector3 (9999, 9999, 9999);
	public GameObject camParent;
	public GameObject camLeft;
	public GameObject camRight;
	public GameObject bubble;
	public float speed = 10f;
	public float rotationSpeed = 2f;
	public float stop = 10f;
	private bool start = false;

	public float dwellTime = 0.2f;
	public float interval = 0.1f;
	public GameObject currentBubbleCenter;
	public GameObject eyepointer;
	private Vector3 lastHit;
	private int dwellCount;
	public bool rayCastAllowed = false;
    public bool useCorrectedGaze = false;
	//public KeyCode pushToTrack = KeyCode.Z;
	//public bool pressKeyToTrack = true;

	void Start ()
	{
		lastHit = Vector3.zero;
		dwellCount = 0;
        bubbleGo = GameObject.FindGameObjectWithTag("Bubble");

        InvokeRepeating ("doRayCast", 0.0f, interval);
	}



	void doRayCast ()
	{
		bool doIt = rayCastAllowed;
		//if (pressKeyToTrack) {
		//	doIt &= Input.GetKey (pushToTrack);
		//}
		if (doIt) {
            Debug.Log("do raycast");
            Vector2 neu = camParent.GetComponent<udpsocket>().LastEyeCoordinate;
            if (useCorrectedGaze)
            {
                neu = GameObject.FindGameObjectWithTag("eyepointer_corrected").GetComponent<RectTransform>().anchoredPosition;
            }
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

	
		if (Input.GetMouseButtonDown (0)) {
            
			GameObject go = simpleBubble (Input.mousePosition);
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



	public void calcBubble (Vector2 pos)
	{
		GameObject go = bestBubble (pos);
		Vector3 newPos = getPosition (go);
		if (go != null) { 
			currentBubbleCenter = go;
			bubble.transform.position = newPos;
		}
	}

	GameObject simpleBubble (Vector2 pos)
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay (pos);
		if (Physics.Raycast (ray, out hit)) {
			return hit.collider.gameObject;
		}
		return null;
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
        if (bubble != null)
        {
            bubble.transform.position = pos;
        }
	}

	public static void changeBubbleSize (Vector3 scale)
	{
      
		GameObject bubble = GameObject.FindGameObjectWithTag ("Bubble");
        if (bubble != null) {
            bubble.transform.localScale = scale;
        }
	}

	public static void changeBubbleSize (float scale)
	{
		Vector3 vec = new Vector3 (scale * StereoScript.X_DISTORTION, scale, scale);
		changeBubbleSize (vec);
	}

    public static bool isFarAway()
    {

        GameObject bubble = GameObject.FindGameObjectWithTag("Bubble");
        Vector3 pos = bubble.transform.position;
        if (Math.Abs(pos.x) >= 9999 || Math.Abs(pos.y) >= 9999 || Math.Abs(pos.z) >= 9999)
        {
            return true;
        }
        return false;
    }



        
}
