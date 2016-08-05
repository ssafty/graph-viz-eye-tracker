using UnityEngine;
using System.Collections;

public class UsingTarget : MonoBehaviour{
    [SerializeField]
    Transform target;
    public Transform Target
    {
        get
        {
            return target;
        }
        set
        {
            target = value;
        }
    }

}
