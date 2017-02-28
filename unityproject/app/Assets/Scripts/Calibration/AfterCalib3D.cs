
#define USE_MOUSE
//#define USE_PUPIL_EYE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AfterCalib3D : MonoBehaviour {


	private PupilGazeTracker gaze;

    // participant details
    private string participant_name;
	private string working_dir;

    // select layer
    private int selectLayer = 0;
    private bool useCalibration = true;

    private float[,] all_layers_coeff_A = new float[Calib3D.NUM_LAYERS, 5];
    private float[,] all_layers_coeff_B = new float[Calib3D.NUM_LAYERS, 5];

    // object that holds all participant data
    private Calib3D.Participant participant;

    private float current_left_pupil_x;
    private float current_left_pupil_y;
    private float left_pupil_x;
    private float left_pupil_y;

    public void use_calibration(bool use)
    {
        this.useCalibration = use;
    }

	public Vector2 get_cursor_vec()
	{
		if (this.enabled) {
			return new Vector2 (this.left_pupil_x, this.left_pupil_y);
		} else {
			Debug.LogError ("You cannot use the coordinates as calibration is not done ..");
			return Vector2.zero;
		}
	}

	public void OnGUI()
	{
		GUI.Box(new Rect(this.left_pupil_x - 15, Screen.height - this.left_pupil_y - 15, 30, 30), new GUIContent("[C]"));
		GUI.Box(new Rect(this.current_left_pupil_x - 15, Screen.height - this.current_left_pupil_y - 15, 30, 30), new GUIContent("[O]"));
	}

    // Use this for initialization
	public void load_calib_file_and_initialize (string participant_name, string working_dir, PupilGazeTracker gaze)
    {
        // disable cursor
        // Cursor.visible = false;

        this.participant_name = participant_name;
		this.working_dir = working_dir;
		this.gaze = gaze;

		#if USE_PUPIL_EYE
		if (this.gaze == null) {
			Debug.LogError ("Sadly PupilGazeTracker is not available !!!");
		}
		#endif

        this.ReadXML();
    }
	
	// Update is called once per frame
	void Update ()
    {
        // acquire data from pupil wyw
        this.acquire_data();

        // calibrate
        this.calibrate_based_on_selected_latyer();

    }

    public void calibrate_based_on_selected_latyer()
    {
        if (useCalibration)
        {
            this.left_pupil_x =
                all_layers_coeff_A[this.selectLayer, 0] +
                all_layers_coeff_A[this.selectLayer, 1] * this.current_left_pupil_x +
                all_layers_coeff_A[this.selectLayer, 2] * this.current_left_pupil_y +
                all_layers_coeff_A[this.selectLayer, 3] * this.current_left_pupil_x * this.current_left_pupil_x +
                all_layers_coeff_A[this.selectLayer, 4] * this.current_left_pupil_y * this.current_left_pupil_y;
            this.left_pupil_y =
                all_layers_coeff_B[this.selectLayer, 0] +
                all_layers_coeff_B[this.selectLayer, 1] * this.current_left_pupil_x +
                all_layers_coeff_B[this.selectLayer, 2] * this.current_left_pupil_y +
                all_layers_coeff_B[this.selectLayer, 3] * this.current_left_pupil_x * this.current_left_pupil_x +
                all_layers_coeff_B[this.selectLayer, 4] * this.current_left_pupil_y * this.current_left_pupil_y;
        }
        else
        {
            this.left_pupil_x = this.current_left_pupil_x;
            this.left_pupil_y = this.current_left_pupil_y;
        }

    }

	public void acquire_data()
	{
		// fake pupil eye with mouse

		#if USE_MOUSE
		this.current_left_pupil_x = Input.mousePosition.x;
		this.current_left_pupil_y = Input.mousePosition.y;
		#endif
		#if USE_PUPIL_EYE
		this.current_left_pupil_x = gaze.LeftEyePos.x;
		this.current_left_pupil_y = gaze.LeftEyePos.y;
		#endif


	}

    public void ReadXML()
    {
        System.Xml.Serialization.XmlSerializer serializer =
            new System.Xml.Serialization.XmlSerializer(typeof(Calib3D.Participant));

		System.IO.FileStream file = System.IO.File.OpenRead(this.working_dir + this.participant_name + ".xml");

        this.participant = (Calib3D.Participant)serializer.Deserialize(file);

        

        for (int i = 0; i < Calib3D.NUM_LAYERS; i++)
        {
            this.all_layers_coeff_A[i, 0] = this.participant.layers[i].A0;
            this.all_layers_coeff_A[i, 1] = this.participant.layers[i].A1;
            this.all_layers_coeff_A[i, 2] = this.participant.layers[i].A2;
            this.all_layers_coeff_A[i, 3] = this.participant.layers[i].A3;
            this.all_layers_coeff_A[i, 4] = this.participant.layers[i].A4;
            this.all_layers_coeff_B[i, 0] = this.participant.layers[i].B0;
            this.all_layers_coeff_B[i, 1] = this.participant.layers[i].B1;
            this.all_layers_coeff_B[i, 2] = this.participant.layers[i].B2;
            this.all_layers_coeff_B[i, 3] = this.participant.layers[i].B3;
            this.all_layers_coeff_B[i, 4] = this.participant.layers[i].B4;
        }

        file.Close();
    }
}
