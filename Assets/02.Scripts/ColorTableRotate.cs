using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using Valve.VR;

public class ColorTableRotate : MonoBehaviour
{
    public bool rotateTrigger;
    public float rotateSpeed;

    void Update()
    {
        if (rotateTrigger == false)
            RotateY();
        else if (rotateTrigger == true)
            RotateX();
    }

    public void UpRotateSpeed()
    {
        print("UpRotateSpeed");

        if (rotateSpeed >= 100)
            return;

        rotateSpeed += 10;
    }

    public void StopRotate()
    {
        print("StopRotate");
        rotateSpeed = 0;
    }

    public void DownRotateSpeed()
    {
        print("DownRotateSpeed");
        if (rotateSpeed <= 0)
            return;

        rotateSpeed -= 10;
    }

    public void RotateTrigger()
    {
        print("RotateTrigger");
        rotateTrigger ^= true;
    }

    //회전, y축 기준으로
    void RotateY()
    {
        transform.Rotate(new Vector3(0, rotateSpeed * Time.deltaTime, 0));
    }
    //회전, x축 기준으로
    void RotateX()
    {
        transform.Rotate(new Vector3(rotateSpeed * Time.deltaTime, 0, 0));
    }
}
