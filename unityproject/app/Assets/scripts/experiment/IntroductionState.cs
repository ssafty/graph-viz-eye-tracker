using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class IntroductionState : ExperimentState
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
		if (Next) {
			panel.gameObject.SetActive (false);
			c.gameObject.SetActive (false);
			return nextState;
		} else {
			return this;
		}        
	}

	public override void UpdateState (ExperimentController ec)
	{
		c.gameObject.SetActive (true);
		marker.gameObject.SetActive (false);
		graph.gameObject.SetActive (false);
		eyepointer.gameObject.SetActive (false);
		panel.gameObject.SetActive (true);
		text.text = "Hi and welcome to this experiment!";
		Debug.Log ("Hi im now in the Introductionstate");

	}
}
