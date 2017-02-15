using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class createMarker : MonoBehaviour
{

	public float scale;
	public int rows, columns;

	public float offset = 1.0f;
	Vector2 SurfaceBottomLeft = Vector2.zero;
	Vector2 SurfaceTopRight = Vector2.zero;
	public Vector2 newScreen;

	// used for debugging in the UnityEditor
	public KeyCode reduceMarkerSize = KeyCode.F7;


	// Use this for initialization
	void Start ()
	{
		
	}


	public GameObject[] update_markers (string canvasTag, string markerTag, float wDist, float hDist)
	{
		List<GameObject> created = new List<GameObject> ();
		float screenWidth = Screen.width * wDist;
		float screenHeight = Screen.height * hDist;
		Debug.Log (screenWidth + "x" + screenHeight);

		float aspectRatio = screenHeight / screenWidth;
		float vStepsize = screenHeight / (rows - 1);
		float hStepsize = screenWidth / (columns - 1);
		int lastMarker = 0;
		GameObject[] markers = GameObject.FindGameObjectsWithTag (markerTag);

		GameObject marker_object = GameObject.Find (canvasTag);

		offset = offset * ((0.5f + scale) / (scale * 0.85f) - 0.8f);

		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < columns; j++) {
				float x = 0.0f;
				float y = 0.0f;
				Vector2 newPos = Vector2.zero;

				if (j == 0 || j == (columns - 1) || i == 0 || i == (rows - 1)) {
					x = ((hStepsize * j) - (screenWidth / 2));
					y = ((vStepsize * i) - (screenHeight / 2));

					markers [lastMarker].GetComponent<RectTransform> ().localScale = scale * markers [lastMarker].GetComponent<RectTransform> ().localScale;

					Rect rect = markers [lastMarker].GetComponent<RectTransform> ().rect;

					if (j == 0) {
						x += rect.width / offset;
					}
					if (i == 0) {
						y += rect.height / offset;
					}
					if (j == (columns - 1)) {
						x -= rect.width / offset;
					}
					if (i == (rows - 1)) {
						y -= rect.height / offset;
					}

					newPos = new Vector2 (x, y);
					markers [lastMarker].GetComponent<RectTransform> ().anchoredPosition = new Vector2 (x, y);

					if (newPos.x <= SurfaceBottomLeft.x && newPos.y <= SurfaceBottomLeft.y) {
						SurfaceBottomLeft = newPos; //- new Vector2 (rect.width, rect.height);
					}

					if (newPos.x >= SurfaceTopRight.x && newPos.y >= SurfaceTopRight.y) {
						SurfaceTopRight = newPos; //+ new Vector2 (rect.width, rect.height);
					}
						
					lastMarker++;
				}
			}
		}

		if (wDist != 1) {
			for (int i = 0; i < markers.Length; i++) {
				Vector2 new_pos = markers [i].GetComponent<RectTransform> ().anchoredPosition;
				GameObject duplicate = GameObject.Instantiate (markers [i]);
				duplicate.transform.parent = marker_object.transform;
				Vector2 pos_dup = new_pos + new Vector2 (Screen.width * 0.25f, 0f);
				new_pos -= new Vector2 (Screen.width * 0.25f, 0f);
				markers [i].GetComponent<RectTransform> ().anchoredPosition = new_pos;
				duplicate.GetComponent<RectTransform> ().anchoredPosition = pos_dup;
				created.Add (duplicate);
			}

			Rect inner_marker = GameObject.Find ("marker_1").GetComponent<RectTransform> ().rect;
			SurfaceBottomLeft -= new Vector2 (inner_marker.width / 2, inner_marker.height / 2);
			SurfaceTopRight += new Vector2 (inner_marker.width / 2, inner_marker.height / 2);

			newScreen = new Vector2 (SurfaceTopRight.x - SurfaceBottomLeft.x, SurfaceTopRight.y - SurfaceBottomLeft.y);
			Debug.Log (newScreen);
		}
		created.AddRange (markers);
		return created.ToArray ();
	}

	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (reduceMarkerSize)) {
			foreach (GameObject m in GameObject.FindGameObjectsWithTag ("marker")){
				m.transform.localScale -= new Vector3(0.05f,0.05f,0f);
			}
		}
	}
}
