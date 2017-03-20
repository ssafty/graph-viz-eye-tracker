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
    public string currentState;
    private string calibrationData = "noCalibrationDataSet";
    public string currentEyePosRawX;
    public string currentEyePosRawY;
    public string currentEyePosAftercalibX;
    public string currentEyePosAftercalibY;
    private Vector2 neu;
    void Start ()
	{

        int fileCount = Directory.GetFiles(@"ExperimentLog\", "*.csv*").Length;
        participantId = (fileCount+1).ToString();
		file = new System.IO.StreamWriter(@"ExperimentLog\"+participantId+".csv");
		file.WriteLine ("participantId,condition,timeSinceStartup,correctNodeHit,keypressed,calibrationData,bubbleSize,numberNodes,targetNode,currentSelectedNode,currentState,correctedEyeX,correctedEyeY,rawEyeX,rawEyeY");
		file.Flush ();
	} 
	void Update ()
	{
    
        neu = GameObject.FindGameObjectWithTag("eyepointer_corrected").GetComponent<RectTransform>().anchoredPosition;
        currentEyePosAftercalibX = (neu.x + (Screen.width / 2)).ToString();
        currentEyePosAftercalibY = (neu.y + (Screen.height / 2)).ToString();
        neu = GameObject.FindGameObjectWithTag("eyepointer").GetComponent<RectTransform>().anchoredPosition;
        currentEyePosRawX = (neu.x + (Screen.width / 2)).ToString();
        currentEyePosRawY = (neu.y + (Screen.height / 2)).ToString();
        if (Input.anyKey)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                keypressed = "Enter";
            }
            else if (Input.GetMouseButton(0))
            {
                keypressed = "LEFT_MOUSE";
            }
            else if (Input.GetMouseButton(1))
            {
                keypressed = "RIGHT_MOUSE";
            }
            else
            {
                keypressed = Input.inputString;
            }
        }
        else if (HapringSelectAndRotate.lastPressed != "")
        {
            keypressed = HapringSelectAndRotate.lastPressed;
            HapringSelectAndRotate.lastPressed = "";
        }
       
        else if (Input.GetAxis("Zoom") != 0) {
            keypressed = "MOUSE_WHEEL";
        } else {
            keypressed = "";
        }
		
	}
	// lateUpdate is called once per frame after all other Updates
	void LateUpdate ()
	{
		file.WriteLine (participantId + "," + condition  + "," + Time.realtimeSinceStartup + "," + correctNodehit + "," + keypressed + "," +calibrationData+ "," + bubbleSize + "," + currentGraphSize.ToString() + "," + targetNode + "," + currentNode + "," + currentState +","+ currentEyePosAftercalibX +","+ currentEyePosAftercalibY+","+ currentEyePosRawX + ","+ currentEyePosRawY);
		file.Flush ();
		correctNodehit = "";
	}

	public static experimentLogger getLogger ()
	{
		return GameObject.FindGameObjectWithTag ("GameController").GetComponent<experimentLogger> ();
	}
}
