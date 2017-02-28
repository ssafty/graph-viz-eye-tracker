
#define USE_LEFT_EYE
//#define USE_RIGHT_EYE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;
using System.Collections;

public class CollectCalibStats : MonoBehaviour {


	//
	public const int FRAME_COUNT_FOR_STATS = 2000;

	// participant details
	private string participant_name;
	private string working_dir;

	// object to store all participant data
	private ParticipantStats participant_stats;

	// object that helps get calibrated information
	private AfterCalib3D afterCalib3D;

	public void init_game_for_collecting_statistics(string participant_name, string working_dir){

		this.participant_name = participant_name;
		this.working_dir = working_dir;
		this.afterCalib3D = GameObject.Find ("MainCamera").GetComponent<AfterCalib3D>();
	}


	// Update is called once per frame
	void Update () {
		if (!this.afterCalib3D.enabled)
			return;


	}




	[XmlRoot("root")]
	public class ParticipantStats
	{

		[XmlAttribute("name")]
		public string name;


		[XmlArrayItem("ball_x")]
		public float[] ball_x = new float[CollectCalibStats.FRAME_COUNT_FOR_STATS];
		[XmlArrayItem("ball_y")]
		public float[] ball_y = new float[CollectCalibStats.FRAME_COUNT_FOR_STATS];
		[XmlArrayItem("ball_z")]
		public float[] ball_z = new float[CollectCalibStats.FRAME_COUNT_FOR_STATS];

		[XmlArrayItem("eye_x_with_calib")]
		public float[] eye_x_with_calib = new float[CollectCalibStats.FRAME_COUNT_FOR_STATS];
		[XmlArrayItem("eye_y_with_calib")]
		public float[] eye_y_with_calib = new float[CollectCalibStats.FRAME_COUNT_FOR_STATS];

		[XmlArrayItem("eye_x_without_calib")]
		public float[] eye_x_without_calib = new float[CollectCalibStats.FRAME_COUNT_FOR_STATS];
		[XmlArrayItem("eye_y_without_calib")]
		public float[] eye_y_without_calib = new float[CollectCalibStats.FRAME_COUNT_FOR_STATS];

	}

}
