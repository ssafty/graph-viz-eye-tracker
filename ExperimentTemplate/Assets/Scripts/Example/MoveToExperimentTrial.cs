using UnityEngine;
using System.Collections;

public class MoveToExperimentTrial : ExperimentTrial
{
    private Vector3 targetPosition;

    public Vector3 TargetPosition
    {
        get
        {
            return targetPosition;
        }
    }
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="_targetPosition"></param>
    public MoveToExperimentTrial(int trialnum, Vector3 _targetPosition) : base(trialnum)
    {
        targetPosition = _targetPosition;
    }
}
