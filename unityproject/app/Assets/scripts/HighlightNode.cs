using UnityEngine;
using System.Collections;
using System.Runtime.ConstrainedExecution;

public class HighlightNode : MonoBehaviour
{

	public Color highlightColor = Color.green;
	private Color white = Color.white;

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
		}
	}
}
