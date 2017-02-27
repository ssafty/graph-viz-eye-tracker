using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPythonCallByBat : MonoBehaviour {

    private string participant_name = "aaa";

    // Use this for initialization
    void Start1 () {
        try
        {

          

            System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
            myProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            myProcess.StartInfo.CreateNoWindow = true;
            myProcess.StartInfo.UseShellExecute = false;
            //myProcess.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";
            //string path = " %USERPROFILE%\\Documents\\call_python.bat %USERPROFILE%\\Documents\\" + this.participant_name + ".xml";
            //myProcess.StartInfo.Arguments = " /C " + path;
            myProcess.StartInfo.FileName = "C:\\Users\\praveenneuron\\Documents\\Documents\\call_python.bat";
            myProcess.StartInfo.Arguments = "C:\\Users\\praveenneuron\\Documents\\" + this.participant_name + ".xml";
            myProcess.EnableRaisingEvents = true;
            myProcess.Start();
            myProcess.WaitForExit();
            int ExitCode = myProcess.ExitCode;
            print(ExitCode);


        }
        catch (System.Exception e)
        {
            print(e);
        }
    }

    void Start()
    {
        int exitCode;
        System.Diagnostics.ProcessStartInfo processInfo;
        System.Diagnostics.Process process;


        processInfo = new System.Diagnostics.ProcessStartInfo("C:\\Windows\\system32\\cmd.exe", " /c " + " C:\\Users\\praveenneuron\\Documents\\call_python.bat " + " C:\\Users\\praveenneuron\\Documents\\" + this.participant_name + ".xml");
        //processInfo = new System.Diagnostics.ProcessStartInfo("C:\\Users\\praveenneuron\\Documents\\call_python.bat", " C:\\Users\\praveenneuron\\Documents\\" + this.participant_name + ".xml");

        processInfo.WorkingDirectory = "C:\\Users\\praveenneuron\\Documents\\";
        processInfo.CreateNoWindow = true;
        processInfo.UseShellExecute = false;
        // *** Redirect the output ***
        processInfo.RedirectStandardError = true;
        processInfo.RedirectStandardOutput = true;

        process = System.Diagnostics.Process.Start(processInfo);
        process.WaitForExit();

        // *** Read the streams ***
        // Warning: This approach can lead to deadlocks, see Edit #2
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        exitCode = process.ExitCode;

        Debug.Log("output>> " + output);
        Debug.Log("error>> " + error);
        Debug.Log("ExitCode>> " + error + exitCode.ToString());
        
        process.Close();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
