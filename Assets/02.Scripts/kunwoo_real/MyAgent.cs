using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;
using TMPro;

public class MyAgent : Agent
{
    // 파이썬 종료, 이미지 처리 여부

    /// <summary>
    /// isEndPy         : 파이썬 종료 확인
    /// isImgCheck      : 이미지 체크 확인
    /// modelstartPos   : 프린트 위치
    /// 
    /// DrawObjects             : 정규화된 모델 배열
    /// DrawObjectsShader       : 정규화된 모델의 홀로그램 쉐이터 적용 배열
    /// printObject             : 출력 모델 객체 
    /// printObjectShaderApply  : 출력 모델 
    /// 
    /// tabletDontTouch     : 출력중 테블릿 터치방지
    /// printButtonHold     : 출력버튼 막기
    /// printingText        : 프린터에 출력중 텍스트 표시
    /// printButton         : 프린터 꺼내기 버튼
    /// GaugeImage          : 출력 게이지 이미지
    /// </summary>
    public bool isEndPy = true;
    public bool isImgCheck = false;

    [SerializeField]
    private GameObject modelStartPos;

    public List<GameObject> DrawObjects;
    public List<GameObject> DrawObjectsShaderApply;
    public GameObject printObject;
    public GameObject printObjectShaderApply;

    public GameObject tabletDontTouch;
    public GameObject printButtonHold;
    public GameObject printingText;
    public Button printButton;
    public Image GaugeImage;

    int preAction = 0;

    #region User define function
    public void EndPython()
    {
        isEndPy = false;
    }
    public void CheckImage()
    {
        isImgCheck = true;
    }

    #endregion

    #region ML function
    public override void InitializeAgent()
    {
        //academy = FindObjectOfType(typeof(TestAcademy)) as TestAcademy;
    }

    public override void CollectObservations()
    {
        AddVectorObs(isEndPy);
        AddVectorObs(isImgCheck);
    }

    /// <summary>
    /// InitializeAgent()       : 초기화
    /// CollectObservations()   : 파이썬에 보낼 값을 설정(수치 추가)
    /// AgentAction()           : 파이썬의 머신러닝에 대한 액션을 입력받아서 어떻게 행동할지 해서 리워드로 다시 파이썬에 보냄
    /// AgentReset()            : 한회전 돌다시 시작
    /// </summary>
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        int action = (int)vectorAction[0];
        if (isImgCheck == false){
            return;
        } else {
            //인식이 되었을때 오브젝트 생성
            printObjectShaderApply = Instantiate(DrawObjectsShaderApply[action-1], modelStartPos.transform);
            printObject = Instantiate(DrawObjects[action-1], modelStartPos.transform);
            printObject.SetActive(false);
            StartCoroutine(PrintObjectCo());

            isImgCheck = false;
            action = 0;   
        }

    }

    public override void AgentReset() { }
    #endregion

    #region 프린터 출력을 위한 코루틴 생성
    IEnumerator PrintObjectCo()
    {

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Play();

        tabletDontTouch.SetActive(true);
        printingText.SetActive(true);

        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(0.5f);
            GaugeImage.fillAmount += 0.2f;
        }

        printingText.SetActive(false);
        printButtonHold.SetActive(false);
        printButton.gameObject.SetActive(true);
    }
    #endregion
}
