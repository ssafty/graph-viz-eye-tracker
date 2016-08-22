using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SetLocalToWorldScale))]
public class MoveObject : MonoBehaviour {
	SetLocalToWorldScale scale;
	public float speed = 1.0f;
	public Bounds bounds;
	// Use this for initialization
	void Start () {
		scale = GetComponent<SetLocalToWorldScale>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.A))
		{
			transform.localPosition = bounds.ClosestPoint(transform.localPosition+ new Vector3(-speed*scale.LocalToWorldScale,0,0));
		}
		if(Input.GetKey(KeyCode.D))
		{
			transform.localPosition = bounds.ClosestPoint(transform.localPosition+  new Vector3(speed*scale.LocalToWorldScale,0,0));
		}
		if(Input.GetKey(KeyCode.W))
		{
			transform.localPosition = bounds.ClosestPoint(transform.localPosition+  new Vector3(0,0,speed*scale.LocalToWorldScale));
		}
		if(Input.GetKey(KeyCode.S))
		{
			transform.localPosition = bounds.ClosestPoint(transform.localPosition+  new Vector3(0,0,-speed*scale.LocalToWorldScale));
		}
		if(Input.GetKey(KeyCode.Q))
		{
			
			transform.localPosition = bounds.ClosestPoint(transform.localPosition+ new Vector3(0,speed*scale.LocalToWorldScale,0));
		}
		if(Input.GetKey(KeyCode.E))
		{
			transform.localPosition = bounds.ClosestPoint(transform.localPosition+  new Vector3(0,-speed*scale.LocalToWorldScale,0));
		}
		
	}
}
