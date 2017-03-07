using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class PupilCalibrationState : ExperimentState
{

    
	bool next = false;
	[SerializeField]
	Text text;
	[SerializeField]
	GameObject marker;
	[SerializeField]
	GameObject eyepointer;
	[SerializeField]
	GameObject c;
	[SerializeField]
	GameObject panel;
	[SerializeField]
	KeyCode skip = KeyCode.End;

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
			ec.drawGraph = true;
			return nextState;
		} else {
			return this;
		}        
	}

	public override void UpdateState (ExperimentController ec)
	{
		experimentLogger.getLogger ().currentState = "PupilCalibration";
		panel.gameObject.SetActive (true);
		//eyepointer.gameObject.SetActive(false);
		text.text = "The first phase of the calibration will start in the next seconds";
		c.gameObject.SetActive (true);
		
	}
}
