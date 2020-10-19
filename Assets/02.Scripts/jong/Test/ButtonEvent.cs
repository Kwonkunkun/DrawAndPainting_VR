using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEvent : MonoBehaviour
{
    MyAgent myAgent;

    void Start()
    {
        myAgent = gameObject.GetComponent<MyAgent>();
    }

    public void BtnImgCheck()
    {
        Debug.Log("이미지 인식버튼");
        myAgent.isImgCheck = true;
    }

    public void BtnEndPy()
    {
        Debug.Log("종료버튼");
        myAgent.isEndPy = false;
    }

    public void BtnLog()
    {
        Debug.Log("버튼이 눌렸습니다.");
    }
}
