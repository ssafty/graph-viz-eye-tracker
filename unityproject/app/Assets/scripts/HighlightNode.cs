using UnityEngine;
using System.Collections;
using System.Runtime.ConstrainedExecution;
using System.Collections.Generic;

public class HighlightNode : MonoBehaviour
{

	public Color highlightColor = Color.green;
	private Color white = Color.white;
	private bool highlighted = false;

	void OnTriggerStay (Collider other)
	{
		ChangeColorTo (other, highlightColor);
	}



	void OnTriggerExit (Collider other)
	{
		ChangeColorTo (other, white);
	}

	private void ChangeColorTo (Collider other, Color color)
	{
		Debug.Log (other + "  " + other.tag);
		bool ok = other.tag == "Bubble";
		if (ok) {
			gameObject.GetComponent<Renderer> ().material.color = color;
			highlighted = gameObject.GetComponent<Renderer> ().material.color == highlightColor;
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
		return selected;
	}
}
