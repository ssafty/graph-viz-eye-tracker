using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

public class HapringSelectAndRotate : Singleton<HapringSelectAndRotate>
{

	private int currentIndex = -1;

	public enum BUTTON_DATA
	{
		BUTTONA_PRESSED = 0,
		BUTTONA_RELEASED = 1,
		BUTTONB_PRESSED = 2,
		BUTTONB_RELEASED = 3,
		NONE = 4
	}

	public enum JOYSTICK_DATA
	{
		INCREASE = 0,
		DECREASE = 1,
		NONE = 2
	}

	public string serverIP = "127.0.0.1";
	public static int dllPort = 26000;
	public int unityPort = 27000;
	byte[] data;
	Socket sender;
	IPEndPoint target;
	UDPReceiver workerObject;

	float power;
	float lastSentPower = 0;

	static byte lastJoystickEventReceived = (byte)JOYSTICK_DATA.NONE;
	public static byte lastCommandValue = (byte)BUTTON_DATA.NONE;
	public static double qw;
	public static double qx;
	public static double qy;
	public static double qz;
	public static Quaternion rot;
	public static float xDeg = 0;
	public static float yDeg = 0;
	public static float zDeg = 0;

	Coroutine c;
	Thread workerThread;

	public GameObject bubble;
	public Camera mainCamera;
	private Transform mainCameraParentTransform;

	void Awake()
	{
		sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		target = new IPEndPoint(IPAddress.Parse(serverIP), dllPort);
		Debug.Log("UDP sender ready!");

		workerObject = new UDPReceiver(unityPort, dllPort);
		workerThread = new Thread(workerObject.DoWork);

		workerThread.Start();
		Debug.Log("main thread: Starting worker thread...");

		while (!workerThread.IsAlive) ;

		Debug.Log("UDP receiver ready!");

		doRequest(0);
		requestChangeOnOrientationStatus(true);

		Settings settings = SaveLoad.Load();
		xDeg = settings.xDeg;
		yDeg = settings.yDeg;
		zDeg = settings.zDeg;
	}

	void OnDestroy()
	{
		doRequest(0);
		Debug.Log("worker thread stopping");

		workerObject.RequestStop();
		workerThread.Join();
		try
		{
			sender.Shutdown(SocketShutdown.Both);
			sender.Close();
		}
		catch (Exception e)
		{
			Debug.Log(e);
		}

		Debug.Log("sender destroyed");
	}

	void Start(){
		if (bubble == null) {
			Debug.LogError ("Please attach bubble game object to this script!!!");
		}
		if (mainCamera == null) {
			Debug.LogError ("Please attach main camera to this script!!!");
		} else {
			mainCameraParentTransform = mainCamera.transform.parent;
		}

	}

	void Update()
	{
		// select the nodes
		nodeSelection ();

		// rotate around bubble
		rotateTargetObject();
	}

	// code for selecting the nodes
	void nodeSelection()
	{
		// get the highlighted nodes
		List<GameObject> nodes = HighlightNode.GetAllHighlighted();

		// if no highlighted nodes exit
		if (nodes.Count == 0) return;

		// check if current index in range
		if (currentIndex >= nodes.Count) currentIndex = 0;
		if (currentIndex == -1) currentIndex = nodes.Count-1;

		// check for operation next or previous
		if (checkIncreaseJoystickEvent ()) {
			Debug.Log ("NODE BROWSING UP");
			currentIndex += 1;
		} else if (checkDecreaseJoystickEvent ()) {
			Debug.Log ("NODE BROWSING DOWN");
			currentIndex -= 1;
		} else if (checkButtonBStatus ()) {
			// select the node
			Debug.Log ("NODE SELECTION");
			nodes [currentIndex].GetComponent<Renderer> ().material.color = Color.red;
			startVibration();
			Invoke ("stopVibration", 0.5f);
			return;
		} else {
			return;
		}

		// check if next index in range
		if (currentIndex >= nodes.Count) currentIndex = 0;
		if (currentIndex == -1) currentIndex = nodes.Count-1;

		// reset color of all nodes
		foreach (GameObject n in nodes){
			n.GetComponent<Renderer> ().material.color = HighlightNode.highlightColor;
		}

		// highlight the node
		nodes [currentIndex].GetComponent<Renderer> ().material.color = Color.yellow;

	}

	// code for rotating object
	public void rotateTargetObject()
	{

		//graph.transform.localRotation = rot;
		if (checkButtonAStatus()) {
			startVibration();
			//mainCamera.transform.position = bubble.transform.position + (Vector3.forward * 100.0f);
			//mainCamera.transform.LookAt (bubble.transform);
			mainCamera.transform.SetParent (bubble.transform);
			bubble.transform.localRotation = rot;
		} else {
			stopVibration ();
			mainCamera.transform.parent = mainCameraParentTransform;
		}
	}
	private void saveSettings()
	{
		Settings settings = new Settings();
		settings.xDeg = xDeg;
		settings.yDeg = yDeg;
		settings.zDeg = zDeg;
		SaveLoad.Save(settings);
	}

	void OnGUI()
	{
		Event e = Event.current;
		if (e.type.Equals(EventType.KeyDown))
		{
			switch (e.keyCode)
			{
			case KeyCode.Q:
				xDeg++;
				break;
			case KeyCode.A:
				xDeg--;
				break;
			case KeyCode.W:
				yDeg++;
				break;
			case KeyCode.S:
				yDeg--;
				break;
			case KeyCode.E:
				zDeg++;
				break;
			case KeyCode.D:
				zDeg--;
				break;
			case KeyCode.R:
				saveSettings();
				break;
			case KeyCode.UpArrow:
				doRequest(127);
				break;
			case KeyCode.DownArrow:
				doRequest(0);
				break;
			}
		}
	}

	public void requestChangeOnOrientationStatus(bool status)
	{
		data = Encoding.ASCII.GetBytes((status ? -1.0f : -2.0f).ToString());
		sender.SendTo(data, target);
	}

	public void doRequest(float power)
	{
		try
		{
			if (power != lastSentPower)
			{
				data = Encoding.ASCII.GetBytes(((byte)power).ToString());
				sender.SendTo(data, target);
				lastSentPower = power;
			}
		}
		catch (UnityException e)
		{
			Debug.Log("EXCEPTION ******************************* " + e.Message);
		}
	}

	public class UDPReceiver
	{
		UdpClient client;
		IPEndPoint source;

		public void DoWork()
		{
			bool result;

			while (!_shouldStop)
			{
				try
				{
					if (client.Available >= 1)
					{
						byte[] data = client.Receive(ref source);

						if (data.Length == 16)
						{
							qw = (double)((((int)data[0] << 24) + ((int)data[1] << 16) + ((int)data[2] << 8) + data[3])) * (1.0 / (1 << 30));
							qx = (double)((((int)data[4] << 24) + ((int)data[5] << 16) + ((int)data[6] << 8) + data[7])) * (1.0 / (1 << 30));
							qy = (double)((((int)data[8] << 24) + ((int)data[9] << 16) + ((int)data[10] << 8) + data[11])) * (1.0 / (1 << 30));
							qz = (double)((((int)data[12] << 24) + ((int)data[13] << 16) + ((int)data[14] << 8) + data[15])) * (1.0 / (1 << 30));
							rot = Quaternion.Euler(xDeg, yDeg, zDeg) * new Quaternion((float)qx, (float)qy, (float)qz, (float)qw);
							//Debug.Log("IMU >>>>>> " + qx + " : "+ qy + " : "+ qz + " : " + qw);
						}
						else if (data.Length == 1)
						{
							result = Byte.TryParse(Encoding.UTF8.GetString(data), out lastCommandValue);
							/*
                            if (result)
                            {
                                switch (lastCommandValue)
                                {
                                    case 0: // BUTTON_DATA.BUTTONA_PRESSED:
                                        {
                                            break;
                                        }
                                    default:
                                        break;
                                }
                            }
                            */
						}
						else if (data.Length == 2)
						{
							///Debug.Log("J *** " + data[0] + " : " + data[1]);

							// simple code to skip fast events
							if (data[0] < 80 || data[0] > 240){

							}

							// code to set events
							if (data[0] < 80)
							{
								lastJoystickEventReceived = (byte)JOYSTICK_DATA.INCREASE;
							}
							else if (data[0] > 240)
							{
								lastJoystickEventReceived = (byte)JOYSTICK_DATA.DECREASE;
							}
							else
							{
								lastJoystickEventReceived = (byte)JOYSTICK_DATA.NONE;
							}
						}
					}
				}
				catch (Exception e)
				{
					Debug.Log("EXCEPTION ******************************* " + e.Message);
				}
			}
			client.Close();

			Debug.Log("worker thread: terminating gracefully.");
		}

		public void RequestStop()
		{
			_shouldStop = true;
			Debug.Log("requesting worker thread shutdown");
		}
		// Volatile is used as hint to the compiler that this data
		// member will be accessed by multiple threads.
		private volatile bool _shouldStop = false;

		public UDPReceiver(int port, int dllport)
		{
			//this.port = port;
			//this.dllport = dllport;
			client = new UdpClient(port);
			source = new IPEndPoint(IPAddress.Broadcast, dllPort);
			//client.Connect(source);
		}

	}

	/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public bool checkIncreaseJoystickEvent()
	{
		if (lastJoystickEventReceived == (byte)JOYSTICK_DATA.INCREASE)
		{
			//lastJoystickEventReceived = (byte)JOYSTICK_DATA.NONE;
			return true;
		}

		return false;
	}

	public bool checkDecreaseJoystickEvent()
	{
		if (lastJoystickEventReceived == (byte)JOYSTICK_DATA.DECREASE)
		{
			//lastJoystickEventReceived = (byte)JOYSTICK_DATA.NONE;
			return true;
		}

		return false;
	}

	public void startVibration()
	{
		doRequest(90);
	}

	public void stopVibration()
	{
		doRequest(0);
	}

	public bool checkButtonAStatus()
	{
		if (lastCommandValue == (byte)BUTTON_DATA.BUTTONA_PRESSED) return true;
		return false;
	}

	public bool checkButtonBStatus()
	{
		if (lastCommandValue == (byte)BUTTON_DATA.BUTTONB_PRESSED) return true;
		return false;
	}
	/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
