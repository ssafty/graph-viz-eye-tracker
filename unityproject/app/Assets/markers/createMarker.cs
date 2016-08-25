using UnityEngine;
using System.Collections;

public class createMarker : MonoBehaviour {


    public int rows, columns;
    private GameObject[] markers;
	public float offset = 1.0f;
	Vector2 SurfaceBottomLeft = Vector2.zero;
	Vector2 SurfaceTopRight = Vector2.zero;
	public Vector2 newScreen;
    // Use this for initialization
    void Start() {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
		Debug.Log (screenWidth + "x" + screenHeight);
        //screenWidth = 3840; 797
        //screenHeight = 2160; 393
        float aspectRatio = screenHeight / screenWidth;
        float vStepsize = screenHeight / (rows-1);
        float hStepsize = screenWidth / (columns-1);
        int lastMarker = 0;
        markers = GameObject.FindGameObjectsWithTag("marker");

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                float x = 0.0f;
                float y = 0.0f;
                Vector2 newPos = Vector2.zero;

                if (j == 0 || j == (columns-1) || i == 0 || i == (rows-1))
                {
                    x = ((hStepsize * j) - (screenWidth / 2));
                    y = ((vStepsize * i) - (screenHeight / 2));

					Rect rect = markers[lastMarker].GetComponent<RectTransform> ().rect;

					if (j == 0) { x += rect.width / offset; }
					if (i == 0) { y += rect.height / offset; }
					if (j == (columns-1))   { x -= rect.width / offset; }
					if (i == (rows-1))      { y -= rect.height / offset; }

                    newPos = new Vector2(x, y);
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

		Rect inner_marker = GameObject.Find ("marker_1").GetComponent<RectTransform> ().rect;
		SurfaceBottomLeft -= new Vector2 (inner_marker.width/2, inner_marker.height/2);
		SurfaceTopRight += new Vector2 (inner_marker.width/2, inner_marker.height/2);

		newScreen = new Vector2 (SurfaceTopRight.x - SurfaceBottomLeft.x, SurfaceTopRight.y - SurfaceBottomLeft.y);
		Debug.Log (newScreen);
	}

	
	// Update is called once per frame
	void Update () {
		
	}
}
