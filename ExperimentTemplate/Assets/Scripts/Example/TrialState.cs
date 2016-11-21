using UnityEngine;
using System.Collections;
using System;

public class TrialState : ExperimentState {

    [SerializeField]
    Transform finger;
    [SerializeField]
    Mesh m;
    [SerializeField]
    Material outside;
    [SerializeField]
    Material inside;


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
        MoveToExperimentTrial mttrial = ec.CurrentTrials[ec.CurrentTrialIndex] as MoveToExperimentTrial;
        if(mttrial != null)
        {
            Vector3 pos = mttrial.TargetPosition;
            Vector3 scale = new Vector3().FromFloat(0.1f);

            if(Vector3.Distance(finger.position,pos)< 0.1f)
            {
                Graphics.DrawMesh(m, Matrix4x4.TRS(pos, Quaternion.identity, scale), inside, 0);
            }
            else
            {
                Graphics.DrawMesh(m, Matrix4x4.TRS(pos, Quaternion.identity, scale), outside, 0);
            }
            
        }
        else
        {
            throw new UnityException("couldn't cast trial as MoveToExperimentTrial");
        }
    }
}
