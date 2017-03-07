using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Security.Policy;
using System.CodeDom.Compiler;

public class TrainingState : ExperimentState
{
	GameObject c;
	[SerializeField]
	GameObject panel;
	[SerializeField]
	Text text;
	[SerializeField]
	GameObject marker;
	[SerializeField]
	GameObject eyepointer;
	[SerializeField]
	GameObject graph;
	[SerializeField]
	GameObject GameController;
	[SerializeField]
	ExperimentState PupilCalibration;
	[SerializeField]
	ExperimentState CustomCalibration;
	[SerializeField]
	ExperimentState instructionState;

	private int indexToUseForTraining = 0;
	private bool first = true;
	private bool PupilCalibrationDone = false;
	private int lastTraining = -1;
	MoveToExperimentTrial mttrial;

	public override ExperimentState HandleInput (ExperimentController ec)
	{
		ec.drawGraph = false;
		if (first) {
			first = false;
			return instructionState;
		}
		if (lastTraining != ec.currentTrialIndex) {
			indexToUseForTraining = ec.currentTrialIndex;
			lastTraining = ec.currentTrialIndex;
			Debug.Log ("using graph " + indexToUseForTraining + " for training");
			MoveToExperimentTrial mttrialTemp = ec.CurrentTrials [ec.CurrentTrialIndex] as MoveToExperimentTrial;
			mttrial = new MoveToExperimentTrial (ec.CurrentTrialIndex, new Graph (mttrialTemp.Graph.Name, mttrialTemp.Graph.NumNodes, mttrialTemp.Graph.NumberHighlightedNodes, mttrialTemp.Graph.BubbleSize, mttrialTemp.Graph.ExperimentType));

		}
		if (!PupilCalibrationDone && mttrial.Graph.ExperimentType != experimentType.MOUSE) {
			Debug.Log ("going to pupil calibration");

			if (mttrial.Graph.ExperimentType == experimentType.WITHCUSTOMCALIB || mttrial.Graph.ExperimentType == experimentType.EYE) {
				PupilCalibrationDone = true;
				return PupilCalibration;
			}
		} else {
			ec.drawGraph = true;
		}
		if ((indexToUseForTraining - ec.CurrentTrialIndex) > ec.NumberOfTrainings) {
			Debug.Log ("Training done. Going to Trial Phase");
			first = true;
			return nextState;
		} else {
			return this;
		}
	}

	public override void UpdateState (ExperimentController ec)
	{

		if (ec.drawGraph == true) {
			experimentLogger.getLogger ().currentState = "Training";
			marker.gameObject.SetActive (true);
			eyepointer.gameObject.SetActive (true);
			graph.gameObject.SetActive (true);

			if (mttrial != null) {

				mttrial.initialze (GameController);

				mttrial.update ();
				if (mttrial.done) {
					GameObject[] nodes = GameObject.FindGameObjectsWithTag ("Node");
					foreach (GameObject node in nodes) {
						GameObject.Destroy (node);
					}
					indexToUseForTraining++;
				}
			} else {
				throw new UnityException ("couldn't cast trial as MoveToExperimentTrial");
			}
		}
	}
}
