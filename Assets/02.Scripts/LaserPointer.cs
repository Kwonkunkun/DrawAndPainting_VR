using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class LaserPointer : MonoBehaviour
{
    //라인 관련
    private LineRenderer line;
    public float maxDistance = 20.0f;
    public Color color = Color.blue;

    //레이케스트 관련
    private RaycastHit hit;
    private Transform tr;
    private int layerMask = 1 << 8;

    //brush 관련
    public GameObject Canvas;
    public GameObject brush;

    private void Start()
    {
        tr = GetComponent<Transform>();
    }
    private void Update()
    {
        if (line!=null && Physics.Raycast(tr.position, tr.forward, out hit, maxDistance, layerMask))
        {
            line.SetPosition(1, new Vector3(0, 0, hit.distance));

            Vector3 drawPos = hit.point;
            GameObject draw = Instantiate(brush);
            draw.transform.position = drawPos;
            draw.transform.up = hit.normal;
            draw.transform.SetParent(Canvas.transform);
            draw.transform.localPosition += new Vector3(0.0f, 0.001f, 0.0f);
        }
    }

    public void CreateLineRenderer()
    {
        //LineRenderer 생성
        line = this.gameObject.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.receiveShadows = false;

        //시작점과 끝점의 위치 설정
        line.positionCount = 2;
        line.SetPosition(0, Vector3.zero);
        line.SetPosition(1, new Vector3(0, 0, maxDistance));

        //라인의 너비 설정
        line.startWidth = 0.03f;
        line.endWidth = 0.005f;

        //라인의 머터리얼 및 색상 설정
        line.material = new Material(Shader.Find("Unlit/Color"));
        line.material.color = this.color;
    }

    public void DestoryLineRenderer()
    {
        Destroy(line);
        line = null;
    }
}
