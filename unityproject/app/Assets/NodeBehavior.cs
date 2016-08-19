using UnityEngine;
using System.Collections;

public class NodeBehavior : MonoBehaviour {

	private GameObject cam;
	public float opaque_distance;

	private Renderer rend;
	public float opaqueClamp;

	// Use this for initialization
	void Start () {
		cam = GameObject.Find ("Main Camera");
		rend = GetComponent<Renderer>();
		opaqueClamp = opaque_distance * transform.lossyScale.magnitude / 2.0f;
	}

	// Update is called once per frame
	void Update () {
		print (transform.lossyScale);
		float opacity = Vector3.Distance (transform.position, cam.transform.position);
		opacity = Mathf.Clamp (opacity, 0.0f, opaqueClamp) / opaqueClamp;
		Color color = new Color (rend.material.color.r, rend.material.color.b, rend.material.color.g, opacity);
		rend.material.color = color;
	}
}
