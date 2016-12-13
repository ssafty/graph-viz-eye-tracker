using UnityEngine;
using System.Collections;

public class MoveToExperimentTrial : ExperimentTrial
{
    private Graph _graph;
    bool first = true;
    bool currentlyHighlighted = false;
    bool graphCreated = false;
    public static Color colorDesAuserwaehlten = new Color(1.0f, 0.0f, 0.0f);
    Node selectedNode;
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
        else if(currentlyHighlighted && selectedNode.gotHit)
        {
            selectedNode.derAuserwaehlte = false;
            Bubble.moveTo(Bubble.REST_POS);
            selectedNode = null;
            highlight();
            
        }
        if(selectedNode != null)
        {
            selectedNode.Highlight(colorDesAuserwaehlten);
        }
        
    }
    private void highlight()
    {
        Node node = Node.GetNodeWithId(Random.Range(0, Graph.NumNodes));
        node.Highlight(colorDesAuserwaehlten);
        selectedNode = node;
        node.derAuserwaehlte = true;
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
