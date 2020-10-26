using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLook : MonoBehaviour
{
    public GameObject cameraToLookAt;

    void Start() { }

    void Update()  {

        Vector3 v = cameraToLookAt.transform.position - transform.position;
        v.x = v.z = 0;
        transform.LookAt(cameraToLookAt.transform.position - v );

        transform.Rotate(Vector3.up * 180);
    }
}
