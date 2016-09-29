using UnityEngine;
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
	private Hashtable nodesByID;
	private Hashtable edges;

	public int nodeCount;
	public int edgeCount = 0;
	private GameObject graphParent;

	public bool[,] adjacenyList;

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

		//find count and most connected element
		int max_count = find_count_and_highest_connection (root, out nodeCount);

		//initialize adjacenyList with the nodeCount
		adjacenyList = new bool[nodeCount, nodeCount];

		//Building nodes and edges
		for (int i = 0; i < root.ChildNodes.Count; i++) {
			XmlElement xmlGraph = root.ChildNodes [i] as XmlElement;

			int node_id = 0;
			for (int j = 0; j < xmlGraph.ChildNodes.Count; j++) {
				XmlElement xmlNode = xmlGraph.ChildNodes [j] as XmlElement;

				//create nodes
				if (xmlNode.Name == "node") {
					float x = float.Parse (xmlNode.Attributes ["x"].Value) / SCALE;
					float y = float.Parse (xmlNode.Attributes ["y"].Value) / SCALE;
					float z = float.Parse (xmlNode.Attributes ["z"].Value) / SCALE;

					Node nodeObject = Instantiate (nodePrefab, new Vector3 (x, y, z), Quaternion.identity) as Node;
					nodeObject.transform.parent = graphParent.transform;
					nodeObject.nodeText.text = WWW.UnEscapeURL (xmlNode.Attributes ["id"].Value.Replace ("_", " ")); //decode URL -> name
					nodeObject.nodeText.fontSize = 250;
					nodeObject.nodeText.transform.localScale = new Vector3 (0.018f, 0.018f, 0.018f);
					nodeObject.id = node_id;
					nodeObject.id_string = xmlNode.Attributes ["id"].Value;
					nodes.Add (nodeObject.id_string, nodeObject);
					nodesByID.Add (nodeObject.id, nodeObject);
					nodeObject.title = nodeObject.nodeText.text;
					nodeObject.desc = "Loading ...";
					float count = float.Parse (xmlNode.Attributes ["count"].Value);
					//nodeObject.scale_size = 500/count; //Scale with count
					nodeObject.scale_size = 1f;
					nodeObject.transform.localScale = new Vector3 (nodeObject.scale_size, nodeObject.scale_size, nodeObject.scale_size);

					//increment id for the next node
					node_id ++;
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
		// Map edges to nodes
		// and fill in the adjacency list
		foreach (string key in edges.Keys) {
			Edge link = edges [key] as Edge;
			link.source = nodes [link.sourceId] as Node;
			link.target = nodes [link.targetId] as Node;

			//both ways
			adjacenyList [link.source.id ,link.target.id] = true;
			adjacenyList [link.target.id ,link.source.id] = true;
		}
	}

	void Start ()
	{
		nodes = new Hashtable ();
		nodesByID = new Hashtable ();
		edges = new Hashtable ();

		graphParent = GameObject.FindGameObjectWithTag ("GraphParent");
		StartCoroutine (loadLayout ());
	}


	private int find_count_and_highest_connection (XmlElement root, out int nodeCount)
	{
		nodeCount = 0;
		int max = 0;
		for (int i = 0; i < root.ChildNodes.Count; i++) {
			XmlElement xmlGraph = root.ChildNodes [i] as XmlElement;

			for (int j = 0; j < xmlGraph.ChildNodes.Count; j++) {
				XmlElement xmlNode = xmlGraph.ChildNodes [j] as XmlElement;

				//create nodes
				if (xmlNode.Name == "node") {
					nodeCount++;
					int count = int.Parse (xmlNode.Attributes ["count"].Value);
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
		
	public void HighlightNodes(int NodeId, bool Highlight){
		Node main = nodesByID [NodeId] as Node;
		if (Highlight)
			main.HighlightAsMain (); 
		else 
			main.HighlightDefault ();

		for (int i = 0; i < adjacenyList.GetLength(0); i++) {
			if (adjacenyList [NodeId, i] && NodeId != i) {
				//isNeighbor, but we only have int ID and hashmap uses String as hashValue (so it can build edges)
				// solution: use another HashMap to save IDs reference to Node objects (memory vs speed tradeoff)
				Node neighbor = nodesByID[i] as Node;
				if (Highlight)
					neighbor.HighlightAsNeighbor ();
				else
					neighbor.HighlightDefault ();
			}	
		}
	}
}
