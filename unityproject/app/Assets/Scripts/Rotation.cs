using UnityEngine;
using System.Collections;
using System;

public class Rotation : MonoBehaviour
{
    public float zoomSpeed = 3f;
    public float rotationSpeed = 1.5f;
    public float strafeSpeed = 0.6f;

    public float sensitivity = 0.1f;
    public float scrollSpeed = 4.0f;
    

    private void cursorMode(bool rightClick)

    {
        

        Cursor.visible = !rightClick;

        if (rightClick)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void Update()
    {
        bool rightClick = Input.GetMouseButton(1);
        cursorMode(rightClick);

    if(rightClick) { 
            Debug.Log(Input.GetAxis("Mouse X") + "    " + Input.GetAxis("Mouse Y"));
            transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime * 7, Space.World);
        }


        transform.Translate(new Vector3(strafeSpeed * Input.GetAxis("Horizontal2"), strafeSpeed * Input.GetAxis("Vertical2"), zoomSpeed * Input.GetAxis("Zoom") + scrollSpeed * Input.GetAxis("Scroll")), Camera.main.transform);



    }
}
