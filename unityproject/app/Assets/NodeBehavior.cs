using UnityEngine;
using System.Collections;

public class NodeBehavior : MonoBehaviour {

	private GameObject cam;
	public float opaque_distance = 30.0f;

	private Renderer rend;

	// Use this for initialization
	void Start () {
		cam = GameObject.Find ("Main Camera");
		rend = GetComponent<Renderer>();
	}

	// Update is called once per frame
	void Update () {
		float opacity = Vector3.Distance (transform.position, cam.transform.position);
		opacity = Mathf.Clamp (opacity, 0.0f, opaque_distance) / opaque_distance;
		Color color = new Color (rend.material.color.r, rend.material.color.b, rend.material.color.g, opacity);
		rend.material.color = color;
	}
}
