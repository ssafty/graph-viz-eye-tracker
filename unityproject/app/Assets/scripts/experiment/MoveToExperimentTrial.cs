﻿using UnityEngine;
using System.Collections;

public class MoveToExperimentTrial : ExperimentTrial
{
    private Graph _graph;
    bool first = true;
    bool currentlyHighlighted = false;
    int hightlightCounter = 0;
    bool graphCreated = false;
    public static Color colorDesAuserwaehlten = new Color(1.0f, 0.0f, 0.0f);
    public static Color colorDesGefundenenAuserwaehlten = new Color(1.0f, 1.0f, 0.0f);
    Node selectedNode;
    public bool done = false;
    
    public Graph Graph
    {
        get
        {
            return _graph;
        }
    }
    public void initialze(GameObject gameController)
    {
        if (first)
        {
            if (Graph.ExperimentType == experimentType.EYE)
            {
                gameController.GetComponent<Bubble>().rayCastAllowed = true;
            }
            else if (Graph.ExperimentType == experimentType.MOUSE)
            {
                gameController.GetComponent<Bubble>().rayCastAllowed = false;
            }
            Bubble.changeBubbleSize(_graph.BubbleSize);
            first = false;
            createGraph();
            experimentLogger.getLogger().currentGraph = _graph.Name;
            experimentLogger.getLogger().bubbleSize = _graph.BubbleSize.ToString();
            graphCreated = true;
            Vector3 pos = Node.GetNodeWithId(0).transform.position;
            GameObject.FindGameObjectWithTag("metaCamera").transform.position = new Vector3(pos.x, pos.y, pos.z -20);
            GameObject.FindGameObjectWithTag("metaCamera").transform.LookAt(pos);

        }
       
    }

    public void update()
    {
        //set first red node
        if (!currentlyHighlighted && graphCreated)
        {

            if(hightlightCounter < _graph.NumberHighlightedNodes) { 
                currentlyHighlighted = true;
                highlight();
            }
        }
        else if(currentlyHighlighted && selectedNode.gotHit)
        {
            Debug.Log("Auserwählter = false");
                selectedNode.derAuserwaehlte = false;
                Bubble.moveTo(Bubble.REST_POS);
                selectedNode = null;
                hightlightCounter++;

            if (hightlightCounter < _graph.NumberHighlightedNodes)
            {
                highlight();
            }

            else
            {
                done = true;
            }
            
        }
        if(selectedNode != null)
        {
            if (selectedNode.GetComponent<Node>().id != GameObject.FindGameObjectWithTag("GameController").GetComponent<HapringController>().currentIndex) {
                selectedNode.Highlight(colorDesAuserwaehlten);
            } else
            {
                selectedNode.Highlight(colorDesGefundenenAuserwaehlten);
            }
            }
        
    }
    private void highlight()
    {
        Debug.Log("highlight new Auserwählten");
        Node node = Node.GetNodeWithId(Random.Range(0, Graph.NumNodes));
        node.Highlight(colorDesAuserwaehlten);
        selectedNode = node;
        node.derAuserwaehlte = true;
        experimentLogger.getLogger().currentHighlightedNode = node.id_string;
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
