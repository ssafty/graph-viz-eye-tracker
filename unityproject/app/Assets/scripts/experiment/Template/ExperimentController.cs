using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public abstract class ExperimentController
    : Singleton<ExperimentController>
{


	[SerializeField]
	protected ExperimentState currentState;

	public int currentTrialIndex;
	public bool drawGraph = false;
	private int numberOfTrainings;

	protected StreamWriter outputStream;

	protected List<ExperimentTrial> currentTrials;

	public List<ExperimentTrial> CurrentTrials {
		get {
			return currentTrials;
		}
	}

	public int CurrentTrialIndex {
		get {
			return currentTrialIndex;
		}

		set {
			currentTrialIndex = value;
		}
	}

	public int NumberOfTrainings {
		get {
			return numberOfTrainings;
		}
	}

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	protected virtual void Update ()
	{
		currentState = currentState.HandleInput (this);
		currentState.UpdateState (this);
	}

	protected abstract void FillTrials ();
    
}
