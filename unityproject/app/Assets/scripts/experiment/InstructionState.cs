using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class InstructionState : ExperimentState
{
	[SerializeField]
	Canvas c;
	[SerializeField]
	Text text;
	[SerializeField]
	GameObject marker;
	[SerializeField]
	GameObject eyepointer;
	[SerializeField]
	GameObject graph;
	[SerializeField]
	GameObject panel;
	[SerializeField]
	KeyCode skip = KeyCode.End;

	bool next = false;

	public bool Next {
		get {
			return next;
		}

		set {
			next = value;
		}
	}

	public override ExperimentState HandleInput (ExperimentController ec)
	{
		if (Next || Input.GetKeyDown (skip)) {
			Debug.Log ("Moving on to training phase");
			Next = false;
			panel.gameObject.SetActive (false);
			c.gameObject.SetActive (false);
			return nextState;
		} else {
			return this;
		}        
	}

	public override void UpdateState (ExperimentController ec)
	{
		experimentLogger.getLogger ().currentState = "Instruction";
		c.gameObject.SetActive (true);
		graph.gameObject.SetActive (false);
		//eyepointer.gameObject.SetActive(false);
		panel.gameObject.SetActive (true);
		MoveToExperimentTrial mttrial = ec.CurrentTrials [ec.CurrentTrialIndex] as MoveToExperimentTrial;
		if (mttrial.Graph.ExperimentType == experimentType.EYE || mttrial.Graph.ExperimentType == experimentType.WITHCUSTOMCALIB) {
			text.text = "You will see a graph, \n please try to select a cluster of nodes with your eyes \n and use the ring on your finger to switch between the nodes. \n The first one is just for testing if everything works. \n The ones after that are the ones that matter. \n But first we need to do eyetracker calibration.";
		} else if (mttrial.Graph.ExperimentType == experimentType.MOUSE) {
			text.text = "After pressing the button you will see a graph. \n Please use kouse and keyboard to navigate through the graph and select the red node. \n The first one is just for testing if everything works.  \n The ones after that are the ones that matter.";
		}

	}
}
