﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;

public class MyAgent : Agent
{
    // 파이썬 종료, 이미지 처리 여부
    public bool isEndPy = false;
    public bool isImgCheck = false;

    public List<GameObject> DrawObjects;

    #region User define function
    public void EndPython()
    {
        isEndPy = true;
    }
    public void CheckImage()
    {
        isImgCheck = true;
    }

    #endregion

    #region ML function
    // 초기화
    public override void InitializeAgent()
    {
        //academy = FindObjectOfType(typeof(TestAcademy)) as TestAcademy;
    }

    // 파이썬에 보낼 값을 설정(수치 추가)
    public override void CollectObservations()
    {
        AddVectorObs(isEndPy);
        AddVectorObs(isImgCheck);
    }

    int preAction = 0;

    //파이썬의 머신러닝에 대한 액션을 입력받아서 어떻게 행동할지 해서 리워드로 다시 파이썬에 보냄
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        int action = (int)vectorAction[0];
        if (isImgCheck == false){
            return;
        } else {
            //인식이 되었을때 오브젝트 생성
            //GameObject obj = Instantiate(DrawObjects[action-1]);
            isImgCheck = false;
            action = 0;   
        }

           


        //obj를 형이 paint 매니저 (싱글톤 추천) 를 만들어서 넣으면됨 
     
    }

    // 다시 시작
    public override void AgentReset()
    {
        
    }

    #endregion
}
