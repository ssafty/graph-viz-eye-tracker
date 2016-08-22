using System;
using System.Collections.Generic;
using UnityEngine;


public class PanCameraScript : MonoBehaviour
{
    [System.Serializable]
    public class AdvancedParameters
    {
        public float startIPD = 0.06f;
        public float startHeight = 1.3f;
        public float startWidth = 0.7f;
        public Vector3 startHeadPos = new Vector3(0, 0.5f, -0.35f);
        public float startHeadTrackingScale = 1.0f;
    }


    public SetIPD ipd;
    public List<AsymFrustum> frustums = new List<AsymFrustum>();
    public AdvancedParameters advancedParameters;
    public GameObject headNode;
    public GameObject tirc;
    public float trackIRScale;



    public void Start()
    {
        ipd = gameObject.GetComponentInChildren<SetIPD>();
        frustums.AddRange(gameObject.GetComponentsInChildren<AsymFrustum>());
        headNode.transform.localPosition = advancedParameters.startHeadPos;
        ipd.difference = advancedParameters.startIPD;
        foreach (AsymFrustum f in frustums)
        {
            f.width = advancedParameters.startWidth;
            f.height = advancedParameters.startHeight;
        }
        trackIRScale = advancedParameters.startHeadTrackingScale;
        //Debug.Log(bounds);
    }
    private void Update()
    {

        if (tirc != null)
        {
            tirc.GetComponent<SetLocalToWorldScale>().LocalToWorldScale = trackIRScale;
        }
    }
   
}
