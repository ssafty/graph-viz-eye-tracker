using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class CalibrationState : ExperimentState
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
	public bool Next
	{
		get
		{
			return next;
		}

		set
		{
			next = value;
		}
	}

	public override ExperimentState HandleInput(ExperimentController ec)
	{
		if(Next)
		{
			panel.gameObject.SetActive(false);
			c.gameObject.SetActive(false);
			return nextState;
		}
		else
		{
			return this;
		}        
	}

    public override void UpdateState(ExperimentController ec)
    {
    	panel.gameObject.SetActive(true);
    	marker.gameObject.SetActive(true);
		eyepointer.gameObject.SetActive(false);
		c.gameObject.SetActive(true);
		text.text = "Please look in the middle of the screen.";
    }
}
