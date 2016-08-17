using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;

public class InfoLabelController : MonoBehaviour
{
	public static float WIDTH = 200f;
	public static float HEIGHT = 300f;
	private GameObject header;
	private GameObject desc;
	private GameObject node;


	// Update is called once per frame
	void Update ()
	{
		if (node != null) {
			Vector3 pos = node.transform.position;
			float newX = pos.x + InfoLabelController.WIDTH / 2;
			float newY = pos.y - 20 - InfoLabelController.HEIGHT / 2;
			gameObject.transform.position = new Vector3 (newX, newY, pos.z);
		}
	}

	public void setGraphNode (GameObject node)
	{
		this.node = node;
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
		gameObject.SetActive (false);
	}

	public void show ()
	{
		gameObject.SetActive (true);
	}



}
