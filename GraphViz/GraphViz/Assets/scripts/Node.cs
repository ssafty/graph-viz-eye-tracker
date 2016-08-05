using UnityEngine;
using System.Collections;

public class Node : MonoBehaviour {

	public string id;
	public int count;
	public TextMesh nodeText;
	
	// Update is called once per frame
	void Update () {
		Vector3 targetPosition = Camera.main.transform.position;
		Vector3 currentPosition = transform.position;
		targetPosition.y = 0;
		currentPosition.y = 0;
		nodeText.transform.rotation = Quaternion.LookRotation(currentPosition - targetPosition, Vector3.up);
	}
}
