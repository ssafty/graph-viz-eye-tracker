using UnityEngine;
using System.Collections;
using System;
using System.ComponentModel;
using System.Reflection;

public class Node : MonoBehaviour
{

	public string id;
	public int count;
	public TextMesh nodeText;
	public  string title = "EMTPY";
	public  string desc = "EMTPY";
	private GameObject infoController;
	public GameObject labelPrefab;


	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{ 
		Vector3 targetPosition = Camera.main.transform.position;
		Vector3 currentPosition = transform.position;
		targetPosition.y = 0;
		currentPosition.y = 0;
		nodeText.transform.rotation = Quaternion.LookRotation (currentPosition - targetPosition, Vector3.up);
	}

	public void SetupLabel ()
	{
		Vector3 pos = transform.position;
		float newX = pos.x + InfoLabelController.WIDTH / 2;
		float newY = pos.y - 20 - InfoLabelController.HEIGHT / 2;
		pos = new Vector3 (newX, newY, pos.z);
		infoController = Instantiate (labelPrefab, pos, Quaternion.identity) as GameObject;
		InfoLabelController ctrl =	infoController.GetComponent<InfoLabelController> ();
		ctrl.setGraphNode (gameObject);
		ctrl.hide ();
		ctrl.setTitle (title);
		ctrl.setDescription (desc); 
		infoController.transform.SetParent (GameObject.Find ("Canvas").transform);
	}
}
