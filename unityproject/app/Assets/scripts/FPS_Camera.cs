using UnityEngine;
using System.Collections;

// WARNING: Enabling this breaks the c/v yoomin on keyboard
public class FPS_Camera : MonoBehaviour {

	public float sensitivity;
	public float scrollSpeed;

	private Vector3 targetPos;

	// Use this for initialization
	void Start () {
		targetPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround (transform.position, new Vector3 (0, 1.0f, 0), sensitivity * Input.GetAxis ("Mouse X"));

		if (transform.eulerAngles.x > 270 || transform.eulerAngles.x < 85) {
			transform.Rotate (new Vector3 (-sensitivity * Input.GetAxis ("Mouse Y"), 0.0f, 0.0f));
		}

		if (transform.eulerAngles.x > 275 || transform.eulerAngles.x < 90) {
			transform.Rotate (new Vector3 (-sensitivity * Input.GetAxis ("Mouse Y"), 0.0f, 0.0f));
		}

		//transform.Translate (new Vector3 (0, 0, scrollSpeed * Input.GetAxis ("Scroll")));

		transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime*500);
		targetPos = targetPos + transform.forward * scrollSpeed * Input.GetAxis ("Scroll");
	}

}
