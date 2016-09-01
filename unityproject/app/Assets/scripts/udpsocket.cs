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
    public int screenHeight, screenWidth;
    createMarker markerScript;
    RectTransform rect;
    void Start()
    {
        markerScript = camera.GetComponent<createMarker>();
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
        rect.anchoredPosition = LastEyeCoordinate;
    }

    private void recv(IAsyncResult res)
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);
        byte[] received = Client.EndReceive(res, ref RemoteIpEndPoint);
        Client.BeginReceive(new AsyncCallback(recv), null);
        ShowPositionOnScreen(false, screenWidth, screenHeight, Encoding.UTF8.GetString(received));
    }
    void ShowPositionOnScreen(Boolean flip_y, int width, int height, String response)
    {
        if (response.Length > 0)
        {
            string[] responseArray = response.Split(')');
            foreach (string tuple in responseArray)
            {
                string[] tempCoordinates = tuple.Split(',');
                if (tempCoordinates.Length == 2)
                {
                    
                    double x, y;

                    double.TryParse(tempCoordinates[0].Substring(1), out x);
                    double.TryParse(tempCoordinates[1].Substring(1), out y);
                    Debug.Log("x" + x + ";y" + y);
                    x *= width;
                    if (flip_y)
                    {
                        y = 1 - y;
                    }
                    y *= height;

                    
                    Vector2 newPos = new Vector2(((float)x - (markerScript.newScreen.x / 2)), (float)y - (markerScript.newScreen.y / 2));
                    Debug.Log("moving cube to x: " + x + "y: " + y);
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
