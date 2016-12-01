using UnityEngine;
using System.Collections;
using System.Runtime.ConstrainedExecution;

public class MouseClickUtil : MonoBehaviour
{

	public static  int LEFT_BTN = 0;
	public static int DEFAULT_DISTANCE = 150;

	public static GameObject checkMouseClick (int btn, int distance)
	{
		GameObject clicked = null;
		if (Input.GetMouseButtonDown (btn)) {
			RaycastHit hitPoint;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hitPoint, distance)) { //used 150 instead of Mathf.Infinity
				clicked = hitPoint.transform.gameObject;
			}
		}
		return clicked;
	}

	public static GameObject checkMouseClick (int btn)
	{
		return MouseClickUtil.checkMouseClick (0, DEFAULT_DISTANCE);
	}
}

