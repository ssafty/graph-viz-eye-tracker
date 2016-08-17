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
	public  string title;
	public  string desc;

	void Start ()
	{
		title = "Node Titel";
		desc = "Description";
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
}
