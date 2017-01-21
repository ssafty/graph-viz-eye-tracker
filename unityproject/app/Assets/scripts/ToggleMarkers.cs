using UnityEngine;
using System.Collections;

public class ToggleMarkers : MonoBehaviour
{

	private GameObject[] markers;
	private bool state = false;
	public KeyCode toggleMarkerKey = KeyCode.F12;

	void Start ()
	{
		markers = GameObject.FindGameObjectsWithTag ("marker");
	}

	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (toggleMarkerKey)) {
			
			foreach (GameObject m in markers) {
				m.SetActive (state);
				Debug.Log ("set state to " + state);
			}
			state = !state;
		}
	
	}
}
