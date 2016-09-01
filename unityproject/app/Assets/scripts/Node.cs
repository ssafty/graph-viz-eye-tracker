using UnityEngine;
using System.Collections;
using System;
using System.ComponentModel;
using System.Reflection;

public class Node : MonoBehaviour
{
	// parts and properties
	public string id;
	public int count;
	public TextMesh nodeText;
	public  string title = "EMTPY";
	public  string desc = "EMTPY";
	public GameObject labelPrefab;

	// Distance at which a Node with size 2 is fully opaque 
	public float opaque_distance;

	// Scene Camera
	private GameObject cam;

	// Distance at which this Node is fully opaque
	private float opaqueClamp;

	private GameObject infoController;
	private Renderer rend;

	void Start ()
	{
		cam = GameObject.Find ("Main Camera");
		rend = GetComponent<Renderer>();

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
		Color color = new Color (rend.material.color.r, rend.material.color.b, rend.material.color.g, opacity);
		rend.material.color = color;
	}


}
