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

	public GameObject goalNode;
	private int currentIndex = -1;
	private bool joyNeutral = true;
	//Has the joystick been neutral in the meantime
	private bool bNeutral = true;
	//Has the joystick been neutral in the meantime
	private Vector3 lastRotRing;
	private Vector3 lastRotGraph;
	private Quaternion lastRingQuad;
	private Quaternion lastGraphQuad;

	public static String lastPressed = "";

	//private Vector3 startPosition;

	//History stuff. Stacks would be simpler thatn lists
	private Stack<Vector3> camPosHistory;
	private Stack<Quaternion> camRotHistory;

	public GameObject pivot;

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

	public static byte pressureThreshold = 127;
	public static bool isTipPressed = false;
	public static float tipPressCounter = 0f;
	public static float zDeg = 0;

	Coroutine c;
	Thread workerThread;



	public Camera mainCamera;
	public GameObject gameController;
	private HapringController hapringController;
	private Transform mainCameraParentTransform;

	void Awake ()
	{
		sender = new Socket (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		target = new IPEndPoint (IPAddress.Parse (serverIP), dllPort);
		Debug.Log ("UDP sender ready!");

		workerObject = new UDPReceiver (unityPort, dllPort);
		workerThread = new Thread (workerObject.DoWork);

		workerThread.Start ();
		Debug.Log ("main thread: Starting worker thread...");

		while (!workerThread.IsAlive)
			;

		Debug.Log ("UDP receiver ready!");

		doRequest (0);
		requestChangeOnOrientationStatus (true);

		Settings settings = SaveLoad.Load ();
		xDeg = settings.xDeg;
		yDeg = settings.yDeg;
		zDeg = settings.zDeg;

		lastRotRing = rot.eulerAngles;
		lastRotGraph = pivot.transform.rotation.eulerAngles;
		lastGraphQuad = pivot.transform.rotation;
		lastRingQuad = rot;

		camPosHistory = new Stack<Vector3> ();
		camRotHistory = new Stack<Quaternion> ();
	}

	void OnDestroy ()
	{
		doRequest (0);
		Debug.Log ("worker thread stopping");

		workerObject.RequestStop ();
		workerThread.Join ();
		try {
			sender.Shutdown (SocketShutdown.Both);
			sender.Close ();
		} catch (Exception e) {
			Debug.Log (e);
		}

		Debug.Log ("sender destroyed");
	}

	void Start ()
	{

		if (mainCamera == null) {
			Debug.LogError ("Please attach main camera to this script!!!");
		} else {
			mainCameraParentTransform = mainCamera.transform.parent;
		}
		hapringController = gameController.GetComponent<HapringController> ();
	}

	private float lastBClick = 0f;

	void Update ()
	{
		// select the nodes
		nodeSelection ();

		// rotate around bubble
		rotateTargetObject ();

		if (checkButtonBStatus ()) {
			GoToBubble.Zoom ();
		}




	}
	// code for selecting the nodes
	void nodeSelection ()
	{
		if (checkNoJoystickEvent ()) {	
			joyNeutral = true;
		}

		if (checkIncreaseJoystickEvent () && joyNeutral) {
			hapringController.switchNode (HapringController.Direction.left);
			lastPressed = "HAPRING_JOYSTICK_UP";
			joyNeutral = false;
		} else if (checkDecreaseJoystickEvent () && joyNeutral) {
			hapringController.switchNode (HapringController.Direction.right);
			lastPressed = "HAPRING_JOYSTICK_DOWN";
			joyNeutral = false;
		}



		if (tipPressCounter > 11) {
			
			startVibration ();
			hapringController.switchNode (HapringController.Direction.select);
			//	Invoke ("stopVibration", 1.0f);
			tipPressCounter = 0;
		}
		if (tipPressCounter > 0f) {
			tipPressCounter -= 0.025f;
		}
	}



	// code for rot	ating object
	public void rotateTargetObject ()
	{

		if (checkButtonAStatus ()) {
			mainCamera.transform.SetParent (pivot.transform);
			Quaternion diff = Quaternion.Inverse (lastRingQuad) * rot;
			pivot.transform.rotation = lastGraphQuad * diff;

		} else {
			stopVibration ();
			mainCamera.transform.parent = mainCameraParentTransform;
			lastRotRing = rot.eulerAngles;
			lastRotGraph = pivot.transform.rotation.eulerAngles;
			lastGraphQuad = pivot.transform.rotation;
			lastRingQuad = rot;
		}
	}

	private void saveSettings ()
	{
		Settings settings = new Settings ();
		settings.xDeg = xDeg;
		settings.yDeg = yDeg;
		settings.zDeg = zDeg;
		SaveLoad.Save (settings);
	}

	void OnGUI ()
	{
		Event e = Event.current;
		if (e.type.Equals (EventType.KeyDown)) {
			switch (e.keyCode) {
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
				saveSettings ();
				break;
			case KeyCode.UpArrow:
				doRequest (127);
				break;
			case KeyCode.DownArrow:
				doRequest (0);
				break;
			}
		}
	}

	public void requestChangeOnOrientationStatus (bool status)
	{
		data = Encoding.ASCII.GetBytes ((status ? -1.0f : -2.0f).ToString ());
		sender.SendTo (data, target);
	}

	public void doRequest (float power)
	{
		try {
			if (power != lastSentPower) {
				data = Encoding.ASCII.GetBytes (((byte)power).ToString ());
				sender.SendTo (data, target);
				lastSentPower = power;
			}
		} catch (UnityException e) {
			Debug.Log ("EXCEPTION ******************************* " + e.Message);
		}
	}


	public class UDPReceiver
	{
		UdpClient client;
		IPEndPoint source;

		public void DoWork ()
		{
			bool result;

			while (!_shouldStop) {
				try {
					if (client.Available >= 1) {
						byte[] data = client.Receive (ref source);

						if (data.Length == 16) {

							qw = (double)((((int)data [0] << 24) + ((int)data [1] << 16) + ((int)data [2] << 8) + data [3])) * (1.0 / (1 << 30));
							qx = (double)((((int)data [4] << 24) + ((int)data [5] << 16) + ((int)data [6] << 8) + data [7])) * (1.0 / (1 << 30));
							qy = (double)((((int)data [8] << 24) + ((int)data [9] << 16) + ((int)data [10] << 8) + data [11])) * (1.0 / (1 << 30));
							qz = (double)((((int)data [12] << 24) + ((int)data [13] << 16) + ((int)data [14] << 8) + data [15])) * (1.0 / (1 << 30));
							rot = Quaternion.Euler (xDeg, yDeg, zDeg) * new Quaternion ((float)qx, (float)qy, (float)qz, (float)qw);
							//Debug.Log("IMU >>>>>> " + qx + " : "+ qy + " : "+ qz + " : " + qw);
						} else if (data.Length == 1) {
							tipPressCounter = 0;
							result = Byte.TryParse (Encoding.UTF8.GetString (data), out lastCommandValue);

						} else if (data.Length == 2) {
							// code to set events
							if (data [0] < 80) {
								lastJoystickEventReceived = (byte)JOYSTICK_DATA.INCREASE;
							} else if (data [0] > 240) {
								lastJoystickEventReceived = (byte)JOYSTICK_DATA.DECREASE;
							} else {
								lastJoystickEventReceived = (byte)JOYSTICK_DATA.NONE;
							}
						} else if (data.Length == 3) {
							if((data [2] > pressureThreshold)) {
								tipPressCounter++;
								lastPressed = "HAPRING_TIP";
							} else {
								tipPressCounter = 0;
							}
						}
					}
				} catch (Exception e) {
					Debug.Log ("EXCEPTION ******************************* " + e.Message);
				}
			}
			client.Close ();

			Debug.Log ("worker thread: terminating gracefully.");
		}

		public void RequestStop ()
		{
			_shouldStop = true;
			Debug.Log ("requesting worker thread shutdown");
		}
		// Volatile is used as hint to the compiler that this data
		// member will be accessed by multiple threads.
		private volatile bool _shouldStop = false;

		public UDPReceiver (int port, int dllport)
		{
			//this.port = port;
			//this.dllport = dllport;
			client = new UdpClient (port);
			source = new IPEndPoint (IPAddress.Broadcast, dllPort);
			//client.Connect(source);
		}

	}

	/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public bool checkIncreaseJoystickEvent ()
	{
		if (lastJoystickEventReceived == (byte)JOYSTICK_DATA.INCREASE) {
			//lastJoystickEventReceived = (byte)JOYSTICK_DATA.NONE;
			return true;
		}
	
		return false;
	}

	public bool checkDecreaseJoystickEvent ()
	{
		if (lastJoystickEventReceived == (byte)JOYSTICK_DATA.DECREASE) {
			//lastJoystickEventReceived = (byte)JOYSTICK_DATA.NONE;
			return true;
		}

		return false;
	}

	public bool checkNoJoystickEvent ()
	{
		if (lastJoystickEventReceived == (byte)JOYSTICK_DATA.NONE) {
			//lastJoystickEventReceived = (byte)JOYSTICK_DATA.NONE;
			return true;
		}

		return false;
	}

	public void startVibration ()
	{
		doRequest (90);
	}

	public void stopVibration ()
	{
		doRequest (0);
	}

	public bool checkButtonAStatus ()
	{
		if (lastCommandValue == (byte)BUTTON_DATA.BUTTONA_PRESSED) {
			lastPressed = "HAPRING_A";
			return true;
		}
		return false;
	}

	public bool checkButtonBStatus ()
	{
		if (lastCommandValue == (byte)BUTTON_DATA.BUTTONB_PRESSED) {
			lastPressed = "HAPRING_B";
			return true;
		}
		return false;
	}
	/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}

