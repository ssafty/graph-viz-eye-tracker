using UnityEngine;
using System.Collections;

public class SmoothlyFollowObject : UsingTarget {

    [SerializeField]
    float speedPerSecond;
    [SerializeField]
    [Range(0,1)]
    float turn;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //transform.position += Vector3.ClampMagnitude(Target.position - transform.position, speedPerSecond*Time.deltaTime);
        transform.position += Vector3.ClampMagnitude(transform.forward, speedPerSecond * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Target.position - transform.position), turn * Time.deltaTime);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Target.position, 0.4f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.4f);
    }
}
