using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class EraseDraw : MonoBehaviour
{
    public float boldWidth;

    Transform tr;
    [SerializeField]
    List<LineRenderer> lines;
    private void Start()
    {
        tr = GetComponent<Transform>();    
    }

    #region USER DEFINE FUCTION
    public void EraseOne()
    {
        Destroy(tr.GetChild(tr.childCount - 1).gameObject);
    }

    public void EraseAll()
    {
        for(int i = tr.childCount -1; i >= 0; i--)
        {
            Destroy(tr.GetChild(i).gameObject);
        }
    }

    //이거 쓰는 이유 : 파이썬에서 28 x 28로 이미지를 압축시킬때 손실이 너무 많이 발생.. 이를 방지하기 위해서
    public void LineBold()
    {
        lines = new List<LineRenderer>();
        //라인을 일단 다 받고
        for (int i = 0; i < tr.childCount; i++)
        {
            lines.Add(tr.GetChild(i).GetComponent<LineRenderer>());
        }

        //라인 두껍게 변경
        foreach(LineRenderer line in lines)
        {
            line.startWidth = boldWidth;
            line.endWidth = boldWidth;
        }
        Debug.Log("LineBold");
    }

    #endregion

}
