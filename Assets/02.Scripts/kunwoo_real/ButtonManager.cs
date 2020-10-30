using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{

    public GameObject canvasClear;

    [SerializeField]
    MyAgent myAgent;

    [SerializeField]
    private GameObject Temp;        //회전 테이블 자식의 위치를 이용하기 위함
    [SerializeField]
    private GameObject ColorTable;  //회전 테이블 객체
    [SerializeField]
    private GameObject particleEvent;   //파티클
    [SerializeField]
    private GameObject PrinterparticlePos01;   //파티클 위치
    [SerializeField]
    private GameObject PrinterparticlePos02;   //파티클 위치

    [SerializeField]
    private GameObject player;      // 플레이어
    [SerializeField]
    private GameObject playerPos;   // 플레이어 게임시작 이동 위치
    [SerializeField]
    private GameObject playerCam;      // 플레이어 카메라
    [SerializeField]
    private GameObject[] video;      // 비디오 버튼

    public bool TEST_MODE;

    void Start() { }

    void Update() { }

    public void ClickImageButton() {
        //Debug.Log("이미지 인식버튼");
        myAgent.isImgCheck = true;

        if (TEST_MODE) {
            float[] testVal = { 14.0f };
            myAgent.AgentAction(testVal, "Test");
        }
    }

    public void ClickCheckEndButton() {
        //Debug.Log("종료버튼");
        myAgent.isEndPy = false;
    }

    public void ClickPrintTakeOutButton() {
        StartCoroutine(PrintTakeOutCo());
    }
    IEnumerator PrintTakeOutCo() {
        // 
        myAgent.printButtonHold.SetActive(true);
        

        AudioSource printTakeOutSound = GetComponent<AudioSource>();
        printTakeOutSound.Play();


        // 효과 발동
        GameObject printParticle01 = Instantiate(particleEvent, PrinterparticlePos01.transform);
        Destroy(printParticle01, 3);

        yield return new WaitForSeconds(2f);

        Destroy(myAgent.printObjectShaderApply);

        GameObject printParticle02 = Instantiate(particleEvent, PrinterparticlePos02.transform);
        Destroy(printParticle02, 3);

        yield return new WaitForSeconds(2f);

        myAgent.printButton.gameObject.SetActive(false);
        myAgent.tabletDontTouch.SetActive(false);
        myAgent.GaugeImage.fillAmount = 0f;

        CanvasClear();  // 그림판 지우기

        /*  출력물을 이제 어디에 위치해야 하는지 지정  */
        myAgent.printObject.transform.position = Temp.transform.position;
        myAgent.printObject.transform.parent = ColorTable.transform;
        myAgent.printObject.SetActive(true);
    }


    public void CanvasClear(){  // 지우기 버튼

        //Debug.Log("캔버스 메소드 호출");
        StartCoroutine(CanvasClearCo());
    }
    IEnumerator CanvasClearCo(){
        canvasClear.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        canvasClear.SetActive(false);
    }
    

    public void ClickGameStartButton() // 시작버튼
    {
        StartCoroutine(GameStartCo());
    }

    IEnumerator GameStartCo()
    {
        AudioSource printTakeOutSound = GetComponent<AudioSource>();
        printTakeOutSound.Play();

        GameObject printParticle01 = Instantiate(particleEvent, playerCam.transform);
        printParticle01.transform.localScale = new Vector3(1f, 1f, 1f);
        printParticle01.transform.localPosition = new Vector3(printParticle01.transform.localPosition.x, -0.3f, printParticle01.transform.localPosition.z);
        Destroy(printParticle01, 2);

        yield return new WaitForSeconds(1f);

        GameObject printParticle02 = Instantiate(particleEvent, playerCam.transform);
        printParticle02.transform.localScale = new Vector3(1f, 1f, 1f);
        printParticle02.transform.localPosition = new Vector3(printParticle02.transform.localPosition.x, -0.3f, printParticle02.transform.localPosition.z);
        Destroy(printParticle02, 2);

        yield return new WaitForSeconds(1f);

        player.transform.position = playerPos.transform.position;
        player.transform.rotation = playerPos.transform.rotation;

        for (int i = 0; i < video.Length; i++)
        {
            video[i].GetComponent<MyVideoPlayer>().VideoPause();
            video[i].GetComponent<MyVideoPlayer>().VideoStop();
        }

    }
}
