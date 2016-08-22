using UnityEngine;
using System.Collections;

public class SetLocalToWorldScale : MonoBehaviour {
	
	float oldlocalToWorldScale = 1.0f;
	[SerializeField]
	float localToWorldScale = 1.0f;
	public float LocalToWorldScale {
		get {
			return this.localToWorldScale;
		}
		set {
			oldlocalToWorldScale = localToWorldScale;
			localToWorldScale = value;
		}
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		DoUpdate();
	}
	public void DoUpdate()
	{
		transform.localPosition = transform.localPosition*localToWorldScale/oldlocalToWorldScale;
		
	}
}
