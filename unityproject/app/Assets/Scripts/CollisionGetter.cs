using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionGetter : MonoBehaviour {

	private Collision LastCol;

	public Collision collision	//Gets the last collision
	{
		get{ return LastCol; }
	}

	void OnCollisionStay(Collision collision) {
		LastCol = collision;
	}

	void OnCollisionExit(Collision collision) {
		LastCol = null;
	}
}
