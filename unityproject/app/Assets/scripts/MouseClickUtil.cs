using UnityEngine;
using System.Collections;
using System.Runtime.ConstrainedExecution;

public class MouseClickUtil : MonoBehaviour
{

	public static  int LEFT_BTN = 0;
	public static int DEFAULT_DISTANCE = 150;

	public static Node checkMouseClick (int btn, int distance)
	{
		Node clicked = null;
		if (Input.GetMouseButtonDown (btn)) {
			RaycastHit hitPoint;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hitPoint, distance)) { //used 150 instead of Mathf.Infinity
				clicked = (Node)hitPoint.transform.gameObject.GetComponent ("Node");
			}
		}
		return clicked;
	}

	public static Node checkMouseClick (int btn)
	{
		return MouseClickUtil.checkMouseClick (0, DEFAULT_DISTANCE);
	}
}

