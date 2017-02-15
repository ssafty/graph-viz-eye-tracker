using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MoveToExperimentController : ExperimentController
{

	[SerializeField]
	Vector3 targetPosition;
	[SerializeField]
	int numberOfTrialsForEveryGraph = 5;
	List<Graph> graphList;

	// Use this for initialization
	void Start ()
	{
		FillTrials ();
	}

	// Update is called once per frame
	protected override void Update ()
	{
		base.Update ();

	}

	protected override void FillTrials ()
	{
		Debug.LogWarning ("FillTrials()");
		currentTrials = new List<ExperimentTrial> ();
		graphList = new List<Graph> ();
		graphList.Add (new Graph ("Tree_50", 50, numberOfTrialsForEveryGraph, 5.0f, experimentType.MOUSE));
		graphList.Add (new Graph ("Tree_50", 50, numberOfTrialsForEveryGraph, 10.0f, experimentType.MOUSE));
		graphList.Add (new Graph ("Tree_150", 50, numberOfTrialsForEveryGraph, 10.0f, experimentType.MOUSE));

		graphList = ShuffleList<Graph> (graphList);

		int k = 0;
		for (int i = 0; i < (graphList.Count); i++) {
			k++;
			CurrentTrials.Add (new MoveToExperimentTrial (k, graphList [i]));
		}
	}

	//http://www.vcskicks.com/randomize_array.php
	private List<E> ShuffleList<E> (List<E> inputList)
	{
		List<E> randomList = new List<E> ();

		System.Random r = new System.Random ();
		int randomIndex = 0;
		while (inputList.Count > 0) {
			randomIndex = r.Next (0, inputList.Count); //Choose a random object in the list
			randomList.Add (inputList [randomIndex]); //add it to the new, random list
			inputList.RemoveAt (randomIndex); //remove to avoid duplicates
		}

		return randomList; //return the new random list
	}
}

public enum experimentType
{
	MOUSE,
	EYE,
	HMD
}

public class Graph
{
	private string _name;
	private int _numNodes;
	private int _numberHighlightedNodes;
	private float _bubbleSize;
	private experimentType _experimentType;

	public Graph (string name, int nodes, int numberOfHighlightedNodes, float bubblesize, experimentType type)
	{
		Debug.LogWarning ("Build Graph");
		_numNodes = nodes;
		_name = name;
		_numberHighlightedNodes = numberOfHighlightedNodes;
		_bubbleSize = bubblesize;
		_experimentType = type;
	}

	public experimentType ExperimentType {
		get {
			return _experimentType;
		}
	}

	public string Name {
		get {
			return _name;
		}
	}

	public float BubbleSize {
		get {
			return _bubbleSize;
		}
	}

	public int NumberHighlightedNodes {
		get {
			return _numberHighlightedNodes;
		}
	}

	public int NumNodes {
		get {
			return _numNodes;
		}
	}
}


