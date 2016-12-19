﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MoveToExperimentController : ExperimentController {

    [SerializeField]
    Vector3 targetPosition;
    [SerializeField]
    int numberOfTrialsForEveryGraph = 1;
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
        graphList.Add(new Graph("Tree_50",50,1));
        graphList.Add(new Graph("Tree_150",150,1));
        //graphList.Add(new Graph("Tree_450",450,5));
        //graphList.Add(new Graph("200_nodes",200,5));
        //graphList.Add(new Graph("500_nodes",500,5));
        graphList = ShuffleList<Graph>(graphList);

        int k = 0;
        for (int i = 0; i < (graphList.Count); i++)
        {
            k++;
            CurrentTrials.Add(new MoveToExperimentTrial(k, graphList[i]));
        }
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
    private int _numNodes;
    private int _numberHighlightedNodes;
    public Graph(string name, int nodes, int numberOfHighlightedNodes)
    {
        _numNodes = nodes;
        _name = name;
        _numberHighlightedNodes = numberOfHighlightedNodes;
    }
    public string Name
    {
        get
        {
            return _name;
        }
    }
    public int NumberHighlightedNodes
    {
        get
        {
            return _numberHighlightedNodes;
        }
    }
    public int NumNodes
    {
        get
        {
            return _numNodes;
        }
    }
}


