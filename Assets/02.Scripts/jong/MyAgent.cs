using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;

public class MyAgent : Agent
{
    [HideInInspector]   // 파이썬 종료
    public bool isEndPy =true;

    [HideInInspector]   // 이미지 체크
    public bool isImgCheck =false;
    
    private void Start()  { }


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

    int cnt = 0;
    int preAction =0;

    //파이썬의 머신러닝에 대한 액션을 입력받아서 어떻게 행동할지 해서 리워드로 다시 파이썬에 보냄
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        int action = (int)vectorAction[0];

        if (action != 0) print(action.ToString());

        if(preAction!=action){
            // switch (action) {
            //     case 0: {   // 아직 그림 인식을 안하는 상태

            //             break;
            //         }
            //     case 1: {
            //             playerButton.PreCreate01();

            //             // 다시 그림 인식을 안하는 상태로 변경
            //             isImgCheck = false;
            //             break;
            //         }
            //     case 2: {
            //             playerButton.PreCreate02();

            //             isImgCheck = false;
            //             break;
            //         }
            //     case 3: {
            //             playerButton.PreCreate03();

            //             isImgCheck = false;
            //             break;
            //         }
            //     case 4: {
            //             playerButton.PreCreate04();

            //             isImgCheck = false;
            //             break;
            //         }
            //     case 5: {
            //             playerButton.PreCreate05();

            //             isImgCheck = false;
            //             break;
            //         }
            //     case 6: {
            //             playerButton.PreCreate06();

            //             isImgCheck = false;
            //             break;
            //         }
            //     case 7: {
            //             playerButton.PreCreate07();

            //             isImgCheck = false;
            //             break;
            //         }
            //     case 8: {
            //             playerButton.PreCreate08();

            //             isImgCheck = false;
            //             break;
            //         }
            //     case 9: {
            //             playerButton.PreCreate09();

            //             isImgCheck = false;
            //             break;
            //         }
            //     case 10: {
            //             playerButton.PreCreate10();

            //             isImgCheck = false;
            //             break;
            //         }
            //}
            //Instantiate(gb);
            isImgCheck = false;
            preAction=action;
        }
    }

    // 다시 시작
    public override void AgentReset()
    {
        
    }


}
