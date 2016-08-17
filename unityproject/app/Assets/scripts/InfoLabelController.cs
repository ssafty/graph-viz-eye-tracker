using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;

public class InfoLabelController : MonoBehaviour
{

	private GameObject header;
	private GameObject desc;


	// Update is called once per frame
	void Update ()
	{
	
	}

	public void setTitle (string title)
	{
		transform.Find ("Header").GetComponent<Text> ().text = title;
	}

	public void setDescription (string description)
	{
		transform.Find ("Text/Viewport/Content").GetComponent<Text> ().text = description;
	}

	public void hide ()
	{
		
	}

	public void show ()
	{
		
	}



}
