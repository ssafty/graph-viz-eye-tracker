using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;
using System.Collections;

public class Calib3D : MonoBehaviour {

    // config
    public const int NUM_LAYERS = 3;
    public const int MARKERS_PER_LAYER = 9;
    public const int CALIBRATIONS_PER_MARKER = 5;
    public const int READINGS_PER_CALIBRATION = 30;

    // participant details
    public string participant_name;

    // object to store all participant data
    private Participant participant;

    // marker graphics
    private GameObject marker_parent_go;
    private GameObject[] markers_go = new GameObject[Calib3D.MARKERS_PER_LAYER];

    // marker layout arrangement
    private float marker_layout_scale = 4.0f;
    private float marker_layout_aspect = 2f;
    private float marker_layout_depth = 10.0f;

    // state storage for calibration
    private int current_layer = -1;
    private int current_calib_round = -1;
    private int current_marker = -1;
    private int current_frame = -1;
    private bool start_calib = false;
    private bool start_layer = false;

    // Use this for initialization
    void Start () {

        // get the participant name
        this.participant = new Participant();
        participant.name = this.participant_name;

        // initialize all the required layers and calib points to store the recorded data
        for(int i = 0; i < Calib3D.NUM_LAYERS; i++)
        {
            this.participant.layers[i] = new global::Calib3D.Layer();
            this.participant.layers[i].layer_id = i;

            for (int j=0; j< Calib3D.MARKERS_PER_LAYER; j++)
            {
                this.participant.layers[i].markers[j] = new global::Calib3D.Marker();
                this.participant.layers[i].markers[j].marker_id = j;

                for (int k=0; k<Calib3D.CALIBRATIONS_PER_MARKER; k++)
                {
                    this.participant.layers[i].markers[j].calibrations[k] = new global::Calib3D.Calib();
                    this.participant.layers[i].markers[j].calibrations[k].calib_id = k;

                    for (int l = 0; l<Calib3D.READINGS_PER_CALIBRATION; l++)
                    {
                        this.participant.layers[i].markers[j].calibrations[k].left_x[l] = -9999.0f;
                        this.participant.layers[i].markers[j].calibrations[k].left_y[l] = -9999.0f;
                        this.participant.layers[i].markers[j].calibrations[k].right_x[l] = -9999.0f;
                        this.participant.layers[i].markers[j].calibrations[k].right_y[l] = -9999.0f;
                    }
                }
            }
        }

        // generate the graphics
        this.marker_parent_go = new GameObject("All Markers");
        for (int i = 0; i < Calib3D.MARKERS_PER_LAYER; i++)
        {
            this.markers_go[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            this.markers_go[i].name = "Marker " + i;
            this.markers_go[i].transform.SetParent(this.marker_parent_go.transform);

            if (i == 0)
            {
                this.markers_go[i].transform.position += new Vector3(-1 * this.marker_layout_aspect, -1, 0) * this.marker_layout_scale;
            }
            else if (i == 1)
            {
                this.markers_go[i].transform.position += new Vector3(-1 * this.marker_layout_aspect, 0, 0) * this.marker_layout_scale;
            }
            else if (i == 2)
            {
                this.markers_go[i].transform.position += new Vector3(-1 * this.marker_layout_aspect, 1, 0) * this.marker_layout_scale;
            }
            else if (i == 3)
            {
                this.markers_go[i].transform.position += new Vector3(0 * this.marker_layout_aspect, -1, 0) * this.marker_layout_scale;
            }
            else if (i == 4)
            {
                //
            }
            else if (i == 5)
            {
                this.markers_go[i].transform.position += new Vector3(0 * this.marker_layout_aspect, 1, 0) * this.marker_layout_scale;
            }
            else if (i == 6)
            {
                this.markers_go[i].transform.position += new Vector3(1 * this.marker_layout_aspect, -1, 0) * this.marker_layout_scale;
            }
            else if (i == 7)
            {
                this.markers_go[i].transform.position += new Vector3(1 * this.marker_layout_aspect, 0, 0) * this.marker_layout_scale;
            }
            else if (i == 8)
            {
                this.markers_go[i].transform.position += new Vector3(1 * this.marker_layout_aspect, 1, 0) * this.marker_layout_scale;
            }
        }

        this.marker_parent_go.transform.position = this.transform.position + this.transform.forward * 10;

        

        WriteXML(participant);


    }

    // Update is called once per frame
    void Update()
    {

        // wait for next layer to be selected 
        if (!this.start_layer)
        {
            Debug.Log("Start the first layer by pressing `N`!!!");
            if(Input.GetKeyDown(KeyCode.N))
            {
                this.start_layer = true;
                this.current_layer++;
                Debug.Log("Layer "+this.current_layer+" is selected successfully!!!");
            }
            return;
        }

        // wait for next calib round to be selected
        if (!this.start_calib)
        {
            Debug.Log("Press `Space` to start with calibration round for this layer!!!");
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.start_calib = true;
                this.current_calib_round++;
                Debug.Log("Calibration round " + this.current_calib_round + " is selected!!!");
            }
            return;
        }

        // logic to switch between markers
        if(this.start_calib && this.start_layer)
        {

        }
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

        [XmlArrayItem("calib")]
        public Marker[] markers = new Marker[Calib3D.MARKERS_PER_LAYER];

    }

    [XmlType("marker")]
    public class Marker
    {
        [XmlAttribute("marker_id")]
        public int marker_id;

        [XmlArrayItem("calib")]
        public Calib[] calibrations = new Calib[Calib3D.CALIBRATIONS_PER_MARKER];

    }

    [XmlType("calib")]
    public class Calib
    {
        [XmlAttribute("calib_id")]
        public int calib_id;

        [XmlArrayItem("left_x")]
        public float[] left_x = new float[Calib3D.READINGS_PER_CALIBRATION];

        [XmlArrayItem("left_y")]
        public float[] left_y = new float[Calib3D.READINGS_PER_CALIBRATION];

        [XmlArrayItem("right_x")]
        public float[] right_x = new float[Calib3D.READINGS_PER_CALIBRATION];

        [XmlArrayItem("right_y")]
        public float[] right_y = new float[Calib3D.READINGS_PER_CALIBRATION];

    }

    public static void WriteXML(Participant participant)
    {
        System.Xml.Serialization.XmlSerializer writer =
            new System.Xml.Serialization.XmlSerializer(typeof(Participant));

        var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//"+participant.name+".xml";
        System.IO.FileStream file = System.IO.File.Create(path);

        writer.Serialize(file, participant);
        file.Close();
    }

}

