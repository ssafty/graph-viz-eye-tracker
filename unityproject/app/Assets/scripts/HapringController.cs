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
                currentIndex = nodes[0].GetComponent<Node>().id;
            }
           int index = nodes.FindIndex(c => c.GetComponent<Node>().id == currentIndex);
            Debug.Log("lastIndex =" + index);
           if (index + 2 > nodes.Count)
           {
                foreach (GameObject g in nodes)
                {
                    g.GetComponent<Renderer>().material.color = HighlightNode.highlightColor;
                }
                nodes[0].GetComponent<Renderer>().material.color = highlightColor;
           }
           else
           {
                foreach (GameObject g in nodes)
                {
                    g.GetComponent<Renderer>().material.color = HighlightNode.highlightColor;
                }
                nodes[++index].GetComponent<Renderer>().material.color = highlightColor;
    
           }
        }
	}

}
