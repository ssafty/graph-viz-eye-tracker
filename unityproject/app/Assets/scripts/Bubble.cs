using UnityEngine;
using System.Collections;
using System;

public class Bubble : MonoBehaviour
{
	public static Vector3 REST_POS = new Vector3(9999,9999,9999);
	private Camera camera;
	public GameObject bubble;
	public float speed = 10f;
	public float rotationSpeed = 2f;
	public float stop = 10f;
	private bool start = false;
    	public float jitterMax = 10.0f;
    	public float jitterMin = -10.0f;
    public float dwellTime = 0.1f;
    public float interval = 0.1f;
    public GameObject eyepointer;
    private Vector3 lastHit;
    private int dwellCount;
     
    void Start ()
	{
	
        lastHit = Vector3.zero;
        camera = Camera.main;
        dwellCount = 0;
        InvokeRepeating("doRayCast",0.0f, interval);
	}
	void doRayCast()
    {
        Vector2 neu = Camera.main.GetComponent<udpsocket>().LastEyeCoordinate;
        neu.x = neu.x + (Screen.width / 2);
        neu.y = neu.y + (Screen.height / 2);
        Vector3 newPos = bestBubble(neu);
        if (newPos != Vector3.zero)
        {

            if (newPos == lastHit)
            {
                dwellCount++;
                lastHit = newPos;
                //Debug.Log("increasing dwell counter to " + dwellCount);
            }
            else
            {
                dwellCount = 0;
                lastHit = newPos;
                Debug.Log("resetting dwell counter");
            }
            if (dwellCount >= (dwellTime / interval) && newPos != Vector3.zero && bubble != null)
            {
                Debug.Log("drawing bubble to" + newPos);
                start = true;
                bubble.transform.position = newPos;
                dwellCount = 0;
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
			
			Vector3 newPos = bestBubble (Input.mousePosition);
			if (newPos != Vector3.zero && bubble != null) {
				start = true;
				bubble.transform.position = newPos;
			}
		} else if (Input.anyKeyDown) {
			start = false;
		}

	}

	void RotateToBubble ()
	{
		Vector3 direction = bubble.transform.position - camera.transform.position;
		Quaternion toRotation = Quaternion.FromToRotation (camera.transform.forward, direction);
		camera.transform.localRotation = Quaternion.Lerp (camera.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
	}

	public bool ZoomToBubble ()
	{
		Vector3 pos = new Vector3 (bubble.transform.position.x, bubble.transform.position.y, bubble.transform.position.z - stop);
		if (camera.transform.position != pos && bubble.transform.position != REST_POS) {	


			camera.transform.position = Vector3.MoveTowards (camera.transform.position, pos, speed * Time.deltaTime);
			RotateToBubble ();
			return false;
		} else {
			return true;
		}
	}

	public void calcBubble (Vector2 pos)
	{

		Vector3 newPos = bestBubble (pos);
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
			if (hit.collider.gameObject.tag == "Node") {
				positionSum = positionSum + hit.collider.gameObject.transform.position;
			}
		}
		return (positionSum / Mathf.Max (1, hits.Length));
	}

	Vector3 bestBubble (Vector2 pos)
	{
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
		return bestShotNode == null ? Vector3.zero : bestShotNode.transform.position;
	}
}
