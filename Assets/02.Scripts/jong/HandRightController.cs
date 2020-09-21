using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HandRightController : MonoBehaviour
{
	public SteamVR_Input_Sources rightHand = SteamVR_Input_Sources.RightHand;

	public SteamVR_Action_Boolean trigger = SteamVR_Actions.default_InteractUI;


    public GameObject player;

    bool touchBtn = false;
    string touchName = "";

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 왼쪽 버튼의 트리거 + 버튼에 컨트롤러를 가져다 댈떄
        if (trigger.GetStateDown(rightHand) && touchBtn)
        {
            Debug.Log("왼쪽 트리거 누름");

            if ("BtnRecog" == touchName)  // 인식버튼 
            {
                player.GetComponent<ButtonEvent>().BtnImgCheck();
            }

            if ("BtnEnd" == touchName)   // 종료버튼 
            {
                player.GetComponent<ButtonEvent>().BtnEndPy();

            }

            // 버튼에 있는 OnClick() 실행
            // if(button!=null){ button.onClick.Invoke(); }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.tag);

        if (other.gameObject.tag == "Button")
        {
            touchBtn = true;
            touchName = other.gameObject.name;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Button")
        {
            touchBtn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Button")
        {
            touchBtn = false;
            touchName = "";
        }
    }
}
