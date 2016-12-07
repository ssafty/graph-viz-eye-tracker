using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MoveToExperimentController : ExperimentController {

    [SerializeField]
    Vector3 targetPosition;
    [SerializeField]
    int numberOfTrialsForEveryGraph = 2;
    List<Graph> graphList;

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

        graphList = new List<Graph>();
        graphList.Add(new Graph("Tree_50"));
        graphList.Add(new Graph("Tree_150"));
        graphList.Add(new Graph("Tree_450"));
        graphList.Add(new Graph("200_nodes"));
        graphList.Add(new Graph("500_nodes"));
        graphList.Add(new Graph("1000_nodes"));
        ShuffleList<Graph>(graphList);

        int k = 0;
        for (int i = 0; i < (graphList.Count - 1); i++)
        {
            for (int j = 0; j < numberOfTrialsForEveryGraph; j++) {
                k++;
                CurrentTrials.Add(new MoveToExperimentTrial(k, graphList[i]));
            }
        }
        ShuffleList(currentTrials);     
    }

    //http://www.vcskicks.com/randomize_array.php
    private List<E> ShuffleList<E>(List<E> inputList)
    {
        List<E> randomList = new List<E>();

        System.Random r = new System.Random();
        int randomIndex = 0;
        while (inputList.Count > 0)
        {
            randomIndex = r.Next(0, inputList.Count); //Choose a random object in the list
            randomList.Add(inputList[randomIndex]); //add it to the new, random list
            inputList.RemoveAt(randomIndex); //remove to avoid duplicates
        }

        return randomList; //return the new random list
    }
}

public class Graph
{
    private string _name;
    public Graph(string name)
    {
        _name = name;
    }
    public string Name
    {
        get
        {
            return _name;
        }
    }
}


