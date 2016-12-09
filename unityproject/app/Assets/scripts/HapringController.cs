﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HapringController : MonoBehaviour {
    public Color highlightColor = Color.red;
    private List<GameObject> nodes;
    private int currentIndex = -1;
    void Start () {
	
	}

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            switchNode(Direction.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            switchNode(Direction.down);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            switchNode(Direction.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            switchNode(Direction.right);
        }
    }
        
    void switchNode(Direction direction)
    {
        if (direction == Direction.right)
        {
            nodes = HighlightNode.GetAllHighlighted();
            if (currentIndex == -1 && nodes.Count > 0)
            {
                Debug.Log("first assigned");
                currentIndex = nodes[0].GetComponent<Node>().id;
            }
            int index = nodes.FindIndex(c => c.GetComponent<Node>().id == currentIndex);
            Debug.Log("lastIndex =" + index);
            if (nodes.Count > 0 && index + 2 > nodes.Count)
            {
                Debug.Log("nodesCount was smaller than 2+index");
                foreach (GameObject g in nodes)
                {
                    g.GetComponent<Renderer>().material.color = HighlightNode.highlightColor;
                }
                currentIndex = nodes[0].GetComponent<Node>().id;
                nodes[0].GetComponent<Renderer>().material.color = highlightColor;
            }
            else if (nodes.Count > 0)
            {
                Debug.Log((2 + index) + " was not bigger than" + nodes.Count);
                foreach (GameObject g in nodes)
                {
                    g.GetComponent<Renderer>().material.color = HighlightNode.highlightColor;
                }
                currentIndex = nodes[index + 1].GetComponent<Node>().id;
                nodes[index + 1].GetComponent<Renderer>().material.color = highlightColor;

            }
        }
        else if(direction == Direction.left)
        {
            nodes = HighlightNode.GetAllHighlighted();
            if (currentIndex == -1 && nodes.Count > 0)
            {
                Debug.Log("first assigned");
                currentIndex = nodes[0].GetComponent<Node>().id;
            }
            int index = nodes.FindIndex(c => c.GetComponent<Node>().id == currentIndex);
            Debug.Log("lastIndex =" + index);
            if (nodes.Count > 0 && (index - 1) >= 0)
            {
                foreach (GameObject g in nodes)
                {
                    g.GetComponent<Renderer>().material.color = HighlightNode.highlightColor;
                }
                currentIndex = nodes[index - 1].GetComponent<Node>().id;
                nodes[index - 1].GetComponent<Renderer>().material.color = highlightColor;
            }
            else if (nodes.Count > 0)
            {
                foreach (GameObject g in nodes)
                {
                    g.GetComponent<Renderer>().material.color = HighlightNode.highlightColor;
                }
                currentIndex = nodes[nodes.Count-1].GetComponent<Node>().id;
                nodes[nodes.Count-1].GetComponent<Renderer>().material.color = highlightColor;

            }
        }
    }
    enum Direction { up, down, left, right};

}