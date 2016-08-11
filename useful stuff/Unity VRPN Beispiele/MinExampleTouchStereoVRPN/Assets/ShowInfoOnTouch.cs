using UnityEngine;
using System.Collections;
using TouchScript;

public class ShowInfoOnTouch : MonoBehaviour {

    [SerializeField]
    bool on = false;
    [SerializeField]
    GameObject infocanvas;
    TouchScript.Gestures.LongPressGesture gesture;
    

	// Use this for initialization
	void Start () {
        gesture = GetComponent<TouchScript.Gestures.LongPressGesture>();
        gesture.LongPressed += Gesture_LongPressed;
	}

    private void Gesture_LongPressed(object sender, System.EventArgs e)
    {
        on = !on;
        infocanvas.SetActive(on);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
