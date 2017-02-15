using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HapringController : MonoBehaviour {
    private Color highlightColor = Color.magenta;
    private List<GameObject> nodes;
    private GameObject currentSelectNode;
    public int currentIndex = -1;
    void Start () {
	
	}
    bool isInBubble(int index)
    {
        List<GameObject> hightlightedNodes;
        hightlightedNodes = HighlightNode.GetAllHighlighted();
        foreach(GameObject g in hightlightedNodes)
        {
            if (g.GetComponent<Node>().id == index) { return true; }
        }
        return false;
    }
    void Update()
    {
        

        if (!isInBubble(currentIndex))
        {
            //Debug.Log(GetComponent<Bubble>().currentBubbleCenter);
            
            if (GetComponent<Bubble>().currentBubbleCenter != null && !Bubble.REST_POS.Equals(GameObject.FindGameObjectWithTag("Bubble").transform.position))
            {
                Debug.Log("first assigned");
                currentIndex = GetComponent<Bubble>().currentBubbleCenter.GetComponent<Node>().id;
                currentSelectNode = GetComponent<Bubble>().currentBubbleCenter;
                GetComponent<Bubble>().currentBubbleCenter.GetComponent<Renderer>().material.color = highlightColor;
            }
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            chooseSelectedNode();
        }
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
		if (direction == Direction.up) {
			chooseSelectedNode();
		}
        if (direction == Direction.right)
        {
            nodes = HighlightNode.GetAllHighlighted();
            int index = nodes.FindIndex(c => c.GetComponent<Node>().id == currentIndex);
            Debug.Log("lastIndex =" + index);
            if (nodes.Count > 0 && index + 2 > nodes.Count)
            {
                Debug.Log("nodesCount was smaller than 2+index");
                foreach (GameObject g in nodes)
                {
                    if (!g.GetComponent<Node>().derAuserwaehlte)
                    {
                        g.GetComponent<Renderer>().material.color = HighlightNode.highlightColor;
                    }
                    g.GetComponent<Node>().gotHit = false;
                }
                currentIndex = nodes[0].GetComponent<Node>().id;
                nodes[0].GetComponent<Renderer>().material.color = highlightColor;
                currentSelectNode = nodes[0];
            }
            else if (nodes.Count > 0)
            {
                Debug.Log((2 + index) + " was not bigger than" + nodes.Count);
                foreach (GameObject g in nodes)
                {
                    if (!g.GetComponent<Node>().derAuserwaehlte)
                    {
                        g.GetComponent<Renderer>().material.color = HighlightNode.highlightColor;
                    }
                    g.GetComponent<Node>().gotHit = false;
                }
                currentIndex = nodes[index + 1].GetComponent<Node>().id;
                nodes[index + 1].GetComponent<Renderer>().material.color = highlightColor;
                currentSelectNode = nodes[index + 1];
            }
        }
        else if(direction == Direction.left)
        {

            nodes = HighlightNode.GetAllHighlighted();
            int index = nodes.FindIndex(c => c.GetComponent<Node>().id == currentIndex);
            Debug.Log("lastIndex =" + index);
            if (nodes.Count > 0 && (index - 1) >= 0)
            {
                foreach (GameObject g in nodes)
                {
                    if (!g.GetComponent<Node>().derAuserwaehlte)
                    {
                        g.GetComponent<Renderer>().material.color = HighlightNode.highlightColor;
                    }
                    g.GetComponent<Node>().gotHit = false;
                }
                currentIndex = nodes[index - 1].GetComponent<Node>().id;
                nodes[index - 1].GetComponent<Renderer>().material.color = highlightColor;
                currentSelectNode = nodes[index - 1];
            }
            else if (nodes.Count > 0)
            {
                foreach (GameObject g in nodes)
                {
                    if(!g.GetComponent<Node>().derAuserwaehlte)
                    {
                        g.GetComponent<Renderer>().material.color = HighlightNode.highlightColor;
                    }
                    g.GetComponent<Node>().gotHit = false;
                }
                currentIndex = nodes[nodes.Count-1].GetComponent<Node>().id;
                nodes[nodes.Count-1].GetComponent<Renderer>().material.color = highlightColor;
                currentSelectNode = nodes[nodes.Count - 1];
            }
        }
    }
    public void chooseSelectedNode()
    {
        if (currentSelectNode != null)
        {
            if (currentSelectNode.GetComponent<Node>().derAuserwaehlte)
            {
                experimentLogger.getLogger().correctNodehit = "t";
            }
            else
            {
                experimentLogger.getLogger().correctNodehit = "f";
            }
            currentIndex = -1;
            currentSelectNode.GetComponent<Node>().gotHit = true;
        } 
    }
    
    enum Direction { up, down, left, right};

}
