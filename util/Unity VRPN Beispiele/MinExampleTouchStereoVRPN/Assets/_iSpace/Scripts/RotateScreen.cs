using UnityEngine;
using System.Collections;

public class RotateScreen : MonoBehaviour {

public float speed = 1.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.R))
		{
			transform.Rotate(speed,0,0,Space.Self);
		}
		if(Input.GetKey(KeyCode.F))
		{
			transform.Rotate(-speed,0, 0,Space.Self);
		}	
	}
}
