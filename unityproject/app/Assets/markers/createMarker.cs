using UnityEngine;
using System.Collections;

public class createMarker : MonoBehaviour {


    public int rows, columns;
    private GameObject[] markers;
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
		float offset = 1.3f;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                float x = 0.0f;
                float y = 0.0f;
                Vector3 newPos = Vector3.zero;

                if (j == 0 || j == (columns-1) || i == 0 || i == (rows-1))
                {
                    x = ((hStepsize * j) - (screenWidth / 2));
                    y = ((vStepsize * i) - (screenHeight / 2));

					Rect rect = markers[lastMarker].GetComponent<RectTransform> ().rect;
					//Debug.Log (rectPosition);

					if (j == 0) { x += rect.width / offset; }
					if (i == 0) { y += rect.height / offset; }
					if (j == (columns-1))   { x -= rect.width / offset; }
					if (i == (rows-1))      { y -= rect.height / offset; }

                    newPos = new Vector3(x, y, 5);
					Debug.Log (newPos);
					markers [lastMarker].GetComponent<RectTransform> ().anchoredPosition = new Vector2 (x, y);
                    //markers[lastMarker].transform.position = newPos;
                    lastMarker++;
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
