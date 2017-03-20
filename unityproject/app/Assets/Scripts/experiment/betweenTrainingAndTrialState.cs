using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class betweenTrainingAndTrialState : ExperimentState
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
			panel.gameObject.SetActive (false);
			c.gameObject.SetActive (false);
			Next = false;
			return nextState;
		} else {
			return this;
		}        
	}

	public override void UpdateState (ExperimentController ec)
	{
		experimentLogger.getLogger ().currentState = "betweenTrainingAndTrialState";
		c.gameObject.SetActive (true);
		graph.gameObject.SetActive (false);
		//eyepointer.gameObject.SetActive(false);
		panel.gameObject.SetActive (true);
		text.text = "Training is done. Now its going um the wurst!";
		//Debug.Log ("Hi im now in the Introductionstate");

	}
}
