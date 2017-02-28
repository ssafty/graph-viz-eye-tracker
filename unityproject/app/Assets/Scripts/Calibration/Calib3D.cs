
#define USE_MOUSE
//#define USE_PUPIL_EYE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;
using System.Collections;


public class Calib3D : MonoBehaviour
{

    
    private PupilGazeTracker gaze;

    // config for full calib setup
    public const int NUM_LAYERS = 1;
    //public const int NUM_LAYERS = 3;
    public const int MARKERS_PER_LAYER = 9;
    public const int CALIBRATION_ROUNDS = 2;

    //config for data storage per calib round
    public const int READINGS_PER_CALIBRATION = 10;
    public const int FRAMES_TO_WAIT_AT_START = 1;
    public const int FRAMES_TO_CAPTURE = Calib3D.READINGS_PER_CALIBRATION;
    public const int FRAMES_TO_WAIT_AT_END = 1;

    // marker layout arrangement
    private float marker_layout_scale = 4.0f;
    private float marker_layout_aspect = 2f;
    private float[] marker_layout_depth = { 30.0f, 20.0f, 10.0f };
   // private float[] marker_layout_depth = { 30.0f };

    // participant details
    public string participant_name;
	private string working_dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\EyeTracker\\" ;

    // object to store all participant data
    private Participant participant;

    // marker graphics
    private GameObject marker_parent_go;
    private GameObject[] markers_go = new GameObject[Calib3D.MARKERS_PER_LAYER];

    // state management for calibration
    private int current_layer = -1;
    private int current_calib_round = -1;
    private int current_marker = 0;
    private int current_frame_counter = -1;
	private bool enable_calib_3D = false;
    private bool wait_for_user_to_switch_calib_round = true;
    private bool wait_for_user_to_switch_layer = true;
    private bool calib_done_with_data_acquisation = false;
	private bool wait_for_python_to_get_job_done = true;

    private float current_left_pupil_x;
    private float current_left_pupil_y;


	public void OnGUI()
	{

		// enable the calibration
		if (!this.enable_calib_3D) {
			if (GUI.Button (new Rect (30, 30, 100, 50), "Calibrate 3D")) {

				// check if PupilGazeTracker is available. It is very important to have it working before Calib3D starts
				GameObject go = GameObject.Find ("PupilGazeTracker");
				if(go != null) this.gaze = go.GetComponent<PupilGazeTracker> ();
				#if USE_PUPIL_EYE
				if (go == null || this.gaze == null) {
					Debug.LogError ("Sadly PupilGazeTracker is not available. It is very important to have it working before Calib3D starts!!!");
					return;
				}
				#endif
				this.StartCalib3DScene ();
				this.enable_calib_3D = true;
			}
			return;
		}

		GUI.Box(new Rect(this.current_left_pupil_x - 15, Screen.height - this.current_left_pupil_y - 15, 30, 30), new GUIContent("[X]"));
	}


    // Use this for initialization
    void StartCalib3DScene()
    {
        // disable cursor
        //Cursor.visible = false;

        // get the participant name
        this.participant = new Participant();
        if (this.participant_name == null || this.participant_name == "")
        {
            Debug.LogWarning("You did not provide participant name using default name!!!");
            this.participant_name = "Unknown_Name";
		}
		this.participant_name += "__" + DateTime.Now;
		this.participant_name = this.participant_name.Replace("/", "_");
		this.participant_name = this.participant_name.Replace(":", "_");
		this.participant_name = this.participant_name.Replace(" ", "_");
		participant.name = this.participant_name;

        // initialize all the required layers and calib points to store the recorded data
        for (int i = 0; i < Calib3D.NUM_LAYERS; i++)
        {
            this.participant.layers[i] = new global::Calib3D.Layer();
            this.participant.layers[i].layer_id = i;
            this.participant.layers[i].A0 = 0.0f;
            this.participant.layers[i].A1 = 0.0f;
            this.participant.layers[i].A2 = 0.0f;
            this.participant.layers[i].A3 = 0.0f;
            this.participant.layers[i].A4 = 0.0f;
            this.participant.layers[i].B0 = 0.0f;
            this.participant.layers[i].B1 = 0.0f;
            this.participant.layers[i].B2 = 0.0f;
            this.participant.layers[i].B3 = 0.0f;
            this.participant.layers[i].B4 = 0.0f;

            for (int j = 0; j < Calib3D.MARKERS_PER_LAYER; j++)
            {
                this.participant.layers[i].markers[j] = new global::Calib3D.Marker();
                this.participant.layers[i].markers[j].marker_id = j;

                for (int k = 0; k < Calib3D.CALIBRATION_ROUNDS; k++)
                {
                    this.participant.layers[i].markers[j].calib_rounds[k] = new global::Calib3D.CalibRound();
                    this.participant.layers[i].markers[j].calib_rounds[k].calib_round = k;

                    for (int l = 0; l < Calib3D.READINGS_PER_CALIBRATION; l++)
                    {
                        this.participant.layers[i].markers[j].calib_rounds[k].left_x[l] = -9999.0f;
                        this.participant.layers[i].markers[j].calib_rounds[k].left_y[l] = -9999.0f;
                    }
                }
            }
        }

        // generate the graphics
        this.marker_parent_go = new GameObject("All Markers And Cursor");
        for (int i = 0; i < Calib3D.MARKERS_PER_LAYER; i++)
        {
            this.markers_go[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            this.markers_go[i].name = "Marker " + i;
            this.markers_go[i].transform.SetParent(this.marker_parent_go.transform);
            this.markers_go[i].gameObject.GetComponent<Renderer>().material.color = Color.green;

            if (i == 0)
            {
                this.markers_go[i].transform.position += new Vector3(-1 * this.marker_layout_aspect, 1, 0) * this.marker_layout_scale;
            }
            else if (i == 1)
            {
                this.markers_go[i].transform.position += new Vector3(0 * this.marker_layout_aspect, 1, 0) * this.marker_layout_scale;
            }
            else if (i == 2)
            {
                this.markers_go[i].transform.position += new Vector3(1 * this.marker_layout_aspect, 1, 0) * this.marker_layout_scale;
            }
            else if (i == 3)
            {
                this.markers_go[i].transform.position += new Vector3(-1 * this.marker_layout_aspect, 0, 0) * this.marker_layout_scale;
            }
            else if (i == 4)
            {
                this.markers_go[i].transform.position += new Vector3(0 * this.marker_layout_aspect, 0, 0) * this.marker_layout_scale;
            }
            else if (i == 5)
            {
                this.markers_go[i].transform.position += new Vector3(1 * this.marker_layout_aspect, 0, 0) * this.marker_layout_scale;
            }
            else if (i == 6)
            {
                this.markers_go[i].transform.position += new Vector3(-1 * this.marker_layout_aspect, -1, 0) * this.marker_layout_scale;
            }
            else if (i == 7)
            {
                this.markers_go[i].transform.position += new Vector3(0 * this.marker_layout_aspect, -1, 0) * this.marker_layout_scale;
            }
            else if (i == 8)
            {
                this.markers_go[i].transform.position += new Vector3(1 * this.marker_layout_aspect, -1, 0) * this.marker_layout_scale;
            }
        }

        this.marker_parent_go.transform.position = Vector3.one * -1000; // jsut throwing it somewhere far

    }

    // Update is called once per frame
    void Update()
    {
		// check if calib 3D is to be performed
		if (!this.enable_calib_3D) {
			return;
		}

        // acquire data from pupil wyw
        this.acquire_data();

        // if calib data is acquired completely ... wait for python to calculte coeffiecients and exit gracefully
        if (this.calib_done_with_data_acquisation)
        {
			if (this.wait_for_python_to_get_job_done) {
				// check if python has finished with calibration job
				if (File.Exists (this.working_dir + this.participant_name + ".calibjobdone")) {
					this.wait_for_python_to_get_job_done = false;
				} else {
					Debug.Log("Calibration data is acquired. Waiting for python to get the job done ...");
				}
			} else {
				Debug.Log("Great ... Python has done the job :) ... Check file " + this.participant_name + ".xml");
				if (this.gameObject.GetComponent<AfterCalib3D> () == null) {
					this.gameObject.AddComponent<AfterCalib3D> ();
				}
				Debug.Log("Enabeling the AfterCalib3D script");
				this.gameObject.GetComponent<AfterCalib3D>().enabled = true;
				Debug.Log("Load XML file that is calibrated by python script ...");
				this.gameObject.GetComponent<AfterCalib3D>().load_calib_file_and_initialize(this.participant_name, this.working_dir, this.gaze);
				Debug.Log("Enabeling the AfterCalib3D script");
				//this.gameObject.GetComponent<CollectCalibStats>().enabled = true;
				//Debug.Log("Load XML file that is calibrated by python script ...");
				//this.gameObject.GetComponent<CollectCalibStats>().init_game_for_collecting_statistics(this.participant_name, this.working_dir);
				//Debug.Log ("Destroy ..... the calib markers and participant XML object");
				Destroy (this.marker_parent_go);
				this.participant = null;
				Debug.Log("Destroy ..... the calib script");
				Destroy(this);
			}
            return;
        }

        // wait for next layer to be selected 
        if (this.wait_for_user_to_switch_layer)
        {
            Debug.Log("Start the layer " + (this.current_layer + 1) + " by pressing `L`!!!");
            if (Input.GetKeyDown(KeyCode.L))
            {
                this.wait_for_user_to_switch_layer = false;
                this.current_layer++;
                this.marker_parent_go.transform.position = this.transform.position + this.transform.forward * this.marker_layout_depth[this.current_layer];
                this.estimate_and_store_screen_pos_for_all_markers();
                this.current_calib_round = -1;
                Debug.Log("Layer " + this.current_layer + " is selected successfully!!!");
            }
            return;
        }

        // wait for next calib round to be selected
        if (this.wait_for_user_to_switch_calib_round)
        {
            Debug.Log("Press `R` to start with calibration round " + (this.current_calib_round + 1) + " for current layer " + this.current_layer + " !!!");
            if (Input.GetKeyDown(KeyCode.R))
            {
                this.wait_for_user_to_switch_calib_round = false;
                this.current_calib_round++;
                this.current_marker = 0;
                Debug.Log("Calibration round " + this.current_calib_round + " is selected!!!");
            }
            return;
        }

        // monitor frames
        this.current_frame_counter++;

        // skip initial frames
        if (this.current_frame_counter < Calib3D.FRAMES_TO_WAIT_AT_START)
        {
            Debug.Log("Skipping initial frames ...");
            this.markers_go[this.current_marker].gameObject.GetComponent<Renderer>().material.color = Color.magenta;
            return;
        }

        // store data for FRAMES_TO_CAPTURE number of frames
        if (this.current_frame_counter >= Calib3D.FRAMES_TO_WAIT_AT_START && this.current_frame_counter < Calib3D.FRAMES_TO_WAIT_AT_START + Calib3D.FRAMES_TO_CAPTURE)
        {

            // compute index
            int index = this.current_frame_counter - Calib3D.FRAMES_TO_WAIT_AT_START;
            Debug.Log("Storing frame " + index + " ... for marker " + this.current_marker + " ... at calib round " + this.current_calib_round + " ... at layer " + this.current_layer);
            this.markers_go[this.current_marker].gameObject.GetComponent<Renderer>().material.color = Color.red;

            this.populate_data(index);

            return;
        }

        // skip end frames
        if (this.current_frame_counter < Calib3D.FRAMES_TO_WAIT_AT_START + Calib3D.FRAMES_TO_CAPTURE + Calib3D.FRAMES_TO_WAIT_AT_END)
        {
            Debug.Log("Skipping end frames ...");
            this.markers_go[this.current_marker].gameObject.GetComponent<Renderer>().material.color = Color.magenta;
            return;
        }

        // change status at last frame
        if (this.current_frame_counter == Calib3D.FRAMES_TO_WAIT_AT_START + Calib3D.FRAMES_TO_CAPTURE + Calib3D.FRAMES_TO_WAIT_AT_END)
        {

            // reset frame counter when last frame 
            Debug.Log("Changing status to next marker ...");
            this.markers_go[this.current_marker].gameObject.GetComponent<Renderer>().material.color = Color.green;
            this.current_frame_counter = -1;

            // update marker count to point to next marker
            this.current_marker++;

            // change calib round if last frame of last marker
            if (this.current_marker == Calib3D.MARKERS_PER_LAYER)
            {
                Debug.Log("Changing status to next calib round ...");
                this.current_marker = 0; // reset marker count
                this.wait_for_user_to_switch_calib_round = true;

                // change layer if last frame of last marker of last calib round
                if (this.current_calib_round == Calib3D.CALIBRATION_ROUNDS - 1)
                {
                    Debug.Log("Changing status to next layer ...");
                    this.current_calib_round = -1;
                    this.wait_for_user_to_switch_layer = true;

                    // calib done and save file
                    if (this.current_layer == Calib3D.NUM_LAYERS - 1)
                    {
                        Debug.Log("Changing status to end calib data acquization ...");
                        this.calib_done_with_data_acquisation = true;
                        //Debug.Log("Calculating calibration statistics ..."); // this things can anyways be done in python now
                        //this.calculate_calibration_statistics_mean();
                        Debug.Log("Saving XML file that can be used by python ...");
						this.WriteXML();
						Debug.Log("Calculating coefficents and any other things with python externally ...");
						this.give_python_a_job();
                    }
                }
            }

            return;
        }
	}

	public void give_python_a_job()
	{
		string calibjob_file_path = this.working_dir + this.participant_name + ".calibjob";
		if (!File.Exists(calibjob_file_path))
		{
			// create job file
			File.Create(calibjob_file_path);
		}
		else
		{
			Debug.LogError("This should not happen");
		}
	}

    public void calculate_calibration_statistics_mean()
    {

        for (int i = 0; i < Calib3D.NUM_LAYERS; i++)
        {
            Calib3D.Layer layer = this.participant.layers[i];

            for (int j = 0; j < Calib3D.MARKERS_PER_LAYER; j++)
            {
                Calib3D.Marker marker = layer.markers[j];

                float calib_left_x = 0.0f;
                float calib_left_y = 0.0f;

                for (int k = 0; k < Calib3D.CALIBRATION_ROUNDS; k++)
                {
                    Calib3D.CalibRound calib_rnd = marker.calib_rounds[k];

                    for (int l = 0; l < Calib3D.READINGS_PER_CALIBRATION; l++)
                    {
                        calib_left_x += calib_rnd.left_x[l];
                        calib_left_y += calib_rnd.left_y[l];
                    }
                }

                marker.calib_mean_left_x = calib_left_x / (float)(Calib3D.CALIBRATION_ROUNDS * Calib3D.READINGS_PER_CALIBRATION);
                marker.calib_mean_left_y = calib_left_y / (float)(Calib3D.CALIBRATION_ROUNDS * Calib3D.READINGS_PER_CALIBRATION);
            }
        }
    }

    public void estimate_and_store_screen_pos_for_all_markers()
    {
        for (int i = 0; i < Calib3D.MARKERS_PER_LAYER; i++)
        {
            Vector3 screen_pos = Camera.main.WorldToScreenPoint(this.markers_go[i].transform.position);
            this.participant.layers[this.current_layer].markers[i].screen_x = screen_pos.x;
            this.participant.layers[this.current_layer].markers[i].screen_y = screen_pos.y;
            Debug.Log(screen_pos);
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

    public void populate_data(int index)
    {
        CalibRound calib = this.participant.layers[this.current_layer].markers[this.current_marker].calib_rounds[this.current_calib_round];

        // load data from sensor here

        calib.left_x[index] = this.current_left_pupil_x;
        calib.left_y[index] = this.current_left_pupil_y;
    }


    [XmlRoot("root")]
    public class Participant
    {

        [XmlAttribute("name")]
        public string name;

        [XmlArrayItem("layer")]
        public Layer[] layers = new Layer[Calib3D.NUM_LAYERS];

    }

    [XmlType("layer")]
    public class Layer
    {
        [XmlAttribute("layer_id")]
        public int layer_id;

        [XmlAttribute("A0")]
        public float A0;
        [XmlAttribute("A1")]
        public float A1;
        [XmlAttribute("A2")]
        public float A2;
        [XmlAttribute("A3")]
        public float A3;
        [XmlAttribute("A4")]
        public float A4;
        [XmlAttribute("B0")]
        public float B0;
        [XmlAttribute("B1")]
        public float B1;
        [XmlAttribute("B2")]
        public float B2;
        [XmlAttribute("B3")]
        public float B3;
        [XmlAttribute("B4")]
        public float B4;

        [XmlArrayItem("marker")]
        public Marker[] markers = new Marker[Calib3D.MARKERS_PER_LAYER];

    }

    [XmlType("marker")]
    public class Marker
    {
        [XmlAttribute("marker_id")]
        public int marker_id;

        [XmlAttribute("screen_x")]
        public float screen_x;

        [XmlAttribute("screen_y")]
        public float screen_y;

        [XmlAttribute("calib_mean_left_x")]
        public float calib_mean_left_x;

        [XmlAttribute("calib_mean_left_y")]
        public float calib_mean_left_y;

        [XmlArrayItem("calib_round")]
        public CalibRound[] calib_rounds = new CalibRound[Calib3D.CALIBRATION_ROUNDS];

    }

    [XmlType("calib_round")]
    public class CalibRound
    {
        [XmlAttribute("calib_round")]
        public int calib_round;

        [XmlArrayItem("left_x")]
        public float[] left_x = new float[Calib3D.READINGS_PER_CALIBRATION];

        [XmlArrayItem("left_y")]
        public float[] left_y = new float[Calib3D.READINGS_PER_CALIBRATION];

    }

    public void WriteXML()
    {
        System.Xml.Serialization.XmlSerializer writer =
            new System.Xml.Serialization.XmlSerializer(typeof(Participant));

		var path = this.working_dir + this.participant_name + ".xml";
        System.IO.FileStream file = System.IO.File.Create(path);

        writer.Serialize(file, this.participant);
        file.Close();
    }

}
