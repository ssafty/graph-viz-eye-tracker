using UnityEngine;
using System.Collections;

public class Bounce : MonoBehaviour {

    // Use this for initialization
    [SerializeField]
    AnimationCurve curve;
    [SerializeField]
    float bouncetime = 1.0f;
    [SerializeField]
    bool on = false;

    [SerializeField]
    Transform target;

    private Vector3 startscale;

    TouchScript.Gestures.LongPressGesture gesture;


    // Use this for initialization
    void Start()
    {
        gesture = GetComponent<TouchScript.Gestures.LongPressGesture>();
        gesture.LongPressed += Gesture_LongPressed;
        startscale = target.localScale;
    }
    private void Gesture_LongPressed(object sender, System.EventArgs e)
    {
        on = !on;
        StartCoroutine("DoBounce", !on);
    }
    // Update is called once per frame
    void Update () {
 
    }
    IEnumerator DoBounce(bool turnoff)
    {
        float timeleft = bouncetime;
        
        while(timeleft>0)
        {
            
            float factor = (!turnoff)? 1.0f - timeleft / bouncetime : timeleft / bouncetime;
            float scaleval = curve.Evaluate(factor);
            target.localScale = startscale * scaleval;
            timeleft -= Time.deltaTime;
            yield return null;
        }        
    }
}
