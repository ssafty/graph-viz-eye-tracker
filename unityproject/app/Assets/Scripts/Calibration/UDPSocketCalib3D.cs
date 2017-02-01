using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Net;
using System.Text;
using System.Collections.Generic;

public class UDPSocketCalib3D : MonoBehaviour
{
    public int Port;
    UdpClient Client;
    public GameObject eyepointer;
    public Vector2 LastEyeCoordinate;
    public GameObject camera;

    createMarker markerScript;
    Bubble bubbleScript;
    RectTransform rect;

    private List<Vector2> processingList;

    void Start()
    {
        processingList = new List<Vector2>();
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
        //bubbleScript.calcBubble(LastEyeCoordinate);

        //filter out when list gets 30 points
        if (processingList.Count >= 30)
        {
            LastEyeCoordinate = FilterGazeCoordinates(processingList);
            processingList = new List<Vector2>();
        }

        rect.anchoredPosition = LastEyeCoordinate;
    }

    private void recv(IAsyncResult res)
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, Port);
        byte[] received = Client.EndReceive(res, ref RemoteIpEndPoint);

        Client.BeginReceive(new AsyncCallback(recv), null);

        if (Encoding.UTF8.GetString(received).Length > 0)
        {
            //parse coordinates
            processingList.AddRange(ExtractGazeCoordinates(false, Encoding.UTF8.GetString(received)));
        }
        else
        {
            eyepointer.GetComponent<RectTransform>().anchoredPosition = new Vector2(((float)Input.mousePosition.x - (Screen.width / 2)), (float)Input.mousePosition.y - (Screen.height / 2));
            processingList = new List<Vector2>();
        }
    }

    List<Vector2> ExtractGazeCoordinates(Boolean flip_y, String response)
    {
        string[] responseArray = response.Split(')');
        List<Vector2> outputVec = new List<Vector2>();

        foreach (string tuple in responseArray)
        {
            string[] tempCoordinates = tuple.Split(',');
            if (tempCoordinates.Length == 2)
            {
                double x, y;

                double.TryParse(tempCoordinates[0].Substring(1), out x);
                double.TryParse(tempCoordinates[1].Substring(1), out y);

                x *= markerScript.newScreen.x;
                if (flip_y) { y = 1 - y; }
                y *= markerScript.newScreen.y;

                //Debug.Log("x" + x + " ;y" + y);
                Debug.Log("GETTING INPUT FROM UDP SERVER");
                outputVec.Add(new Vector2(((float)x - (markerScript.newScreen.x / 2)), (float)y - (markerScript.newScreen.y / 2)));
            }
        }
        return outputVec;
    }

    Vector2 FilterGazeCoordinates(List<Vector2> processingList)
    {
        Double x_sum = 0;
        Double y_sum = 0;

        foreach (Vector2 point in processingList)
        {
            x_sum += point.x;
            y_sum += point.y;
        }

        Vector2 centeroid = new Vector2((float)x_sum / processingList.Count, (float)y_sum / processingList.Count);

        List<Vector3> DistanceFromCenteroid = new List<Vector3>();

        foreach (Vector2 point in processingList)
        {
            DistanceFromCenteroid.Add(new Vector3(point.x, point.y, Vector2.Distance(point, centeroid)));
        }

        DistanceFromCenteroid.Sort((a, b) => a.z.CompareTo(b.z));
        List<Vector3> trimmedList = DistanceFromCenteroid.GetRange(0, (int)(processingList.Count * 0.75));

        x_sum = 0;
        y_sum = 0;
        foreach (Vector3 point in trimmedList)
        {
            x_sum += point.x;
            y_sum += point.y;
        }
        return new Vector2((float)x_sum / trimmedList.Count, (float)y_sum / trimmedList.Count);
    }

}

