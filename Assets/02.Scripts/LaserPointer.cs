using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class LaserPointer : MonoBehaviour
{
    //레이저 관련
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

    //line 관련
    public GameObject lineRendererFactory;
    private LineRenderer lineRenderer;

    //베지에 곡선 관련
    private int linePosCnt;
    private Vector3[] pos = new Vector3[3];

    private void Start()
    {
        tr = GetComponent<Transform>();
       
    }
    private void Update()
    {
        if (line!=null && Physics.Raycast(tr.position, tr.forward, out hit, maxDistance, layerMask))
        {
            line.SetPosition(1, new Vector3(0, 0, hit.distance));

            //Vector3 drawPos = hit.point;
            //GameObject draw = Instantiate(brush);
            //draw.transform.position = drawPos;
            //draw.transform.up = hit.normal;
            //draw.transform.SetParent(Canvas.transform);
            //draw.transform.localPosition += new Vector3(0.0f, 0.001f, 0.0f);

            //lineRenderer가 존재안하면 return 하슈
            if (lineRenderer == null)
                return;

            //움직임이 별로없었으면 넘기는게 좋을듯
            if (lineRenderer.positionCount > 0)
            {
                Vector3 beforePos = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
                Vector3 presentPos = hit.point;

                if (Vector3.Distance(beforePos, presentPos) < 0.005f)
                    return;
            }

            //베지에 곡선 관련
            pos[linePosCnt] = hit.point + new Vector3(0.0f, 0.0f, -0.01f);

            //line point 추가
            if (linePosCnt == 2)
            {
                for (int i = 0; i <= 10; i++)
                {
                    lineRenderer.positionCount++;
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, CalculateBezier(i / 10.0f));
                }
            }
            
            linePosCnt++;
            linePosCnt %= 3;
        }
    }

    private Vector3 CalculateBezier(float ratio)
    {
        Vector3 p1 = Vector3.Lerp(pos[0], pos[1], ratio);
        Vector3 p2 = Vector3.Lerp(pos[1], pos[2], ratio);
        Vector3 p3 = Vector3.Lerp(p1, p2, ratio);
        return p3;
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

        //라인생성
        lineRenderer = Instantiate(lineRendererFactory).GetComponent<LineRenderer>();
    }

    public void DestoryLineRenderer()
    {
        Destroy(line);
        line = null;

        //라인 초기화
        lineRenderer.gameObject.transform.SetParent(Canvas.transform);
        lineRenderer = null;
        linePosCnt = 0;
    }
}
