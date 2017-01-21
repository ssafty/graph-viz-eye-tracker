using UnityEngine;
using System.Collections;
using System;
using System.ComponentModel;
using System.Reflection;

public class Node : MonoBehaviour
{
	public int id;
	public string id_string;
	public int count;
	public TextMesh nodeText;
	public  string title = "EMTPY";
	public  string desc = "EMTPY";
	private GameObject infoController;
	public GameObject labelPrefab;

	public float opaque_distance; 	// Distance at which a Node with size 2 is fully opaque (currently 40f)
	private GameObject cam; 		// Scene Camera
	private float opaqueClamp; 		// Distance at which this Node is fully opaque
	private Renderer rend;
	public float scale_size;

	private float default_red;
	private float default_blue;
	private float default_green;
    public bool gotHit = false;
    public bool derAuserwaehlte = false;
    public Color neighborColor = new Color(0.9f, 0.1f, 0.1f);
    public Color mainColor = new Color(0.1f, 0.1f, 0.9f);
    void Start ()
	{
		cam = GameObject.Find ("MainCamera");
        rend = GetComponent<Renderer>();

        default_red = rend.material.color.r;
		default_blue = rend.material.color.b;
		default_green = rend.material.color.g;


		// Bigger Nodes become transparent more easily
		opaqueClamp = opaque_distance * transform.lossyScale.magnitude / 2.0f;
	}
	
	// Update is called once per frame
	void Update ()
	{ 
		Vector3 targetPosition = Camera.main.transform.position;
		Vector3 currentPosition = transform.position;
		targetPosition.y = 0;
		currentPosition.y = 0;
		nodeText.transform.rotation = Quaternion.LookRotation (currentPosition - targetPosition, Vector3.up);

		//Calculate opacity
		float opacity = Vector3.Distance (transform.position, cam.transform.position);
		opacity = Mathf.Clamp (opacity, 0.0f, opaqueClamp) / opaqueClamp;

		//Set opacity
		Color color = rend.material.color;
		color.a = opacity;
		rend.material.color = color;
//		Color color = new Color (rend.material.color.r, rend.material.color.b, rend.material.color.g, opacity);
//		rend.material.color = color;
	}
    public void Highlight(Color color)
    {
        rend = GetComponent<Renderer>();
        rend.material.color = color;
    }
	public void HighlightAsNeighbor () {
        Highlight(neighborColor);
    }

	public void HighlightAsMain(){
        Highlight(mainColor);
	}

	public void HighlightDefault(){
		rend.material.color = new Color (default_red, default_blue, default_green, rend.material.color.a);
	}

    public static Node GetNodeWithId(int id)
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");
        foreach(GameObject node in nodes)
        {
            if(node.GetComponent<Node>().id == id)
            {
                return node.GetComponent<Node>();
            }
        }
        return nodes[0].GetComponent<Node>();
    }
     
}
