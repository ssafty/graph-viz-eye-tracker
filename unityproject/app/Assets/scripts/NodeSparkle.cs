using UnityEngine;
using System.Collections;

public class NodeSparkle : MonoBehaviour {

	//Start is called once
	void Start() {
		
	}
	// Update is called once per frame
	void Update () {
		
	}

    void letMircoAndHisFriendsShine(GameObject mirco)
    {
        GameObject[] mircosPossibleFriends = GameObject.FindGameObjectsWithTag("Edge");
        StartCoroutine("startTheSparkle", mirco);
        foreach (GameObject friend in mircosPossibleFriends)
        {
            if(friend.GetComponent<Edge>().source == mirco || friend.GetComponent<Edge>().target == mirco)
            {
                if(friend.GetComponent<Edge>().source == mirco) { StartCoroutine("startTheSparkle", friend.GetComponent<Edge>().source); }
                else { StartCoroutine("startTheSparkle", friend.GetComponent<Edge>().target); }

                StartCoroutine("startTheSparkle", friend);
            }
        }
    }

    IEnumerator startTheSparkle(GameObject sparkly)
    {
        Renderer renderer = sparkly.GetComponent<Renderer>();
        renderer.material.SetColor("_Color", Color.red);
        yield return new WaitForSeconds(1);
        renderer.material.SetColor("_Color", Color.white);
        yield return new WaitForSeconds(1);
        renderer.material.SetColor("_Color", Color.red);
        yield return null;
    }
}
