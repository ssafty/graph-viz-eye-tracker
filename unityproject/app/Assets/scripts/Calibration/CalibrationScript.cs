using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

using MatrixMath;

public class CalibrationScript : MonoBehaviour {

    public int recording_length;    //In Frames!
    public int layers;
	public float distance_ratio;
	public GameObject Canvas;
	public GameObject CenterMarker;

	private Vector2[, ,] MarkerPositions;
	private Vector2[, ,] EyeTrackerPositions;
	private Vector2[, ,] HeadTrackerPositions;

	private int layer_id = 0;
    private GameObject current_marker;
    private int current_marker_num;

    private bool recording = false;
    private List<Vector2> recording_accumulation;
    private Vector2 gaze_average = Vector2.zero;
    private int recording_progress = 0;

    // Use this for initialization
    void Start () {
		MarkerPositions = new Vector2[layers,3,3];
		EyeTrackerPositions = new Vector2[layers, 3,3];
		HeadTrackerPositions = new Vector2[layers, 3,3];

		setUpMarkers(distance_ratio);
        layers--;

        recording_accumulation = new List<Vector2>();
	}
	
	// Update is called once per frame
	void Update () {


        if (Input.GetKeyUp(KeyCode.N))   //Switch to next layer
        {
            if (layer_id < layers)
            {
                next_layer();
            }
            else
            {
                print("Done");
            }
        }


        if (Input.inputString == "1" || Input.inputString == "2" || Input.inputString == "3" || Input.inputString == "4" || Input.inputString == "5" || Input.inputString == "6" || Input.inputString == "7" || Input.inputString == "8" || Input.inputString == "9")
        {
            current_marker_num = Int32.Parse(Input.inputString) - 1;
            current_marker = GameObject.Find(current_marker_num.ToString());
            current_marker.GetComponent<Image>().color = Color.red;
            
            record(); //Start average recording
        }

        

        //Recording Gaze Vector
        if (recording)
        {
            Debug.Log("Gathering points .....");
            //print("hi");
            recording_accumulation.Add(Vector2.one);  //TODO Insert real position
            recording_progress++;
        }

        if  (recording_progress >= recording_length || Input.GetKeyUp(KeyCode.S))    
        {
            if (!recording || current_marker == null)
            {
                Debug.Log("Cannot stop recording as you have not started recording yet .... Please select the marker with num keys from 1 to 9!!!");
                return;
            }
            
            Debug.Log("Stopped recording .....");

            Vector2 sum = Vector2.zero;     //Sum of all recorded gaze values
            foreach (Vector2 vec in recording_accumulation)
            {
                sum += vec;
            }
            gaze_average = sum / recording_length;
            recording = false;
            recording_progress = 0;

            //Averaging
            //
            //la_calculations();

            // display status
            current_marker.GetComponent<Image>().color = Color.green;
            MarkerPositions[layer_id, current_marker_num % 3, current_marker_num / 3]
                = current_marker.GetComponent<RectTransform>().position; //TODO
            EyeTrackerPositions[layer_id, current_marker_num % 3, current_marker_num / 3]
                = gaze_average;
        }
        
    }

	public void next_layer()
	{
		layer_id++;
        distance_ratio = Mathf.Lerp(distance_ratio, 1, 0.5f);
        setUpMarkers(distance_ratio);
	}

	private void setUpMarkers(float spread)
	{
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

        current_marker_num = -99;
        //current_marker = GameObject.Find("0");
        //current_marker.GetComponent<Image>().color = Color.red;

    }

    public void record()
    {
        recording = true;
        recording_progress = 0;
        recording_accumulation = new List<Vector2>(); ;
    }

    private void la_calculations()
    {
        /*
        //Building parsing String:
        string m_string = "";
        
        foreach (Vector2 vec in recording_accumulation)
        {
            m_string += (vec.x - gaze_average.x).ToString() + " " + (vec.y - gaze_average.y).ToString() + "\r\n";
        }
         Matrix P = Matrix.Parse(m_string);

        Matrix C = P * Matrix.Transpose(P);

        // list to matrix
        //Meta.Num

        // eigen
        //Matrix vector;
        //Matrix values;
        //Matrix.Eigen(MatrixName, out values, out vector);
        */



    }

}
