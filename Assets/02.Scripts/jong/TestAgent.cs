using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;

public class TestAgent : Agent
{
    private bool isEndPy =true;
    private bool isImgCheck =false;

    //public GameObject gb;


    public Text text;
    //PlayerButton playerButton; 
    
    private void Start()
    {
        //playerButton = transform.GetComponent<PlayerButton>();
    }


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


    public void BtnEndPy(){
        Debug.Log("종료버튼");
        text.text = "종료버튼 눌림";
        isEndPy = false;
    }

    public void BtnImgCheck(){
        Debug.Log("이미지 인식버튼");
        text.text = "인식버튼 눌림";
        isImgCheck =true;
    }

    public void BtnLog(){
        Debug.Log("버튼이 눌렸습니다.");
    }
}
