using UnityEngine;
using System.Collections;

public class Edge : MonoBehaviour
{
	public string id;
	public Node source;
	public Node target;
	public string sourceId;
	public string targetId;

	private LineRenderer lineRenderer;

	// Use this for initialization
	void Start ()
	{
		lineRenderer = gameObject.AddComponent<LineRenderer> ();


		//lineRenderer.material = new Material (Shader.Find("Self-Illumin/Diffuse"));
		lineRenderer.material.SetColor ("_Color", Color.grey);
		lineRenderer.SetWidth (0.05f, 0.05f);
		lineRenderer.SetVertexCount (2);
		lineRenderer.SetPosition (0, new Vector3 (0, 0, 0));
		lineRenderer.SetPosition (1, new Vector3 (1, 0, 0));	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (target != null && source != null) {
			Vector3 dir = (target.transform.position - source.transform.position).normalized;

			lineRenderer.SetPosition (0, source.transform.position + dir * source.scale_size/2); 
			lineRenderer.SetPosition (1, target.transform.position - dir * target.scale_size/2);
		}
	}
}
