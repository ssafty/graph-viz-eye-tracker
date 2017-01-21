using UnityEngine;
using System.Collections;

public class ToggleMarkers : MonoBehaviour
{

	private GameObject[] markers;
	private bool state = false;
	private bool fetch = true;
	public KeyCode toggleMarkerKey = KeyCode.F12;

	void Start ()
	{
		
	}

	
	// Update is called once per frame
	void Update ()
	{
		if (fetch) {
			markers = GameObject.FindGameObjectsWithTag ("marker");
			fetch = false;
		}


		if (Input.GetKeyDown (toggleMarkerKey)) {
			
			foreach (GameObject m in markers) {
				m.SetActive (state);
				Debug.Log ("set state to " + state);
			}
			state = !state;
		}
	
	}
}
