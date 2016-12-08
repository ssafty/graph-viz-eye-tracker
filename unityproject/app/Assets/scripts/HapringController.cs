using UnityEngine;
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
		// get the highlighted nodes
		List<GameObject> nodes = HighlightNode.GetAllHighlighted();

		// if no highlighted nodes exit
		if (nodes.Count == 0) return;
			
		// check if current index in range
		if (currentIndex >= nodes.Count) currentIndex = 0;
		if (currentIndex == -1) currentIndex = nodes.Count-1;

		// check for operation next or previous
		if (direction == Direction.right) {
			currentIndex += 1;
		} else if (direction == Direction.left) {
			currentIndex -= 1;
		} else if (direction == Direction.up) {
			// select the node
			nodes [currentIndex].GetComponent<Renderer> ().material.color = Color.red;
			return;
		}

		// check if next index in range
		if (currentIndex >= nodes.Count) currentIndex = 0;
		if (currentIndex == -1) currentIndex = nodes.Count-1;

		// reset color of all nodes
		foreach (GameObject n in nodes){
			n.GetComponent<Renderer> ().material.color = HighlightNode.highlightColor;
		}

		// highlight the node
		nodes [currentIndex].GetComponent<Renderer> ().material.color = Color.yellow;
    }
    enum Direction { up, down, left, right};

}
