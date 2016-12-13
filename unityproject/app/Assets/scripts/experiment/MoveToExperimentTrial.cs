using UnityEngine;
using System.Collections;

public class MoveToExperimentTrial : ExperimentTrial
{
    private Graph _graph;
    bool first = true;
    bool currentlyHighlighted = false;
    bool graphCreated = false;
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
            graphCreated = true;
        }
    }

    public void update()
    {
        if(!currentlyHighlighted && graphCreated)
        {
            currentlyHighlighted = true;
            highlight();
        }
        
    }
    private void highlight()
    {
        Node.GetNodeWithId(Random.Range(0, Graph.NumNodes)).Highlight(new Color(1.0f, 0.0f, 0.0f));
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
