using UnityEngine;

using System.Collections;
using System.IO;

public class experimentLogger : MonoBehaviour
{

	System.IO.StreamWriter file;
	public string currentGraph;
	public string bubbleSize;
	public string participantId;
	public string keypressed;
	public string trialtime;
	public string currentHighlightedNode;
	public string correctNodehit;

	void Start ()
	{
        
		participantId = "1";
		file = new System.IO.StreamWriter (@"ExperimentLogFolder" + Path.DirectorySeparatorChar + participantId + ".csv");
		file.WriteLine ("timestamp,currentGraph,bubbleSize,participantId,keypressed,currentHighlightedNode,correctNodehit");
		file.Flush ();
	}

	void Update ()
	{
		keypressed = Input.anyKey ? Input.inputString : "";
	}
	// lateUpdate is called once per frame after all other Updates
	void LateUpdate ()
	{
		file.WriteLine (Time.realtimeSinceStartup + "," + currentGraph + "," + bubbleSize + "," + participantId + "," + keypressed, "," + currentHighlightedNode + "," + correctNodehit);
		file.Flush ();
		correctNodehit = "";
	}

	public static experimentLogger getLogger ()
	{
		return GameObject.FindGameObjectWithTag ("GameController").GetComponent<experimentLogger> ();
	}
}
