using System;
using System.Collections.Generic;
using TouchScript.Gestures;
using TouchScript.Gestures.Clustered;
using UnityEngine;

namespace TouchScript.Behaviors
{
	/// <summary>
	/// Simple Component which transforms an object according to events from gestures.
	/// </summary>
	//[AddComponentMenu("TouchScript/Behaviors/PanCameraScript")]
	public class TransformCameraScript : MonoBehaviour
	{
		[System.Serializable]
		public class AdvancedParameters
		{
			public float startIPD = 0.06f;
			public float startHeight = 1.3f;
			public float startWidth = 0.7f;
			public Vector3 startHeadPos = new Vector3 (0, 0.5f, -0.35f);
			public float startHeadTrackingScale = 1.0f;
		}
		#region Public properties

		public ScreenTransformGesture ManipulationGesture;
		public Mesh mesh;
		public bool verbose = false;
		public Bounds bounds = new Bounds (Vector3.zero, new Vector3 (1000, 1000, 1000));
		public SetIPD ipd;
		public List<AsymFrustum> frustums = new List<AsymFrustum> ();
		public AdvancedParameters advancedParameters;
		public GameObject headNode;
		public GameObject tirc;
		public float panTreshold = 0.01f;
		public float scaleTreshold = 0.01f;
		public float rotateTreshold = 0.001f;

		#endregion
		
		#region Private variables
		
		private Vector3 localPositionToGo;
		private Vector3 rotationPoint;
		private float rotationAngle;
		private float trackIRScale;
		//private Vector3 scalePoint;
		private int numTouches;
		private Vector3 panInitPosition,panPosition;

		#endregion


		#region Unity methods

		private void Awake ()
		{
			setDefaults ();
		}
		
		public void Start ()
		{
			ipd = gameObject.GetComponentInChildren<SetIPD> ();
			frustums.AddRange (gameObject.GetComponentsInChildren<AsymFrustum> ());
			headNode.transform.localPosition = advancedParameters.startHeadPos;
			ipd.difference = advancedParameters.startIPD;
			foreach (AsymFrustum f in frustums) {
				f.width = advancedParameters.startWidth;
				f.height = advancedParameters.startHeight;
			}
			trackIRScale = advancedParameters.startHeadTrackingScale;

		}

		private void OnEnable() 
		{
			ManipulationGesture.Transformed += manipulationTransformedHandler;
		
		}
		
		private void OnDisable() 
		{
			ManipulationGesture.Transformed -= manipulationTransformedHandler;
		}

		private void manipulationTransformedHandler(object sender, System.EventArgs e)
		{
			rotationAngle = 0;
			ScreenTransformGesture gesture = sender as ScreenTransformGesture;
			numTouches = gesture.NumTouches;

			/**
			 *			PANNING: 
			 */
				if(gesture.State != Gesture.GestureState.Recognized){
					RaycastHit hPanInit;
				if (Physics.Raycast (Camera.main.ScreenPointToRay (new Vector3(gesture.PreviousScreenPosition.x,gesture.PreviousScreenPosition.y,0)), out hPanInit)) {
						if (verbose) {
							Graphics.DrawMesh (mesh, hPanInit.point, Quaternion.identity, null, 0);
						}
						panInitPosition = hPanInit.point;
					}
				}
					RaycastHit hPan;
					if (Physics.Raycast (Camera.main.ScreenPointToRay (new Vector3(gesture.ScreenPosition.x,gesture.ScreenPosition.y,0)), out hPan)) {
						if (verbose) {
							Graphics.DrawMesh (mesh, hPan.point, Quaternion.identity, null, 0);
						}
						panPosition = hPan.point;
					}
				Vector3 delta = (panPosition - panInitPosition) * -1;
				delta.Scale (new Vector3 (1, 0, 1));
			if(Math.Abs(gesture.DeltaPosition.magnitude) > panTreshold){
				localPositionToGo = Vector3Clamp (localPositionToGo + delta, bounds.min, bounds.max, 1, 0, 1);				
			}

			/**
			 *			ROTATION: 
			 */
			RaycastHit hRot;
			if (Physics.Raycast (Camera.main.ScreenPointToRay (new Vector3(gesture.ScreenPosition.x,gesture.ScreenPosition.y,0)), out hRot)) {
				if (verbose) {
					Graphics.DrawMesh (mesh, hRot.point, Quaternion.identity, null, 0);
				}
				rotationPoint = hRot.point;
			}

			if (Math.Abs (gesture.DeltaRotation) > rotateTreshold){
				rotationAngle = gesture.DeltaRotation;
			}else{
				rotationAngle = 0;
			}

			/**
			 *			SCALING: 
			 */
			/*
			Vector3 normScreenPos = new Vector3 (gesture.ScreenPosition.x, gesture.ScreenPosition.y);
			RaycastHit hScal;
			if (Physics.Raycast (Camera.main.ScreenPointToRay (normScreenPos), out hScal)) {
				if (verbose) {
					Graphics.DrawMesh (mesh, hScal.point, Quaternion.identity, null, 0);
				}
				scalePoint = hScal.point;
			}*/
			if (Math.Abs (gesture.DeltaScale - 1) > scaleTreshold) {
				setScaleValues (gesture.DeltaScale);
			}
		}
	
		private void Update ()
		{
			/// ROTATE
			transform.RotateAround(rotationPoint, Vector3.up, rotationAngle);
			rotationAngle = 0;
			/// PAN
			transform.localPosition = localPositionToGo;
			/// SCALE
			if (tirc != null) {
				tirc.GetComponent<SetLocalToWorldScale>().LocalToWorldScale = trackIRScale;
			}
			
			localPositionToGo = transform.localPosition;

		}
		
		#endregion
		
		#region Private functions
		
		private void setDefaults ()
		{
			localPositionToGo = transform.localPosition;
		}
		
		#endregion
		
		#region Event handlers

		private void setScaleValues (float deltaScale)
		{
			if (trackIRScale * deltaScale < bounds.max.y && trackIRScale * deltaScale > bounds.min.y) {
				
				trackIRScale *= deltaScale;
				ipd.difference *= deltaScale;
				foreach (AsymFrustum f in frustums) {
					f.width *= deltaScale;
					f.height *= deltaScale;
				}
			}
		}
		
		Vector3 Vector3Clamp (Vector3 value, Vector3 min, Vector3 max, int x = 1, int y = 1, int z = 1)
		{
			Vector3 ret = value;
			if (x != 0)
				ret.x = Mathf.Clamp (value.x, min.x, max.x);
			if (y != 0)
				ret.y = Mathf.Clamp (value.y, min.y, max.y);
			if (z != 0)
				ret.z = Mathf.Clamp (value.z, min.z, max.z);
			return ret;
		}
		#endregion
		void OnDrawGizmos ()
		{
			Gizmos.DrawWireCube (bounds.center, bounds.size);

			Gizmos.color = Color.red;

			Gizmos.color = Color.green;
			Gizmos.DrawLine(panInitPosition,panPosition);
		}
	}
}