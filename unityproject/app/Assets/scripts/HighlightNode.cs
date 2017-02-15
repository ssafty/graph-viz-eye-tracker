using UnityEngine;
using System.Collections;
using System.Runtime.ConstrainedExecution;
using System.Collections.Generic;
using System;

public class HighlightNode : MonoBehaviour
{

	public static Color highlightColor = Color.green;
	private Color white = Color.white;
	private bool highlighted = false;

	void OnTriggerEnter (Collider other)
	{
        if(gameObject.GetComponent<Renderer>().material.color != Color.magenta) { 
		ChangeColorTo (other, highlightColor);
        } else
        {
            highlighted = true;
        }
    }



	void OnTriggerExit (Collider other)
	{
		ChangeColorTo (other, white);
	}

	private void ChangeColorTo (Collider other, Color color)
	{
		bool ok = other.tag == "Bubble";
		if (ok) {
			gameObject.GetComponent<Renderer> ().material.color = color;
			highlighted = gameObject.GetComponent<Renderer> ().material.color != Color.white;
		}
	}

	public static List<GameObject> GetAllHighlighted ()
	{
		GameObject[] nodes = GameObject.FindGameObjectsWithTag ("Node");
		List<GameObject> selected = new List<GameObject> ();
		foreach (GameObject n in nodes) {
			if (n.GetComponent<HighlightNode> ().highlighted) {
				selected.Add (n);
			}
		}
        selected.Sort(new myReverserClass());
        return selected;
	}

}
public class myReverserClass : IComparer<GameObject>
{
    public int Compare(GameObject x, GameObject y)
    {
        GameObject nodex = (GameObject) x;
        GameObject nodey = (GameObject) y;
        return nodex.GetComponent<Node>().id.CompareTo(nodey.GetComponent<Node>().id);
    }

}
