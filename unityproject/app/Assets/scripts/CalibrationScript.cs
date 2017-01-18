using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CalibrationScript : MonoBehaviour {

    public int layers;
	public float distance_ratio;
	public GameObject Canvas;
	public GameObject CenterMarker;

	private Vector2[, ,] MarkerPositions;
	private Vector2[, ,] EyeTrackerPositions;
	private Vector2[, ,] HeadTrackerPositions;

	private int current_distance = 0;
    private GameObject current_marker;
    private int current_marker_num;

    // Use this for initialization
    void Start () {
		MarkerPositions = new Vector2[layers,3,3];
		EyeTrackerPositions = new Vector2[layers, 3,3];
		HeadTrackerPositions = new Vector2[layers, 3,3];

		setUpMarkers(distance_ratio);
        layers--;
	}
	
	// Update is called once per frame
	void Update () {
        //print(current_marker.GetComponent<Image>().color);
        if (Input.GetKeyUp(KeyCode.R))
		{
            current_marker.GetComponent<Image>().color = Color.green;
            MarkerPositions[current_distance, current_marker_num % 3, current_marker_num/3] 
				= current_marker.GetComponent<RectTransform>().position; //TODO

            if (current_marker_num < 8)
            {
                current_marker_num++;
                current_marker = GameObject.Find(current_marker_num.ToString());

                current_marker.GetComponent<Image>().color = Color.red;
            }
        }

		if(Input.GetKeyUp(KeyCode.N))
		{
            if (current_distance < layers)
            {
                next_distance();
            }
            else
            {
                print("Done");
            }
        }
	}

	public void next_distance()
	{
		current_distance++;
        distance_ratio = Mathf.Lerp(distance_ratio, 1, 0.5f);
        setUpMarkers(distance_ratio);
	}

	private void setUpMarkers(float spread)
	{
        current_marker_num = 0;
		GameObject[] markers = GameObject.FindGameObjectsWithTag("marker");
		for(int i = 0; i < markers.Length; ++i)
		{
            markers[i].name = "INVALID";
            Destroy(markers[i]);
		}

        GameObject marker = null;
        spread = Mathf.Clamp01(spread);
		for(int x = 0; x < 3; x++)
		{
			for(int y = 0; y < 3; y++)
			{
				Vector2 pos = new Vector2((x-1) * spread * Screen.width * 0.5f, (y-1) * spread * Screen.height * 0.5f);
				pos.x += CenterMarker.GetComponent<RectTransform>().position.x;
				pos.y += CenterMarker.GetComponent<RectTransform>().position.y;

				 marker = Instantiate(CenterMarker);
				marker.name = (x+y*3).ToString();
				marker.tag = "marker";

				marker.transform.SetParent(CenterMarker.transform.parent);
				marker.GetComponent<RectTransform>().position = pos;
				//print(marker.GetComponent<RectTransform>().position);
			}
		}

        current_marker = GameObject.Find("0");
        current_marker.GetComponent<Image>().color = Color.red;

    }
}
