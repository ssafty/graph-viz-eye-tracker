using UnityEngine;
using System.Collections;
using System;

public class TrialState : ExperimentState {

	[SerializeField]
	GameObject marker;
	[SerializeField]
	GameObject eyepointer;

    public override ExperimentState HandleInput(ExperimentController ec)
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ec.CurrentTrialIndex++;
        }
        if(ec.CurrentTrialIndex>= ec.CurrentTrials.Count)
        {
            return nextState;
        }
        else
        {
            return this;
        }
    }

    public override void UpdateState(ExperimentController ec)
    {
		marker.gameObject.SetActive(true);
		eyepointer.gameObject.SetActive(true);
        MoveToExperimentTrial mttrial = ec.CurrentTrials[ec.CurrentTrialIndex] as MoveToExperimentTrial;
        if(mttrial != null)
        {
            Vector3 pos = mttrial.TargetPosition;
            Vector3 scale = new Vector3().FromFloat(0.1f);
            Debug.Log("Experiment in progess" + ec.CurrentTrialIndex);


        }
        else
        {
            throw new UnityException("couldn't cast trial as MoveToExperimentTrial");
        }
    }
}
