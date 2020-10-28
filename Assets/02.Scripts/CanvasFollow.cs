using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFollow : MonoBehaviour
{
    public Transform targetTransform;
    
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetTransform.position, Time.deltaTime * 2);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetTransform.rotation, Time.deltaTime * 2);
    }
}
