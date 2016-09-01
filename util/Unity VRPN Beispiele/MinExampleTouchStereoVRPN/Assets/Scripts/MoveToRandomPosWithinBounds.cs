using UnityEngine;
using System.Collections;

public class MoveToRandomPosWithinBounds : MonoBehaviour {

    [SerializeField]
    Bounds bounds;
    [SerializeField]
    Vector3 currenttarget = Vector3.zero;
    [SerializeField]
    float nextTargetThreshold = 0.1f;
    [SerializeField]
    float maxDistanceToLastPoint = 1.0f;
    [SerializeField]
    float speed = 1.0f;
    private Vector3 offset = Vector3.zero;
    private Vector3 pos = Vector3.zero;
    private Vector3 direction = Vector3.zero;
    private Vector3 startpos;
    // Use this for initialization
    void Start () {
        startpos = transform.position;
        currenttarget = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        pos = transform.position;
        direction = (currenttarget - pos);
        offset = Vector3.ClampMagnitude(direction.normalized*speed*Time.deltaTime, direction.magnitude);
        transform.position += offset;
        if(Vector3.Distance(transform.position,currenttarget)<=nextTargetThreshold)
        {
            Vector3 newtarget = startpos+new Vector3(UnityEngine.Random.Range(bounds.min.x, bounds.max.x), UnityEngine.Random.Range(bounds.min.y, bounds.max.y), UnityEngine.Random.Range(bounds.min.z, bounds.max.z));
            currenttarget += Vector3.ClampMagnitude(newtarget-currenttarget,maxDistanceToLastPoint);
        }
	}
    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(startpos+bounds.center, bounds.size);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(currenttarget, 0.4f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pos, 0.4f);
    }
}
