using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MoveToExperimentController : ExperimentController {

    [SerializeField]
    Vector3 targetPosition;

    // Use this for initialization
    void Start () {
        FillTrials();        
	}

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

    }

    protected override void FillTrials()
    {
        currentTrials = new List<ExperimentTrial>();
        for(int i  = 0; i < 10; i++)
        {
            CurrentTrials.Add(new MoveToExperimentTrial(i, targetPosition + new Vector3(UnityEngine.Random.value, UnityEngine.Random.value)));
        }        
    }
}

