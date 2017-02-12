#define USE_LEFT_EYE
//#define USE_RIGHT_EYE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestAfterCalib3D : MonoBehaviour
{

    public PupilGazeTracker gaze;

    // participant details
    public string participant_name;
    private string calib_file_path;

    // select layer
    public int selectLayer = 0;
    public bool useCalibration = false;

    private float[,] all_layers_coeff_A = new float[Calib3D.NUM_LAYERS, 5];
    private float[,] all_layers_coeff_B = new float[Calib3D.NUM_LAYERS, 5];

    // object that holds all participant data
    private Calib3D.Participant participant;

#if USE_LEFT_EYE
    private float current_left_pupil_x;
    private float current_left_pupil_y;
#endif
#if USE_RIGHT_EYE
    private float current_right_pupil_x;
    private float current_right_pupil_y;
#endif
#if USE_LEFT_EYE
    private float calibrated_left_pupil_x;
    private float calibrated_left_pupil_y;
#endif
#if USE_RIGHT_EYE
    private float calibrated_right_pupil_x;
    private float calibrated_right_pupil_y;
#endif

    // Use this for initialization
    void Start()
    {
        // disable cursor
        Cursor.visible = false;

        // get the participant name
        if (this.participant_name == null || this.participant_name == "")
        {
            Debug.LogWarning("You did not provide participant name using default name!!!");
            this.participant_name = "Default Name";
        }
        this.calib_file_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//" + this.participant_name + ".xml";
        this.ReadXML();
    }

    // Update is called once per frame
    void Update()
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
            this.calibrated_left_pupil_x =
                all_layers_coeff_A[this.selectLayer, 0] +
                all_layers_coeff_A[this.selectLayer, 1] * this.current_left_pupil_x +
                all_layers_coeff_A[this.selectLayer, 2] * this.current_left_pupil_y +
                all_layers_coeff_A[this.selectLayer, 3] * this.current_left_pupil_x * this.current_left_pupil_x +
                all_layers_coeff_A[this.selectLayer, 4] * this.current_left_pupil_y * this.current_left_pupil_y;
            this.calibrated_left_pupil_y =
                all_layers_coeff_B[this.selectLayer, 0] +
                all_layers_coeff_B[this.selectLayer, 1] * this.current_left_pupil_x +
                all_layers_coeff_B[this.selectLayer, 2] * this.current_left_pupil_y +
                all_layers_coeff_B[this.selectLayer, 3] * this.current_left_pupil_x * this.current_left_pupil_x +
                all_layers_coeff_B[this.selectLayer, 4] * this.current_left_pupil_y * this.current_left_pupil_y;
        }
        else
        {
            this.calibrated_left_pupil_x = this.current_left_pupil_x;
            this.calibrated_left_pupil_y = this.current_left_pupil_y;
        }

    }

    public void OnGUI()
    {
#if USE_LEFT_EYE
        GUI.Box(new Rect(this.calibrated_left_pupil_x - 15, Screen.height - this.calibrated_left_pupil_y - 15, 30, 30), new GUIContent("[C]"));
        GUI.Box(new Rect(this.current_left_pupil_x - 15, Screen.height - this.current_left_pupil_y - 15, 30, 30), new GUIContent("[O]"));
#endif
#if USE_RIGHT_EYE
        GUI.Box(new Rect(this.calibrated_right_pupil_x - 15, Screen.height - this.calibrated_right_pupil_y - 15, 30, 30), new GUIContent("[X]"));
        GUI.Box(new Rect(this.current_right_pupil_x - 15, Screen.height - this.current_right_pupil_y - 15, 30, 30), new GUIContent("[O]"));
#endif
    }


    public void acquire_data()
    {
        // fake pupil eye with mouse

#if USE_LEFT_EYE
        //this.current_left_pupil_x = Input.mousePosition.x;
        //this.current_left_pupil_y = Input.mousePosition.y;
        this.current_left_pupil_x = gaze.LeftEyePos.x;
        this.current_left_pupil_y = gaze.LeftEyePos.y;
#endif
#if USE_RIGHT_EYE
        //this.current_right_pupil_x = Input.mousePosition.x;
        //this.current_right_pupil_y = Input.mousePosition.y;
        this.current_right_pupil_x = gaze.RightEyePos.x;
        this.current_right_pupil_y = gaze.RightEyePos.y;
#endif

    }

    public void ReadXML()
    {
        System.Xml.Serialization.XmlSerializer serializer =
            new System.Xml.Serialization.XmlSerializer(typeof(Calib3D.Participant));

        System.IO.FileStream file = System.IO.File.OpenRead(this.calib_file_path);

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
