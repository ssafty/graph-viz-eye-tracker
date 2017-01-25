
#define USE_LEFT_EYE
#define USE_RIGHT_EYE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;
using System.Collections;


public class Calib3D : MonoBehaviour {

    // config for full calib setup
    public const int NUM_LAYERS = 3;
    public const int MARKERS_PER_LAYER = 9;
    public const int CALIBRATION_ROUNDS = 2;

    //config for data storage per calib round
    public const int READINGS_PER_CALIBRATION = 30;
    public const int FRAMES_TO_WAIT_AT_START = 10;
    public const int FRAMES_TO_CAPTURE = Calib3D.READINGS_PER_CALIBRATION;
    public const int FRAMES_TO_WAIT_AT_END = 10;

    // marker layout arrangement
    private float marker_layout_scale = 4.0f;
    private float marker_layout_aspect = 2f;
    private float[] marker_layout_depth = {30.0f, 20.0f, 10.0f};

    // participant details
    public string participant_name;

    // object to store all participant data
    private Participant participant;

    // marker graphics
    private GameObject marker_parent_go;
    private GameObject[] markers_go = new GameObject[Calib3D.MARKERS_PER_LAYER];

    // state storage for calibration
    private int current_layer = -1;
    private int current_calib_round = -1;
    private int current_marker = 0;
    private int current_frame_counter = -1;
    private bool wait_for_user_to_switch_calib_round = true;
    private bool wait_for_user_to_switch_layer = true;
    private bool calib_done = false;
    private string calib_file_path;


    // Use this for initialization
    void Start () {

        // get the participant name
        this.participant = new Participant();
        if (this.participant_name == null || this.participant_name == "")
        {
            Debug.LogWarning("You did not provide participant name using default name!!!");
            this.participant_name = "Default Name";
        }
        participant.name = this.participant_name;
        this.calib_file_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//" + participant.name + ".xml";

        // initialize all the required layers and calib points to store the recorded data
        for (int i = 0; i < Calib3D.NUM_LAYERS; i++)
        {
            this.participant.layers[i] = new global::Calib3D.Layer();
            this.participant.layers[i].layer_id = i;

            for (int j=0; j< Calib3D.MARKERS_PER_LAYER; j++)
            {
                this.participant.layers[i].markers[j] = new global::Calib3D.Marker();
                this.participant.layers[i].markers[j].marker_id = j;

                for (int k=0; k<Calib3D.CALIBRATION_ROUNDS; k++)
                {
                    this.participant.layers[i].markers[j].calib_rounds[k] = new global::Calib3D.CalibRound();
                    this.participant.layers[i].markers[j].calib_rounds[k].calib_round = k;

                    for (int l = 0; l<Calib3D.READINGS_PER_CALIBRATION; l++)
                    {
                        this.participant.layers[i].markers[j].calib_rounds[k].left_x[l] = -9999.0f;
                        this.participant.layers[i].markers[j].calib_rounds[k].left_y[l] = -9999.0f;
                        this.participant.layers[i].markers[j].calib_rounds[k].right_x[l] = -9999.0f;
                        this.participant.layers[i].markers[j].calib_rounds[k].right_y[l] = -9999.0f;
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
        // if calib done nothing to do
        if(this.calib_done)
        {
            Debug.Log("Calibration is completed. Check file " + this.calib_file_path);
            return;
        }

        // wait for next layer to be selected 
        if (this.wait_for_user_to_switch_layer)
        {
            Debug.Log("Start the layer " + (this.current_layer+1) + " by pressing `N`!!!");
            if(Input.GetKeyDown(KeyCode.N))
            {
                this.wait_for_user_to_switch_layer = false;
                this.current_layer++;
                this.marker_parent_go.transform.position = this.transform.position + this.transform.forward * this.marker_layout_depth[this.current_layer];
                this.current_calib_round = -1;
                Debug.Log("Layer "+this.current_layer+" is selected successfully!!!");
            }
            return;
        }

        // wait for next calib round to be selected
        if (this.wait_for_user_to_switch_calib_round)
        {
            Debug.Log("Press `Space` to start with calibration round " + (this.current_calib_round + 1) + " for current layer " + this.current_layer + " !!!");
            if (Input.GetKeyDown(KeyCode.Space))
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
            Debug.Log("Storing frame " + index + " ... for marker " + this.current_marker  + " ... at calib round " + this.current_calib_round + " ... at layer " + this.current_layer);
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
                if (this.current_calib_round == Calib3D.CALIBRATION_ROUNDS-1)
                {
                    Debug.Log("Changing status to next layer ...");
                    this.current_calib_round = -1;
                    this.wait_for_user_to_switch_layer = true;

                    // calib done and save file
                    if (this.current_layer == Calib3D.NUM_LAYERS - 1)
                    {
                        Debug.Log("Changing status to end calib and saving file ...");
                        this.calib_done = true;
                        this.WriteXML();
                    }
                }
            }

            return;
        }
    }

    public void populate_data(int index)
    {
        CalibRound calib = this.participant.layers[this.current_layer].markers[this.current_marker].calib_rounds[this.current_calib_round];

        // load data from sensor here
#if USE_LEFT_EYE
        calib.left_x[index] = 1.1111f;
        calib.left_y[index] = 2.2222f;
#endif
#if USE_RIGHT_EYE
        calib.right_x[index] = 3.3333f;
        calib.right_y[index] = 4.4444f;
#endif
    }
    

    [XmlRoot("root")]
    public class Participant
    {

        [XmlAttribute("name")]
        public string name;

        [XmlArrayItem("layer")]
        public Layer[] layers= new Layer[Calib3D.NUM_LAYERS];

    }

    [XmlType("layer")]
    public class Layer
    {
        [XmlAttribute("layer_id")]
        public int layer_id;

        [XmlArrayItem("marker")]
        public Marker[] markers = new Marker[Calib3D.MARKERS_PER_LAYER];

    }

    [XmlType("marker")]
    public class Marker
    {
        [XmlAttribute("marker_id")]
        public int marker_id;

        [XmlArrayItem("calib_round")]
        public CalibRound[] calib_rounds = new CalibRound[Calib3D.CALIBRATION_ROUNDS];

    }

    [XmlType("calib_round")]
    public class CalibRound
    {
        [XmlAttribute("calib_round")]
        public int calib_round;

#if USE_LEFT_EYE

        [XmlArrayItem("left_x")]
        public float[] left_x = new float[Calib3D.READINGS_PER_CALIBRATION];

        [XmlArrayItem("left_y")]
        public float[] left_y = new float[Calib3D.READINGS_PER_CALIBRATION];

#endif
#if USE_RIGHT_EYE

        [XmlArrayItem("right_x")]
        public float[] right_x = new float[Calib3D.READINGS_PER_CALIBRATION];

        [XmlArrayItem("right_y")]
        public float[] right_y = new float[Calib3D.READINGS_PER_CALIBRATION];

#endif

    }

    public void WriteXML()
    {
        System.Xml.Serialization.XmlSerializer writer =
            new System.Xml.Serialization.XmlSerializer(typeof(Participant));

        var path = this.calib_file_path;
        System.IO.FileStream file = System.IO.File.Create(path);

        writer.Serialize(file, this.participant);
        file.Close();
    }

}

