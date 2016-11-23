using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class ThanksState : ExperimentState {

    [SerializeField]
    Canvas c;
    [SerializeField]
    GameObject panel;
    [SerializeField]
    Text text;
    [SerializeField]
    Button nextButton;
	[SerializeField]
	GameObject marker;
	[SerializeField]
	GameObject eyepointer;
    [SerializeField]
    GameObject graph;
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
        graph.gameObject.SetActive(false);
        c.gameObject.SetActive(true);
        panel.gameObject.SetActive(true);
        text.text = "Thanks for participating. Press X to close.";
    }
}
