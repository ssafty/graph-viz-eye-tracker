using UnityEngine;
using System.Collections;

public class CalibrationScript : MonoBehaviour {

	public float distance_ratio;
	public GameObject Canvas;
	public GameObject CenterMarker;

	private Vector2[, ,] MarkerPositions;
	private Vector2[, ,] EyeTrackerPositions;
	private Vector2[, ,] HeadTrackerPositions;

	private int currentDistance = 0;

	// Use this for initialization
	void Start () {
		MarkerPositions = new Vector2[2,3,3];
		EyeTrackerPositions = new Vector2[2, 3,3];
		HeadTrackerPositions = new Vector2[2, 3,3];

		setUpMarkers(distance_ratio);
	}
	
	// Update is called once per frame
	void Update () {
		int current_marker = 0;
		if(Input.GetKeyUp(KeyCode.R))
		{
			MarkerPositions[currentDistance, current_marker%3, current_marker/3] 
				= GameObject.Find(current_marker.ToString()).GetComponent<RectTransform>().position; //TODO

			current_marker++;
		}

		if(Input.GetKeyUp(KeyCode.N))
		{
			next_distance();
		}
	}

	public void next_distance()
	{
		currentDistance++;
		setUpMarkers(Mathf.Lerp(distance_ratio, 1, 0.5f));
	}

	private void setUpMarkers(float spread)
	{
		GameObject[] markers = GameObject.FindGameObjectsWithTag("marker");
		for(int i = 0; i < markers.Length; ++i)
		{
			Destroy(markers[i]);
		}

		spread = Mathf.Clamp01(spread);
		for(int x = 0; x < 3; x++)
		{
			for(int y = 0; y < 3; y++)
			{
				Vector2 pos = new Vector2((x-1) * spread * Screen.width * 0.5f, (y-1) * spread * Screen.height * 0.5f);
				pos.x += CenterMarker.GetComponent<RectTransform>().position.x;
				pos.y += CenterMarker.GetComponent<RectTransform>().position.y;

				GameObject marker = Instantiate(CenterMarker);
				marker.name = (x+y*3).ToString();
				marker.tag = "marker";

				marker.transform.SetParent(CenterMarker.transform.parent);
				marker.GetComponent<RectTransform>().position = pos;
				print(marker.GetComponent<RectTransform>().position);
			}
		}
	}
}
