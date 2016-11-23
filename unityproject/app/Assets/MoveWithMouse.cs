using UnityEngine;
using System.Collections;

public class MoveWithMouse : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 newPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,Mathf.Abs(Camera.main.transform.position.z-transform.position.z)));
        newPos.z = transform.position.z;


        transform.position = newPos;
	}
}
