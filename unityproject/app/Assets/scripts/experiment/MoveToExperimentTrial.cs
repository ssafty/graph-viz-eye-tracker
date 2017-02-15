using UnityEngine;
using System.Collections;

public class MoveToExperimentTrial : ExperimentTrial
{
	private Graph _graph;
	bool first = true;
	int hightlightCounter = 0;
	bool graphCreated = false;
	public static Color colorDesAuserwaehlten = new Color (1.0f, 0.0f, 0.0f);
	public static Color colorDesGefundenenAuserwaehlten = new Color (1.0f, 1.0f, 0.0f);
	Node nodeToFind;
	public bool done = false;

	public Graph Graph {
		get {
			return _graph;
		}
	}

	public void initialze (GameObject gameController)
	{
		
		if (first) {
			if (Graph.ExperimentType == experimentType.EYE) {
				gameController.GetComponent<Bubble> ().rayCastAllowed = true;
			} else if (Graph.ExperimentType == experimentType.MOUSE) {
				gameController.GetComponent<Bubble> ().rayCastAllowed = false;
			}
			Bubble.changeBubbleSize (_graph.BubbleSize);
			first = false;
			createGraph ();
			highlight ();
			graphCreated = true;

			Vector3 pos = Node.GetNodeWithId (0).transform.position;
			Camera.main.transform.position = new Vector3 (pos.x, pos.y, pos.z - 20);
			Camera.main.transform.LookAt (pos);

			experimentLogger.getLogger ().currentGraph = _graph.Name;
			experimentLogger.getLogger ().bubbleSize = _graph.BubbleSize.ToString ();

		}
       
	}

	private void resetColor ()
	{
		if (nodeToFind.GetComponent<Node> ().id != GameObject.FindGameObjectWithTag ("GameController").GetComponent<HapringController> ().currentIndex) {
			nodeToFind.Highlight (colorDesAuserwaehlten);
		} else {
			nodeToFind.Highlight (colorDesGefundenenAuserwaehlten);
		}
	}

	public void update ()
	{
		if (!done) {
			if (hightlightCounter >= _graph.NumberHighlightedNodes) {
				done = true;
			} else {
				if (nodeToFind.gotHit) {
					Bubble.moveTo (Bubble.REST_POS);
					Debug.LogWarning ("got hit");
					hightlightCounter++;
					nodeToFind.derAuserwaehlte = false;
					nodeToFind.gotHit = false;
					nodeToFind.HighlightDefault ();
					highlight ();

				}
				resetColor ();
			}
		}      
	}

	private void highlight ()
	{
		Node node = Node.GetNodeWithId (Random.Range (0, Graph.NumNodes));
		node.Highlight (colorDesAuserwaehlten);
		node.derAuserwaehlte = true;
		nodeToFind = node;
		experimentLogger.getLogger ().currentHighlightedNode = node.id_string;
	}

	private void createGraph ()
	{
		
		GameController gc = GameObject.Find ("GameController").GetComponent<GameController> ();
		gc.createGraph (_graph.Name);
	}

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="_targetPosition"></param>
	public MoveToExperimentTrial (int trialnum, Graph graph) : base (trialnum)
	{
		_graph = graph;
	}
}
