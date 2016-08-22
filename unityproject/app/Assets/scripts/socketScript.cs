using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

public class socketScript : MonoBehaviour
{
	private TCPConnection myTCP;
	private string serverMsg;
	public string msgToServer;

	public GameObject eyepointer;
	public Vector2 LastEyeCoordinate;

	void Awake ()
	{

		myTCP = gameObject.AddComponent<TCPConnection> ();
		InvokeRepeating ("getRandomData", 0.0f, 1.0f);
	}

	void Start ()
	{
		LastEyeCoordinate = Vector2.zero;
	}

	void Update ()
	{
		ShowPositionOnScreen (false, 3840, 2160);
		eyepointer.GetComponent<RectTransform> ().anchoredPosition = LastEyeCoordinate;

	}

	void OnGUI ()
	{
		//Falls die Verbindung noch nicht hergestellt wurde, zeige Button zum verbinden an
		if (myTCP.socketReady == false) {
			if (GUILayout.Button ("Connect")) {
				Debug.Log ("Attempting to connect..");
				myTCP.setupSocket ();
			}
		}
		//Wenn Verbindung hergestellt wurde, kann hiermit eine Nachricht an den Server geschickt werden
		if (myTCP.socketReady == true) {
			msgToServer = GUILayout.TextField (msgToServer);
			if (GUILayout.Button ("Write to server", GUILayout.Height (30))) {
				SendToServer (msgToServer);
			}
		}
	}

	void ShowPositionOnScreen (Boolean flip_y, int width, int height)
	{

		string response = SocketResponse ();
		Debug.Log (response);
		if (response.Length > 0) {
			string[] responseArray = response.Split (')');

			foreach (string tuple in responseArray) {

				string[] tempCoordinates = tuple.Split (',');
				if (tempCoordinates.Length == 2) {
					double x, y;

					double.TryParse (tempCoordinates [0].Substring (1), out x);
					double.TryParse (tempCoordinates [1].Substring (1), out y);

					x *= width;
					if (flip_y) {
						y = 1 - y;
					}
					y *= height;
					Vector2 newPos = new Vector2 (((float)x - (Screen.width / 2)), (float)y - (Screen.height / 2));
					Debug.Log ("moving cube to x: " + x + "y: " + y);
					LastEyeCoordinate = newPos;
					using (System.IO.StreamWriter file = new System.IO.StreamWriter (@"F:\eyetracker\eyetracker-project\coordinateLog.csv", true)) {
						file.WriteLine (x + ";" + y);
					}
				}
			}
		} else {
			Vector2 newPos = new Vector2 (((float)Input.mousePosition.x - (Screen.width / 2)), (float)Input.mousePosition.y - (Screen.height / 2));

			Cursor.visible = false;
			eyepointer.GetComponent<RectTransform> ().anchoredPosition = newPos;
		}
	}

	string SocketResponse ()
	{
		if (myTCP.socketReady == true) {
			string serverSays = myTCP.readSocket ();

			if (serverSays != "") {
				Debug.Log ("[SERVER]" + serverSays);
			}
			return serverSays;
		} else
			return "";
	}

	public void SendToServer (string str)
	{
		myTCP.writeSocket (str);
		Debug.Log ("[CLIENT] -> " + str);
	}

	void getRandomData ()
	{
		if (myTCP.socketReady == true) {
			SendToServer ("generaterandomdata");
		}
	}

}

