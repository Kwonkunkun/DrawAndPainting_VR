using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{

    public GameObject canvasClearBtn;
    [SerializeField]
    MyAgent myAgent;

    [SerializeField]
    private GameObject Temp;        //회전 테이블 자식의 위치를 이용하기 위함
    [SerializeField]
    private GameObject ColorTable;  //회전 테이블 객체


    void Start() { }

    void Update() { }

    public void ClickImageButton() {
        Debug.Log("이미지 인식버튼");
        myAgent.isImgCheck = true;

        float[] testVal = { 1.0f, 2.0f };
        myAgent.AgentAction(testVal,"Test");
    }

    public void ClickCheckEndButton() {
        Debug.Log("종료버튼");
        myAgent.isEndPy = false;
    }

    public void ClickOutObject() {
        myAgent.tabletDontTouch.SetActive(false);
        myAgent.printButton.gameObject.SetActive(false);
        myAgent.printingText.SetActive(false);
        myAgent.GaugeImage.fillAmount = 0f;

        /*  출력물을 이제 어디에 위치해야 하는지 지정  */
        myAgent.printObject.transform.position = Temp.transform.position;
        myAgent.printObject.transform.parent = ColorTable.transform;


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
