using UnityEngine;
using System.Collections;
using System;

public class TrainingState : ExperimentState
{

    [SerializeField]
    GameObject marker;
    [SerializeField]
    GameObject eyepointer;
    [SerializeField]
    GameObject graph;
    [SerializeField]
    GameObject GameController;

    public override ExperimentState HandleInput(ExperimentController ec)
    {
        if (ec.CurrentTrialIndex > ec.NumberOfTrainings)
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
        experimentLogger.getLogger().currentState = "Training";
        marker.gameObject.SetActive(true);
        eyepointer.gameObject.SetActive(true);
        graph.gameObject.SetActive(true);
        MoveToExperimentTrial mttrial = ec.CurrentTrials[ec.CurrentTrialIndex] as MoveToExperimentTrial;

        if (mttrial != null)
        {

            // Debug.Log("Experiment in progess" + ec.CurrentTrialIndex);
            mttrial.initialze(GameController);

            mttrial.update();
            if (mttrial.done)
            {
                GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");
                foreach (GameObject node in nodes)
                {
                    GameObject.Destroy(node);
                }
                ec.CurrentTrialIndex++;
            }
        }
        else
        {
            throw new UnityException("couldn't cast trial as MoveToExperimentTrial");
        }
    }
}
