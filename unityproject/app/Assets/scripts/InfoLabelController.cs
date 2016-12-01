using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SimpleJSON;
using System;
using UnityEngine.Networking.NetworkSystem;

public class InfoLabelController : MonoBehaviour
{

	private GameObject header;
	private GameObject desc;
	public KeyCode dismissKey;
	public Canvas canvas;


	void Start ()
	{
		Vector3 old = transform.position;
		GameObject marker = GameObject.FindGameObjectWithTag ("marker");
		RectTransform trans = marker.GetComponent<RectTransform> ();

		int offsetX = (int)(4.5 * Math.Abs (trans.rect.x * canvas.scaleFactor));
		int offsetY = (int)(offsetX * 0.5);

		transform.position = new Vector3 (old.x - offsetX, old.y - offsetY, old.z);
	}


	// Update is called once per frame
	void Update ()
	{
//		GameObject node = MouseClickUtil.checkMouseClick (0, 10000);
//		if (node != null && node is Node) {
//			Node n = node as Node;
//			setTitle (n.title);
//			if (n.desc == "Loading ...")
//				StartCoroutine (parseDescription (n));
//			setDescription (n.desc);
//			toggle (true);
//		}
//
//		if (Input.GetKeyDown (dismissKey)) {
//			toggle (false);
//			setTitle ("");
//			setDescription ("");
//		}
	}


	private IEnumerator parseDescription (Node node)
	{
		string url = "https://en.wikipedia.org/w/api.php?format=json&action=query&prop=extracts&exintro=&explaintext=&titles=" + node.title.Replace (" ", "%20");
		WWW www = new WWW (url);
		yield return www;

		JSONNode N = JSON.Parse (www.text);
		string desc = N ["query"] ["pages"] [0] ["extract"];

		//set and call render again
		node.desc = desc;
		setDescription (node.desc);
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
