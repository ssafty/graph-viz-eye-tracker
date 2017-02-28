using UnityEngine;

using System.Collections;
using System.IO;

public class experimentLogger : MonoBehaviour
{
    
	System.IO.StreamWriter file;
    
	public int currentGraphSize;
	public string bubbleSize;
	public string participantId;
    public string keypressed;
    //public string trialtime;
    public string currentNode;
    public string targetNode;
    public string correctNodehit;
    public string condition;
    private string calibrationData = "noCalibrationDataSet";
    void Start ()
	{
        
		participantId = "1";
		file = new System.IO.StreamWriter(@"ExperimentLog\"+participantId+".csv");
		file.WriteLine ("participantId,condition,timeSinceStartup,correctNodeHit,keypressed,calibrationData,bubbleSize,numberNodes,targetNode,currentSelectedNode");
		file.Flush ();
	}

	void Update ()
	{
        if(Input.anyKey)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                keypressed = "Enter";
            } else { 
            keypressed = Input.inputString;
            }
        } else
        {
            keypressed = "";
        }
		
	}
	// lateUpdate is called once per frame after all other Updates
	void LateUpdate ()
	{
		file.WriteLine (participantId + "," + condition  + "," + Time.realtimeSinceStartup + "," + correctNodehit + "," + keypressed + "," +calibrationData+ "," + bubbleSize + "," + currentGraphSize.ToString() + "," + targetNode + "," + currentNode);
		file.Flush ();
		correctNodehit = "";
	}

	public static experimentLogger getLogger ()
	{
		return GameObject.FindGameObjectWithTag ("GameController").GetComponent<experimentLogger> ();
	}
}
