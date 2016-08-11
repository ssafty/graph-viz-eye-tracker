using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode]
public class AsymFrustumOld : MonoBehaviour
{
    /// <summary>
    /// No function yet. Potentially usable to allow the table to be movable in scene coordinates.
    /// </summary>
//    public GameObject table = null;


	public GameObject knubbel;

    /// <summary>
    /// Screen/window to virtual world width
    /// </summary>
    public float width;
    /// <summary>
    /// Screen/window to virtual world height
    /// </summary>
    public float height;
    
    public float maxHeight = 2000.0f;
    float windowWidth;
    float windowHeight;
    //public Vector3 HeadScreenCoord = Vector3.zero;
	private Quaternion startRot;


	public bool verbose = false;

    // Use this for initialization
    void Start()
    {
		startRot = transform.rotation;
    }

	/// <summary>
	/// Fixeds the update.
	/// </summary>
    void LateUpdate()
    {
		windowWidth = width;
		windowHeight = height;
//        Debug.Log("Window height "+ Screen.height);
//        Debug.Log("Screen resolution height"+ Screen.currentResolution.height);
        //Debug.Log("New window width "+ windowWidth );
//        Debug.Log("New window height "+ windowHeight);
		//transform.rotation = startRot;
        //Camera cam = camera; //TODO get proper camera positions
		//setAsymmetricFrustum(camera, camera.transform.position,camera.nearClipPlane);

		Vector3 localPos = knubbel.transform.InverseTransformPoint (transform.position);
		//Debug.LogError (gameObject.name+" campos.x: " + camera.transform.position + " localPos " + localPos);

		setAsymmetricFrustum(GetComponent<Camera>(), localPos,GetComponent<Camera>().nearClipPlane);


		//table.transform.
    }
    /// <summary>
    /// Sets the asymmetric Frustum for the given Table (at pos 0,0,0 )
    /// and the camera passed
    /// </summary>
    /// <param name="cam">Camera to get the asymmetric frustum for</param>
    /// <param name="pos">Position of the camera. Usually cam.transform.position</param>
    /// <param name="nearDist">Near clipping plane, usually cam.nearClipPlane</param>
    public void setAsymmetricFrustum(Camera cam,Vector3 pos, float nearDist)
    {

        // Focal length = orthogonal distance to image plane
		Vector3 newpos = pos;
		//newpos.Scale (new Vector3 (1, 1, 1));
        float focal = 1;
		newpos = new Vector3 (newpos.x, newpos.z, newpos.y);
		if (verbose) 
		{
			Debug.Log (newpos.x+";"+newpos.y+";"+newpos.z);
		}

		focal = Mathf.Clamp(newpos.z,0.001f, maxHeight);

        // Ratio for intercept theorem
        float ratio = focal / nearDist;

        // Compute size for focal
        float imageLeft = (-windowWidth / 2.0f) - newpos.x;
        float imageRight = (windowWidth / 2.0f) - newpos.x;
        float imageTop = (windowHeight / 2.0f) - newpos.y;
        float imageBottom = (-windowHeight / 2.0f) - newpos.y;

        // Intercept theorem
        float nearLeft = imageLeft / ratio;
        float nearRight = imageRight / ratio;
		float nearTop = imageTop / ratio;
		float nearBottom = imageBottom / ratio;

        Matrix4x4 m = PerspectiveOffCenter(nearLeft, nearRight, nearBottom, nearTop, cam.nearClipPlane, cam.farClipPlane);
        cam.projectionMatrix = m;
    }


    // Set an off-center projection, where perspective's vanishing
    // point is not necessarily in the center of the screen.
    //
    // left/right/top/bottom define near plane size, i.e.
    // how offset are corners of camera's near plane.
    // Tweak the values and you can see camera's frustum change.
    Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
    {
        float x = (2.0f * near) / (right - left);
        float y = (2.0f * near) / (top - bottom);
        float a = (right + left) / (right - left);
        float b = (top + bottom) / (top - bottom);
        float c = -(far + near) / (far - near);
        float d = -(2.0f * far * near) / (far - near);
        float e = -1.0f;

        Matrix4x4 m = new Matrix4x4();
        m[0, 0] = x; m[0, 1] = 0; m[0, 2] = a; m[0, 3] = 0;
        m[1, 0] = 0; m[1, 1] = y; m[1, 2] = b; m[1, 3] = 0;
        m[2, 0] = 0; m[2, 1] = 0; m[2, 2] = c; m[2, 3] = d;
        m[3, 0] = 0; m[3, 1] = 0; m[3, 2] = e; m[3, 3] = 0;
        return m;
    }
    /// <summary>
    /// Draws gizmos in the Edit window.
    /// TODO: Add complete frustum.
    /// </summary>
    public virtual void OnDrawGizmos()
    {
		Gizmos.DrawLine (GetComponent<Camera>().transform.position, GetComponent<Camera>().transform.position+GetComponent<Camera>().transform.up * 10);
		Gizmos.DrawLine(GetComponent<Camera>().transform.position, GetComponent<Camera>().transform.position + GetComponent<Camera>().transform.up * 10);
		
		Gizmos.color = Color.green;
		Gizmos.DrawLine(knubbel.transform.position, knubbel.transform.position + knubbel.transform.up);
		
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(knubbel.transform.position - knubbel.transform.forward * 0.5f * windowHeight, knubbel.transform.position + knubbel.transform.forward * 0.5f * windowHeight);
		
		Gizmos.color = Color.red;
		Gizmos.DrawLine(knubbel.transform.position - knubbel.transform.right * 0.5f * windowWidth, knubbel.transform.position + knubbel.transform.right * 0.5f * windowWidth);


	}
}