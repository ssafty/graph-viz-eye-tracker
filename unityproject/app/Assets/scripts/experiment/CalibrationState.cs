using UnityEngine;
using System.Collections;
using System;

public class CalibrationState : ExperimentState
{

    
	bool next = false;
	[SerializeField]
	GameObject marker;
	[SerializeField]
	GameObject eyepointer;

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
			marker.gameObject.SetActive(true);
			eyepointer.gameObject.SetActive(true);

			return nextState;
		}
		else
		{
			return this;
		}        
	}

    public override void UpdateState(ExperimentController ec)
    {
        MoveToExperimentTrial mttrial = ec.CurrentTrials[ec.CurrentTrialIndex] as MoveToExperimentTrial;
        if (mttrial != null)
        {
            Vector3 pos = mttrial.TargetPosition;
            Vector3 scale = new Vector3().FromFloat(0.1f);
            Debug.Log("Calibration in progess" + ec.CurrentTrialIndex);


        }
        else
        {
            throw new UnityException("couldn't cast trial as MoveToExperimentTrial");
        }
    }
}
