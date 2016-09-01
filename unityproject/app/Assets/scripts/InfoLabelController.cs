using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfoLabelController : MonoBehaviour
{

	private GameObject header;
	private GameObject desc;
	public KeyCode dismissKey;



	// Update is called once per frame
	void Update ()
	{
		Node node = MouseClickUtil.checkMouseClick (0, 10000);
		if (node != null) {

			setTitle (node.title);
			setDescription (node.desc);
			toggle (true);
		}

		if (Input.GetKeyDown (dismissKey)) {
			toggle (false);
			setTitle ("");
			setDescription ("");
		}
	}



	private void setTitle (string title)
	{
		transform.Find ("Header").GetComponent<Text> ().text = title;
	}

	private void setDescription (string description)
	{
		transform.Find ("Text").GetComponent<Text> ().text = description;
	}

	private void toggle (bool visible)
	{
		transform.Find ("Header").GetComponent<Text> ().enabled = visible;
		transform.Find ("Text").GetComponent<Text> ().enabled = visible;
		gameObject.GetComponent<Image> ().enabled = visible;

	}







}
