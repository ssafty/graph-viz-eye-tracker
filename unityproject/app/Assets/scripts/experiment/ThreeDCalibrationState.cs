using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ThreeDCalibrationState : ExperimentState
{


    bool next = false;
    [SerializeField]
    Text text;
    [SerializeField]
    GameObject marker;
    [SerializeField]
    GameObject eyepointer;
    [SerializeField]
    GameObject c;
    [SerializeField]
    GameObject panel;
    [SerializeField]
    KeyCode skip = KeyCode.End;
    private bool init = false;
    public bool Next
    {
        get
        {
            return next;
        }

        set
        {
            next = value;
        }
    }

    public override ExperimentState HandleInput(ExperimentController ec)
    {
        if (Next || Input.GetKeyDown(skip))
        {
            panel.gameObject.SetActive(false);
            c.gameObject.SetActive(false);
            return nextState;
        }
        else
        {
            return this;
        }
    }
    private void initialize()
    {
        panel.gameObject.SetActive(true);
        c.gameObject.SetActive(true);
        //eyepointer.gameObject.SetActive(false);
        experimentLogger.getLogger().currentState = "WaitingFor3DCalibration";
        text.text = "The second phase of calibration will start in 10 seconds.\n You will see many green nodes. \n Please focus the one which turns purple/red. \n Follow the colored nodes with your eyes.";
        Invoke("start3DCalib", 2);
    }
    private void start3DCalib()
    {
        panel.gameObject.SetActive(false);
        c.gameObject.SetActive(false);
        experimentLogger.getLogger().currentState = "3DCalibrationStarted";
        Calib3D calib = GameObject.FindGameObjectWithTag("Calib3D").GetComponent<Calib3D>();
        calib.StartCalib3DScene();
        calib.enable_calib_3D = true;
        calib.invokeCalib();
        Invoke("start3DCalibForReal", 1);
        
    }
    private void start3DCalibForReal()
    {
        Calib3D calib = GameObject.FindGameObjectWithTag("Calib3D").GetComponent<Calib3D>();
        calib.startCalibForReal();
    }
    public override void UpdateState(ExperimentController ec)
    {
        if(!init)
        {
            init = true;
            initialize();
            
        }
    }
}
