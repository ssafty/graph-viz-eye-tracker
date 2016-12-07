using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HapringController : MonoBehaviour {
    public Color highlightColor = Color.red;
    private List<GameObject> nodes;
    private int currentIndex = -1;
    void Start () {
	
	}

    void Update () {

        if (Input.GetKeyDown(KeyCode.Tab)) {
            nodes = HighlightNode.GetAllHighlighted();
            if(currentIndex == -1 && nodes.Count > 0)
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
           else if(nodes.Count > 0)
           {
                Debug.Log((2+index) +" was not bigger than"+nodes.Count);
                foreach (GameObject g in nodes)
                {
                    g.GetComponent<Renderer>().material.color = HighlightNode.highlightColor;
                }
                currentIndex = nodes[index + 1].GetComponent<Node>().id;
                nodes[index+1].GetComponent<Renderer>().material.color = highlightColor;
    
           }
        }
	}

}
