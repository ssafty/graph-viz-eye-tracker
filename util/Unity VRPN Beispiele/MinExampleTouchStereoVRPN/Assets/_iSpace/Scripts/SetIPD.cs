using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SetIPD : MonoBehaviour {
	public GameObject left;
	public GameObject right;
	public float difference = 0.00f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 offset = new Vector3(difference/2,0,0);
		if(left!=null)
			left.transform.localPosition	= -offset;
		if(right!=null)
			right.transform.localPosition	= offset;
	}
}
