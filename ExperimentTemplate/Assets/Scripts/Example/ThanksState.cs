using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class ThanksState : ExperimentState {

    [SerializeField]
    Canvas c;
    [SerializeField]
    Text text;

    public override ExperimentState HandleInput(ExperimentController ec)
    {
        if(Input.GetKeyDown(KeyCode.Z))
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
        c.gameObject.SetActive(true);
        text.text = "Thanks for participating";
    }
}
