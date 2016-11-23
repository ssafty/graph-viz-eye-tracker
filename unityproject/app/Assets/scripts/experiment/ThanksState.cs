using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class ThanksState : ExperimentState {

    [SerializeField]
    Canvas c;
    [SerializeField]
    Text text;
    [SerializeField]
    Button nextButton;
	[SerializeField]
	GameObject marker;
	[SerializeField]
	GameObject eyepointer;
    public override ExperimentState HandleInput(ExperimentController ec)
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
            #else
                     Application.Quit();
            #endif
        }
        return this;
    }

    public override void UpdateState(ExperimentController ec)
    {
		marker.gameObject.SetActive(false);
		eyepointer.gameObject.SetActive(false);
        c.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(false);
        text.text = "Thanks for participating. Press X to close.";
    }
}
