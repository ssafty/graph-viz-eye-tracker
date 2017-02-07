using UnityEngine;
using System.Collections;
using System;

public class TrialState : ExperimentState {

	[SerializeField]
	GameObject marker;
	[SerializeField]
	GameObject eyepointer;
    [SerializeField]
    GameObject graph;
    public override ExperimentState HandleInput(ExperimentController ec)
    {
        if(ec.CurrentTrialIndex >= ec.CurrentTrials.Count)
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
        graph.gameObject.SetActive(true);
        MoveToExperimentTrial mttrial = ec.CurrentTrials[ec.CurrentTrialIndex] as MoveToExperimentTrial;
        if(mttrial != null)
        {
           // Debug.Log("Experiment in progess" + ec.CurrentTrialIndex);
            mttrial.initialze();
            mttrial.update();
            if(mttrial.done)
            {
                ec.CurrentTrialIndex++;
            }
        }
        else
        {
            throw new UnityException("couldn't cast trial as MoveToExperimentTrial");
        }
    }
}
