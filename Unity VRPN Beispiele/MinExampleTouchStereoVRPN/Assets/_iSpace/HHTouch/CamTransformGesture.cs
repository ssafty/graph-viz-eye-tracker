/*
 * @author Valentin Simonov / http://va.lent.in/
 */

using System.Collections.Generic;
using TouchScript.Gestures.Base;
using TouchScript.Layers;
using TouchScript.Utils;
#if TOUCHSCRIPT_DEBUG
using TouchScript.Utils.Debug;
#endif
using UnityEngine;

namespace TouchScript.Gestures
{
    /// <summary>
    /// Recognizes a transform gesture, i.e. translation, rotation, scaling or a combination of these.
    /// </summary>
    [HelpURL("http://touchscript.github.io/docs/html/T_TouchScript_Gestures_TransformGesture.htm")]
    public class CamTransformGesture : TransformGestureBase, ITransformGesture
    {
        #region Constants

        /// <summary>
        /// Transform's projection type.
        /// </summary>
        public enum ProjectionType
        {
            /// <summary>
            /// Use a plane with normal vector defined by layer.
            /// </summary>
            Layer,

            /// <summary>
            /// Use a plane with certain normal vector in local coordinates.
            /// </summary>
            Object,

            /// <summary>
            /// Use a plane with certain normal vector in global coordinates.
            /// </summary>
            Global,
        }
        [System.Serializable]
        public class AdvancedParameters
        {
            public float startIPD = 0.06f;
            public float startHeight = 1.3f;
            public float startWidth = 0.7f;
            public Vector3 startHeadPos = new Vector3(0, 0.5f, -0.35f);
            public float startHeadTrackingScale = 1.0f;

        }
        public AdvancedParameters advancedParameters;
        public List<AsymFrustum> frustums = new List<AsymFrustum>();
        public Bounds bounds = new Bounds(Vector3.zero, new Vector3(1000, 1000, 1000));
        public float cameraScale = 1.0f;
        public SetIPD ipd;
        public Camera touchCam;
        public SetLocalToWorldScale scaler;
        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets transform's projection type.
        /// </summary>
        /// <value> Projection type. </value>
        public ProjectionType Projection
        {
            get { return projection; }
            set
            {
                if (projection == value) return;
                projection = value;
                if (Application.isPlaying) updateProjectionPlane();
            }
        }

        /// <summary>
        /// Gets or sets transform's projection plane normal.
        /// </summary>
        /// <value> Projection plane normal. </value>
        public Vector3 ProjectionPlaneNormal
        {
            get
            {
                if (projection == ProjectionType.Layer) return projectionLayer.WorldProjectionNormal;
                return projectionPlaneNormal;
            }
            set
            {
                if (projection == ProjectionType.Layer) projection = ProjectionType.Object;
                value.Normalize();
                if (projectionPlaneNormal == value) return;
                projectionPlaneNormal = value;
                if (Application.isPlaying) updateProjectionPlane();
            }
        }

        /// <summary>
        /// Plane where transformation occured.
        /// </summary>
        public Plane TransformPlane
        {
            get { return transformPlane; }
        }

        /// <summary>
        /// Gets delta position in local coordinates.
        /// </summary>
        /// <value>Delta position between this frame and the last frame in local coordinates.</value>
        public Vector3 LocalDeltaPosition
        {
            get { return TransformUtils.GlobalToLocalVector(cachedTransform, DeltaPosition); }
        }

        /// <summary>
        /// Gets rotation axis of the gesture in world coordinates.
        /// </summary>
        /// <value>Rotation axis of the gesture in world coordinates.</value>
        public Vector3 RotationAxis
        {
            get { return transformPlane.normal; }
        }

        #endregion

        #region Private variables

        [SerializeField]
        private ProjectionType projection = ProjectionType.Layer;

        [SerializeField]
        private Vector3 projectionPlaneNormal = Vector3.forward;

        private TouchLayer projectionLayer;
        private Plane transformPlane;

        #endregion

        #region Public methods

        /// <inheritdoc />
        public void ApplyTransform(Transform target)
        {
            if (!Mathf.Approximately(DeltaScale, 1f) || !Mathf.Approximately(DeltaRotation, 0f))
            {
                // check where the touches were before moving
                Vector2 oldpos0 = getPointPreviousScreenPosition(0);
                //Vector2 oldpos1 = getPointPreviousScreenPosition(1);
                Camera c = touchCam;
                //Vector3 oldworldpos0 = c.ScreenToWorldPoint(new Vector3(oldpos0.x, oldpos0.y, 10));

                Vector3 oldworldpos0;
                //Vector3 oldworldpos1;
                //if (CameraRayCast(c, oldpos0, out oldworldpos0)&& CameraRayCast(c, oldpos1, out oldworldpos1))

                if (CameraRayCast(c, oldpos0, out oldworldpos0))
                {

                    //Vector3 oldworldpos1 = c.ScreenToWorldPoint(new Vector3(oldpos1.x, oldpos1.y, 10));
                    // move camera further away

                    if (!Mathf.Approximately(DeltaScale, 1f))
                        setScaleValues(1.0f / deltaScale);
                    //if (!Mathf.Approximately(DeltaScale, 1f)) target.position = new Vector3(target.position.x, target.position.y / DeltaScale, target.position.z);
                    // rotate camera
                    if (!Mathf.Approximately(DeltaRotation, 0f))
                        target.rotation = Quaternion.AngleAxis(DeltaRotation, -RotationAxis) * target.rotation;

                    // get new pos
                    Vector2 newpos0 = getPointScreenPosition(0);
                    //Vector2 newpos1 = getPointScreenPosition(1);
                    Vector3 newworldpos0;
                    //Vector3 newworldpos1;
                    //if (CameraRayCast(c, newpos0, out newworldpos0)|| CameraRayCast(c, newpos1, out newworldpos1))

                    if (CameraRayCast(c, newpos0, out newworldpos0))
                    {
                        newworldpos0.Scale(new Vector3(1, 0, 1));
                        oldworldpos0.Scale(new Vector3(1, 0, 1));
                        Vector3 offset = newworldpos0 - oldworldpos0;
                        // move the camera so the old touchpoints are at the new position
                        target.position -= offset;
                        target.position = Vector3Clamp(target.position, bounds.min, bounds.max, 1, 0, 1);
                        //Debug.Log(Time.realtimeSinceStartup+": Offset "+offset.ToString("F3")+"; "+DeltaPosition.ToString("F3"));
                    }
                }
            }
            else
            {

                Vector3 offset = Vector3.zero;
                Vector3 oldpos0 = getPointPreviousScreenPosition(0);
                Camera c = touchCam;
                //Vector3 oldworldpos0 = c.ScreenToWorldPoint(new Vector3(oldpos0.x, oldpos0.y, 10));

                Vector3 oldworldpos0;
                //Vector3 oldworldpos1;
                //if (CameraRayCast(c, oldpos0, out oldworldpos0)&& CameraRayCast(c, oldpos1, out oldworldpos1))

                if (CameraRayCast(c, oldpos0, out oldworldpos0))
                {

                }
                Vector2 newpos0 = getPointScreenPosition(0);
                Vector3 newworldpos0;
                if (CameraRayCast(c, newpos0, out newworldpos0))
                {
                    newworldpos0.Scale(new Vector3(1, 0, 1));
                    oldworldpos0.Scale(new Vector3(1, 0, 1));
                    offset = newworldpos0 - oldworldpos0;
                }
                //if (DeltaPosition != Vector3.zero) {
                target.position -= offset;
                target.position = Vector3Clamp(target.position, bounds.min, bounds.max, 1, 0, 1);
                //}
            }



            //if (!Mathf.Approximately(DeltaScale, 1f)) target.localScale *= DeltaScale;
            //if (!Mathf.Approximately(DeltaScale, 1f)) ZoomCamera(target, DeltaScale);
            //if (!Mathf.Approximately(DeltaRotation, 0f)) target.rotation = Quaternion.AngleAxis(DeltaRotation, -RotationAxis) * target.rotation;
            //if (DeltaPosition != Vector3.zero) target.position -= DeltaPosition;

        }
        private void setScaleValues(float deltaScale)
        {
            if (cameraScale * deltaScale < bounds.max.y && cameraScale * deltaScale > bounds.min.y)
            {

                cameraScale *= deltaScale;
                scaler.LocalToWorldScale = cameraScale;
                scaler.DoUpdate();
                //headNode.transform.localPosition*=deltaScale;
                ipd.difference *= deltaScale;
                foreach (AsymFrustum f in frustums)
                {
                    f.width *= deltaScale;
                    f.height *= deltaScale;
                }
            }
        }

        public bool CameraRayCast(Camera c, Vector2 screenPos, out Vector3 worldpos)
        {
            RaycastHit hit;
            if (Physics.Raycast(c.ScreenPointToRay(screenPos), out hit))
            {
                worldpos = hit.point;
                return true;
            }
            else
            {
                worldpos = Vector3.zero;
                return false;
            }
        }
        #endregion

        #region Unity methods

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();
            transformPlane = new Plane();
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();
            updateProjectionPlane();
        }

        #endregion

        #region Gesture callbacks

        /// <inheritdoc />
        protected override void touchesBegan(IList<TouchPoint> touches)
        {
            base.touchesBegan(touches);

            if (State != GestureState.Possible) return;
            if (NumTouches == touches.Count)
            {
                projectionLayer = activeTouches[0].Layer;
                updateProjectionPlane();
            }
        }

        #endregion

        #region Protected methods

        /// <inheritdoc />
        protected override float doRotation(Vector2 oldScreenPos1, Vector2 oldScreenPos2, Vector2 newScreenPos1,
                                            Vector2 newScreenPos2, ProjectionParams projectionParams)
        {
            var newVector = projectionParams.ProjectTo(newScreenPos2, TransformPlane) -
                projectionParams.ProjectTo(newScreenPos1, TransformPlane);
            var oldVector = projectionParams.ProjectTo(oldScreenPos2, TransformPlane) -
                projectionParams.ProjectTo(oldScreenPos1, TransformPlane);
            var angle = Vector3.Angle(oldVector, newVector);
            if (Vector3.Dot(Vector3.Cross(oldVector, newVector), TransformPlane.normal) < 0)
                angle = -angle;

            return angle;
        }

        /// <inheritdoc />
        protected override float doScaling(Vector2 oldScreenPos1, Vector2 oldScreenPos2, Vector2 newScreenPos1,
                                           Vector2 newScreenPos2, ProjectionParams projectionParams)
        {
            var newVector = projectionParams.ProjectTo(newScreenPos2, TransformPlane) -
                projectionParams.ProjectTo(newScreenPos1, TransformPlane);
            var oldVector = projectionParams.ProjectTo(oldScreenPos2, TransformPlane) -
                projectionParams.ProjectTo(oldScreenPos1, TransformPlane);
            return newVector.magnitude / oldVector.magnitude;
        }


        protected override Vector3 doOnePointTranslation(Vector2 oldScreenPos, Vector2 newScreenPos,
                                                         ProjectionParams projectionParams)
        {
            Vector2 oldScreenPos2 = new Vector2(oldScreenPos.x, oldScreenPos.y + 1.0f);
            Vector2 newScreenPos2 = new Vector2(newScreenPos.x, newScreenPos.y + 1.0f);

            if (isTransforming)
            {
                return projectionParams.ProjectTo(newScreenPos, TransformPlane) - projectScaledRotated(oldScreenPos, 0.0f, 0.0f, projectionParams);
            }

            screenPixelTranslationBuffer += newScreenPos - oldScreenPos;
            if (screenPixelTranslationBuffer.sqrMagnitude > screenTransformPixelThresholdSquared)
            {
                isTransforming = true;
                return projectionParams.ProjectTo(newScreenPos, TransformPlane) -
                    projectScaledRotated(newScreenPos - screenPixelTranslationBuffer, 0.0f, 0.0f, projectionParams);
            }

            return Vector3.zero;
        }

        /// <inheritdoc />
        protected override Vector3 doTwoPointTranslation(Vector2 oldScreenPos1, Vector2 oldScreenPos2,
                                                         Vector2 newScreenPos1, Vector2 newScreenPos2, float dR, float dS, ProjectionParams projectionParams)
        {
            if (isTransforming)
            {
                return projectionParams.ProjectTo(newScreenPos1, TransformPlane) - projectScaledRotated(oldScreenPos1, dR, dS, projectionParams);
            }

            screenPixelTranslationBuffer += newScreenPos1 - oldScreenPos1;
            if (screenPixelTranslationBuffer.sqrMagnitude > screenTransformPixelThresholdSquared)
            {
                isTransforming = true;
                return projectionParams.ProjectTo(newScreenPos1, TransformPlane) -
                    projectScaledRotated(newScreenPos1 - screenPixelTranslationBuffer, dR, dS, projectionParams);
            }

            return Vector3.zero;
        }

        /// <inheritdoc />
        private Vector3 projectScaledRotated(Vector2 point, float dR, float dS, ProjectionParams projectionParams)
        {
            var delta = projectionParams.ProjectTo(point, TransformPlane) - cachedTransform.position;
            if (dR != 0) delta = Quaternion.AngleAxis(dR, RotationAxis) * delta;
            if (dS != 0) delta = delta * dS;
            return cachedTransform.position + delta;
        }

#if TOUCHSCRIPT_DEBUG
		protected override void clearDebug()
		{
			base.clearDebug();
			
			GLDebug.RemoveFigure(debugID + 3);
		}
		
		protected override void drawDebug(int touchPoints)
		{
			base.drawDebug(touchPoints);
			
			if (!DebugMode) return;
			switch (touchPoints)
			{
			case 1:
				if (projection == ProjectionType.Global || projection == ProjectionType.Object)
				{
					GLDebug.DrawPlaneWithNormal(debugID + 3, cachedTransform.position, RotationAxis, 4f, GLDebug.MULTIPLY, float.PositiveInfinity);
				}
				break;
			default:
				if (projection == ProjectionType.Global || projection == ProjectionType.Object)
				{
					GLDebug.DrawPlaneWithNormal(debugID + 3, cachedTransform.position, RotationAxis, 4f, GLDebug.MULTIPLY, float.PositiveInfinity);
				}
				break;
			}
		}
#endif

        #endregion

        #region Private functions

        /// <summary>
        /// Updates projection plane based on options set.
        /// </summary>
        private void updateProjectionPlane()
        {
            if (!Application.isPlaying) return;

            switch (projection)
            {
                case ProjectionType.Layer:
                    if (projectionLayer == null)
                        transformPlane = new Plane(cachedTransform.TransformDirection(Vector3.forward), cachedTransform.position);
                    else transformPlane = new Plane(projectionLayer.WorldProjectionNormal, cachedTransform.position);
                    break;
                case ProjectionType.Object:
                    transformPlane = new Plane(cachedTransform.TransformDirection(projectionPlaneNormal), cachedTransform.position);
                    break;
                case ProjectionType.Global:
                    transformPlane = new Plane(projectionPlaneNormal, cachedTransform.position);
                    break;
            }
        }
        Vector3 Vector3Clamp(Vector3 value, Vector3 min, Vector3 max, int x = 1, int y = 1, int z = 1)
        {
            Vector3 ret = value;
            if (x != 0)
                ret.x = Mathf.Clamp(value.x, min.x, max.x);
            if (y != 0)
                ret.y = Mathf.Clamp(value.y, min.y, max.y);
            if (z != 0)
                ret.z = Mathf.Clamp(value.z, min.z, max.z);
            return ret;
        }
        void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
        #endregion
    }
}