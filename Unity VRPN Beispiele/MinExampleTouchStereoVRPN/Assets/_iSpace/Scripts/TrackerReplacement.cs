using UnityEngine;
using System.Collections;

public class TrackerReplacement : MonoBehaviour {
	public GameObject go;
	public Vector3 relativeOffset;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.localPosition = go.transform.localPosition+go.transform.InverseTransformDirection(relativeOffset);
		SetLocalToWorldScale set = GetComponent<SetLocalToWorldScale>();
		if(set!=null)
		{
			transform.localPosition *= set.LocalToWorldScale;
		}
	}
}
