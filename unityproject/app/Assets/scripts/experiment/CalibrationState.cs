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
    [SerializeField]
    KeyCode skip = KeyCode.End;
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
		if(Next || Input.GetKeyDown(skip))
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
    	panel.gameObject.SetActive(false);
    	marker.gameObject.SetActive(false);
		eyepointer.gameObject.SetActive(false);
		c.gameObject.SetActive(true);
		
    }
}
