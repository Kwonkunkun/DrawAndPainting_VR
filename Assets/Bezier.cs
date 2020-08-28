using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier : MonoBehaviour
{
    public Transform tr1;
    public Transform tr2;
    public Transform tr3;

    LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();   
    }

    // Update is called once per frame
    void Update()
    {
        lr.positionCount = 11;
        for(int i = 0; i <= 10; i++)
        {
             lr.SetPosition(i, SetLinePoint(i / 10.0f));
        }
    }

    Vector3 SetLinePoint(float ratio)
    {
        Vector3 p1 = Vector3.Lerp(tr1.position, tr2.position, ratio);
        Vector3 p2 = Vector3.Lerp(tr2.position, tr3.position, ratio);

        Vector3 p3 = Vector3.Lerp(p1, p2, ratio);
        return p3;
    }
}
