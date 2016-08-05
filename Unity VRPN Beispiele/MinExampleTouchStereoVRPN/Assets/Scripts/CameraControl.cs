using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class CameraControl : MonoBehaviour {

	public bool isTopViewm;
	public bool isRightViewm;

	//private bool isTopView;
	//private bool isRightView;

	public float upillaryDistanceInMil = 65;

	// Use this for initialization
	void Start () {
		float halfUpillaryDistanceInM = upillaryDistanceInMil / 2f * 0.001f;
		if (isRightViewm) {
			// camera was left eye and we want to change it to the right eye:
			//camera.transform.position = camera.transform.position + camera.transform.right*halfUpillaryDistanceInM;//new Vector3 (halfUpillaryDistanceInM, 0, 0);
			//camera.transform.position = camera.transform.position + 
			GetComponent<Camera>().transform.Translate(GetComponent<Camera>().transform.right*halfUpillaryDistanceInM);
		} else {
			GetComponent<Camera>().transform.Translate(GetComponent<Camera>().transform.right*halfUpillaryDistanceInM*-1f);
			//camera.transform.position = camera.transform.position + camera.transform.right*(-halfUpillaryDistanceInM);//new Vector3 (-halfUpillaryDistanceInM, 0, 0);
		}
	}

	void FixedUpdate()
	{
	}

	// Update is called once per frame
	void Update () {
		//inverse camera top when i is pressed and bottom when k is
		if (Input.GetKeyDown (KeyCode.I)&&isTopViewm) {
			
			isRightViewm = !isRightViewm;
			setCameraPosition(isRightViewm);
		}

		else if (Input.GetKeyDown (KeyCode.K)&&!isTopViewm) {			
			isRightViewm = !isRightViewm;
			setCameraPosition(isRightViewm);
		}
	}
	
	/// <summary>
	/// Sets the camera position according to isTopView, isRightView and uppiliaryDistanceInMil
	/// </summary>
	/// 
	void setCameraPosition(bool isRightView) {
		//float halfUpillaryDistanceInM = upillaryDistanceInMil / 2f * 0.001f;
		float upillaryDistanceInM = upillaryDistanceInMil * 0.001f;
		if (isRightView) {
			// camera was left eye and we want to change it to the right eye:
			GetComponent<Camera>().transform.position = GetComponent<Camera>().transform.position + GetComponent<Camera>().transform.right*upillaryDistanceInM;//new Vector3 (halfUpillaryDistanceInM, 0, 0);
				} else {
			GetComponent<Camera>().transform.position = GetComponent<Camera>().transform.position + GetComponent<Camera>().transform.right*(-upillaryDistanceInM);//new Vector3 (-halfUpillaryDistanceInM, 0, 0);
				}
	}

}
