using UnityEngine;
using System.Collections;

public class debugMousecast : MonoBehaviour {

    public GameObject bubble;
	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            Vector3 newPos = bestBubble();
            if (newPos != Vector3.zero)
            {
                bubble.transform.position = newPos;
            }
        }
    }
    
    Vector3 simpleBubble()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    Vector3 meanBubble()
    {
        RaycastHit[] hits;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        hits = Physics.RaycastAll(ray);
        Vector3 positionSum = Vector3.zero;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            Debug.Log(hit);
            if (hit.collider.gameObject.tag == "Node")
            {
                positionSum = positionSum + hit.collider.gameObject.transform.position;
            }
        }
        return (positionSum / Mathf.Max(1, hits.Length));
    }

    Vector3 bestBubble()
    {
        RaycastHit[] hits;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        hits = Physics.RaycastAll(ray);
        Vector3 positionSum = Vector3.zero;
        float bestShotDistance = float.MaxValue;
        GameObject bestShotNode = null;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            Debug.Log(hit);

            if (hit.collider.gameObject.tag == "Node")
            {
                float distance = Vector2.Distance(new Vector2(hit.collider.gameObject.transform.position.x, hit.collider.gameObject.transform.position.y), new Vector2(hit.point.x,hit.point.y));

                    if (distance < bestShotDistance)
                {
                    bestShotDistance = distance;
                    bestShotNode = hit.collider.gameObject;
                }
            }
        }
        return bestShotNode==null?Vector3.zero:bestShotNode.transform.position;
    }

}
