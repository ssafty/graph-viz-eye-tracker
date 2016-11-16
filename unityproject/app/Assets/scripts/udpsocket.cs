using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Net;
using System.Text;

public class udpsocket : MonoBehaviour
{
    public int Port;
    UdpClient Client;
    public GameObject eyepointer;
    Vector2 LastEyeCoordinate;
    public GameObject camera;

    createMarker markerScript;
    Bubble bubbleScript;
    RectTransform rect;
    void Start()
    {
        markerScript = camera.GetComponent<createMarker>();
        bubbleScript = camera.GetComponent<Bubble>();
        rect = eyepointer.GetComponent<RectTransform>();
        Client = new UdpClient(Port);
        try
        {
            Client.BeginReceive(new AsyncCallback(recv), null);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    void Update()
    {
        bubbleScript.calcBubble(LastEyeCoordinate);
        //rect.anchoredPosition = LastEyeCoordinate;
    }

    private void recv(IAsyncResult res)
    {
		IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, Port);
        byte[] received = Client.EndReceive(res, ref RemoteIpEndPoint);
        Client.BeginReceive(new AsyncCallback(recv), null);
        ShowPositionOnScreen(false,  Encoding.UTF8.GetString(received));
    }
    void ShowPositionOnScreen(Boolean flip_y, String response)
	{
        if (response.Length > 0)
        {
		//	Debug.Log (response);
            string[] responseArray = response.Split(')');

	
            foreach (string tuple in responseArray)
            {
				//Debug.Log (tuple);
                string[] tempCoordinates = tuple.Split(',');
                if (tempCoordinates.Length == 2)
                {
                    
                    double x, y;

                    double.TryParse(tempCoordinates[0].Substring(1), out x);
                    double.TryParse(tempCoordinates[1].Substring(1), out y);
                    
					x *= markerScript.newScreen.x ;
                    if (flip_y)
                    {
                        y = 1 - y;
                    }
					y *= markerScript.newScreen.y;
					Debug.Log("x" + x + ";y" + y);
                    
                    Vector2 newPos = new Vector2(((float)x - (markerScript.newScreen.x / 2)), (float)y - (markerScript.newScreen.y / 2));
//                    Debug.Log("moving cube to x: " + x + "y: " + y);
                    LastEyeCoordinate = newPos;
                    //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"F:\eyetracker\eyetracker-project\coordinateLog.csv", true))
                    //{
                    //    file.WriteLine(x + ";" + y);
                    //}
                }
            }
        }
        else
        {
            Vector2 newPos = new Vector2(((float)Input.mousePosition.x - (Screen.width / 2)), (float)Input.mousePosition.y - (Screen.height / 2));

            eyepointer.GetComponent<RectTransform>().anchoredPosition = newPos;
        }
    }
}
