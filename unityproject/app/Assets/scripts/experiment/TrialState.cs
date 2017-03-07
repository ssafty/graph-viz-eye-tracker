using UnityEngine;
using System.Collections;
using System;

public class TrialState : ExperimentState
{

	[SerializeField]
	GameObject marker;
	[SerializeField]
	GameObject eyepointer;
	[SerializeField]
	GameObject graph;
	[SerializeField]
	GameObject GameController;
	[SerializeField]
	ExperimentState trainingState;
	private experimentType lastTrialType;
	private bool first = true;

	public override ExperimentState HandleInput (ExperimentController ec)
	{
		if (ec.CurrentTrials.Count > ec.CurrentTrialIndex) {
			MoveToExperimentTrial mttrial = ec.CurrentTrials [ec.CurrentTrialIndex] as MoveToExperimentTrial;
			if (mttrial.Graph.ExperimentType != lastTrialType && !first) {
				Debug.Log ("found new TrialType. Issuing a training phase");
				first = true;
				return trainingState;
			}
		}
		if (ec.CurrentTrialIndex >= ec.CurrentTrials.Count) {
			return nextState;
		} else {
			return this;
		}
	}

	public override void UpdateState (ExperimentController ec)
	{
		experimentLogger.getLogger ().currentState = "Trial";
		marker.gameObject.SetActive (true);
		eyepointer.gameObject.SetActive (true);
		graph.gameObject.SetActive (true);
		MoveToExperimentTrial mttrial = ec.CurrentTrials [ec.CurrentTrialIndex] as MoveToExperimentTrial;
        
		if (mttrial != null) {

			// Debug.Log("Experiment in progess" + ec.CurrentTrialIndex);
			mttrial.initialze (GameController);

			mttrial.update ();
			if (mttrial.done) {
				if (lastTrialType != mttrial.Graph.ExperimentType) {
					first = false;
				}
				lastTrialType = mttrial.Graph.ExperimentType;
				GameObject[] nodes = GameObject.FindGameObjectsWithTag ("Node");
				foreach (GameObject node in nodes) {
					GameObject.Destroy (node);
				}
				ec.CurrentTrialIndex++;
			}
		} else {
			throw new UnityException ("couldn't cast trial as MoveToExperimentTrial");
		}
	}
}
