﻿using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Networking.Types;

public class GameController : MonoBehaviour
{
	public bool loadDataset = false;
	public string datasetName = "output_2";
	public float SCALE = 2;

	public Node nodePrefab;
	public Edge edgePrefab;

	private Hashtable nodes;
	private Hashtable edges;

	private GameObject graphParent;


	private IEnumerator loadLayout ()
	{
		string sourceFile;

		if (loadDataset) {
			sourceFile = datasetName;
		} else {
			sourceFile = "sample_output";
		}

		XmlDocument xmlDoc = new XmlDocument ();
		TextAsset textAsset = (TextAsset)Resources.Load (sourceFile, typeof(TextAsset));
		xmlDoc.LoadXml (textAsset.text);

		XmlElement root = xmlDoc.FirstChild as XmlElement;
		//find most connected element
		float max_count = find_highest_connection (root);

		//Building nodes and edges
		for (int i = 0; i < root.ChildNodes.Count; i++) {
			XmlElement xmlGraph = root.ChildNodes [i] as XmlElement;

			for (int j = 0; j < xmlGraph.ChildNodes.Count; j++) {
				XmlElement xmlNode = xmlGraph.ChildNodes [j] as XmlElement;

				//create nodes
				if (xmlNode.Name == "node") {
					float x = float.Parse (xmlNode.Attributes ["x"].Value) / SCALE;
					float y = float.Parse (xmlNode.Attributes ["y"].Value) / SCALE;
					float z = float.Parse (xmlNode.Attributes ["z"].Value) / SCALE;

					Node nodeObject = Instantiate (nodePrefab, new Vector3 (x, y, z), Quaternion.identity) as Node;
					nodeObject.transform.parent = graphParent.transform;
					nodeObject.nodeText.text = WWW.UnEscapeURL (xmlNode.Attributes ["id"].Value.Replace ("_", " "));
					nodeObject.nodeText.fontSize = 250;
					nodeObject.nodeText.transform.localScale = new Vector3 (0.018f, 0.018f, 0.018f);
					nodeObject.id = xmlNode.Attributes ["id"].Value;
					nodes.Add (nodeObject.id, nodeObject);
					nodeObject.title = nodeObject.nodeText.text;
					nodeObject.desc = nodeObject.nodeText.text + nodeObject.nodeText.text + nodeObject.nodeText.text;
					float count = float.Parse (xmlNode.Attributes ["count"].Value);
					float size = 1f;
					nodeObject.transform.localScale = new Vector3 (size, size, size);
				}

				//create edges
				if (xmlNode.Name == "edge") {
					Edge edgeObject = Instantiate (edgePrefab, new Vector3 (0, 0, 0), Quaternion.identity) as Edge;
					edgeObject.id = xmlNode.Attributes ["id"].Value;
					edgeObject.sourceId = xmlNode.Attributes ["source"].Value;
					edgeObject.targetId = xmlNode.Attributes ["target"].Value;
					edges.Add (edgeObject.sourceId + edgeObject.targetId, edgeObject);
					edgeObject.transform.parent = graphParent.transform;
				}

				//every 100 cycles return control to unity?? I guess to avoid fps lags
				if (j % 100 == 0)
					yield return true;
			}
		}
		//Map Link to Nodes
		foreach (string key in edges.Keys) {
			Edge link = edges [key] as Edge;
			link.source = nodes [link.sourceId] as Node;
			link.target = nodes [link.targetId] as Node;
		}
	}

	void Start ()
	{
		nodes = new Hashtable ();
		edges = new Hashtable ();

		graphParent = GameObject.FindGameObjectWithTag ("GraphParent");
		StartCoroutine (loadLayout ());
	}


	private float find_highest_connection (XmlElement root)
	{
		float max = 0;
		for (int i = 0; i < root.ChildNodes.Count; i++) {
			XmlElement xmlGraph = root.ChildNodes [i] as XmlElement;

			for (int j = 0; j < xmlGraph.ChildNodes.Count; j++) {
				XmlElement xmlNode = xmlGraph.ChildNodes [j] as XmlElement;

				//create nodes
				if (xmlNode.Name == "node") {
					float count = float.Parse (xmlNode.Attributes ["count"].Value);
					if (count > max)
						max = count;
				}
			}
		}
		return max;
	}


	void Update ()
	{
		if (Input.GetKey (KeyCode.LeftBracket)) {
			RaycastHit hitPoint;
			Ray ray = new Ray (Camera.main.transform.position, Camera.main.transform.forward);
			if (Physics.Raycast (ray, out hitPoint, 50)) { //used 50 instead of Mathf.Infinity
				Node l = (Node)hitPoint.transform.gameObject.GetComponent ("Node");
				if (l != null) {
					foreach (Node n in nodes.Values) {
						//n.transform.RotateAround (Camera.main.ViewportToWorldPoint (new Vector3 (0.5f, 0.5f, hitPoint.distance)), Vector3.up, 20 * Time.deltaTime);
						n.transform.RotateAround (l.transform.position, Vector3.up, 20 * Time.deltaTime);
					}
				}
			}
		}

		if (Input.GetKey (KeyCode.RightBracket)) {
			RaycastHit hitPoint;
			Ray ray = new Ray (Camera.main.transform.position, Camera.main.transform.forward);
			if (Physics.Raycast (ray, out hitPoint, 50)) {
				Node l = (Node)hitPoint.transform.gameObject.GetComponent ("Node");
				if (l != null) {
					foreach (Node n in nodes.Values) {
						//n.transform.RotateAround (Camera.main.ViewportToWorldPoint (new Vector3 (0.5f, 0.5f, hitPoint.distance)), -Vector3.up, 20 * Time.deltaTime);
						n.transform.RotateAround (l.transform.position, -Vector3.up, 20 * Time.deltaTime);
					}
				}
			}
		}

	}
}
