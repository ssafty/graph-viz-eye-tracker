using UnityEngine;
using System.Collections;

public class MoveToExperimentTrial : ExperimentTrial
{
    private Graph _graph;
    bool first = true;
    public Graph Graph
    {
        get
        {
            return _graph;
        }
    }
    public void initialze()
    {
        if (first)
        {
            first = false;
            createGraph();
        }
    }
    private void createGraph()
    {
        GameController gc = GameObject.Find("GameController").GetComponent<GameController>();
        gc.createGraph(_graph.Name);
    }
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="_targetPosition"></param>
    public MoveToExperimentTrial(int trialnum, Graph graph) : base(trialnum)
    {
        _graph = graph;
    }
}
