using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOff : MonoBehaviour
{
    public Transform target;
    public Vector3 offSet;

    void Update()
    {
        transform.position = target.position + offSet;
    }
}
