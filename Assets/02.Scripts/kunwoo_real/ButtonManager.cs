using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{

    public GameObject canvasClearBtn;
    [SerializeField]
    MyAgent myAgent;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public void CheckImageButton()
    {
        Debug.Log("이미지 인식버튼");
        myAgent.isImgCheck = true;

        float[] testVal = { 1.0f, 2.0f };
        myAgent.AgentAction(testVal,"Test");
    }

    public void CheckEndButton()
    {
        Debug.Log("종료버튼");
        myAgent.isEndPy = false;
    }

    public void canvasClear(){
        StartCoroutine(canvasClearCo());
    }

    IEnumerator canvasClearCo(){    // 그림판 지우기 버튼 이벤트
        
        canvasClearBtn.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        canvasClearBtn.SetActive(false);

    }
}
