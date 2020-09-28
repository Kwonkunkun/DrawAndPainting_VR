using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

public class ButtonController : MonoBehaviour
{
    private Coroutine buttonHintCoroutine;
    private Coroutine textHintCoroutine;

    public GameObject player;
    public GameObject menu;
    //public Text txt;

    public GameObject LeftDoor;
    public GameObject RightDoor;

    //-------------------------------------------------
    public void ShowButtonHints(Hand hand)
    {
        if (buttonHintCoroutine != null)
        {
            StopCoroutine(buttonHintCoroutine);
        }
        buttonHintCoroutine = StartCoroutine(TestButtonHints(hand));
    }


    //-------------------------------------------------
    public void ShowTextHints(Hand hand)
    {
        if (textHintCoroutine != null)
        {
            StopCoroutine(textHintCoroutine);
        }
        textHintCoroutine = StartCoroutine(TestTextHints(hand));
    }

	public void GameStart() // 게임시작 버튼
    {
        LeftDoor.GetComponent<Animator>().SetBool("Open",true);
        RightDoor.GetComponent<Animator>().SetBool("Open",true);

		// 게임 시작시에 이벤트 호출
		player.GetComponent<PlayerItween>().StartGame();


        if (buttonHintCoroutine != null)
        {
            StopCoroutine(buttonHintCoroutine);
            buttonHintCoroutine = null;
        }

        if (textHintCoroutine != null)
        {
            StopCoroutine(textHintCoroutine);
            textHintCoroutine = null;
        }

        foreach (Hand hand in Player.instance.hands)
        {
            ControllerButtonHints.HideAllButtonHints(hand);
            ControllerButtonHints.HideAllTextHints(hand);
        }
    }


    //-------------------------------------------------
    public void GameEnd()   // 게임종료 버튼
    {
		// 게임 종료시에 이벤트 호출
		player.GetComponent<ButtonEvent>().BtnEndPy();

        if (buttonHintCoroutine != null)
        {
            StopCoroutine(buttonHintCoroutine);
            buttonHintCoroutine = null;
        }

        if (textHintCoroutine != null)
        {
            StopCoroutine(textHintCoroutine);
            textHintCoroutine = null;
        }

        foreach (Hand hand in Player.instance.hands)
        {
            ControllerButtonHints.HideAllButtonHints(hand);
            ControllerButtonHints.HideAllTextHints(hand);
        }
    }

    public void ReturnMenu()   // 메뉴로 돌아가기 버튼
    {
        LeftDoor.GetComponent<Animator>().SetBool("Open",true);
        RightDoor.GetComponent<Animator>().SetBool("Open",true);
        
		// 게임 종료시에 이벤트 호출
		player.GetComponent<PlayerItween>().ReturnMenu();

        if (buttonHintCoroutine != null)
        {
            StopCoroutine(buttonHintCoroutine);
            buttonHintCoroutine = null;
        }

        if (textHintCoroutine != null)
        {
            StopCoroutine(textHintCoroutine);
            textHintCoroutine = null;
        }

        foreach (Hand hand in Player.instance.hands)
        {
            ControllerButtonHints.HideAllButtonHints(hand);
            ControllerButtonHints.HideAllTextHints(hand);
        }
    }

    public void ImageRecog()   // 이미지 인식버튼 이벤트 호출
    {
		// 게임 종료시에 이벤트 호출
		player.GetComponent<ButtonEvent>().BtnImgCheck();

        if (buttonHintCoroutine != null)
        {
            StopCoroutine(buttonHintCoroutine);
            buttonHintCoroutine = null;
        }

        if (textHintCoroutine != null)
        {
            StopCoroutine(textHintCoroutine);
            textHintCoroutine = null;
        }

        foreach (Hand hand in Player.instance.hands)
        {
            ControllerButtonHints.HideAllButtonHints(hand);
            ControllerButtonHints.HideAllTextHints(hand);
        }
    }


    //-------------------------------------------------
    // Cycles through all the button hints on the controller
    //-------------------------------------------------
    private IEnumerator TestButtonHints(Hand hand)
    {
        ControllerButtonHints.HideAllButtonHints(hand);

        while (true)
        {
            for (int actionIndex = 0; actionIndex < SteamVR_Input.actionsIn.Length; actionIndex++)
            {
                ISteamVR_Action_In action = SteamVR_Input.actionsIn[actionIndex];
                if (action.GetActive(hand.handType))
                {
                    ControllerButtonHints.ShowButtonHint(hand, action);
                    yield return new WaitForSeconds(1.0f);
                    ControllerButtonHints.HideButtonHint(hand, action);
                    yield return new WaitForSeconds(0.5f);
                }
                yield return null;
            }

            ControllerButtonHints.HideAllButtonHints(hand);
            yield return new WaitForSeconds(1.0f);
        }
    }


    //-------------------------------------------------
    // Cycles through all the text hints on the controller
    //-------------------------------------------------
    private IEnumerator TestTextHints(Hand hand)
    {
        ControllerButtonHints.HideAllTextHints(hand);

        while (true)
        {
            for (int actionIndex = 0; actionIndex < SteamVR_Input.actionsIn.Length; actionIndex++)
            {
                ISteamVR_Action_In action = SteamVR_Input.actionsIn[actionIndex];
                if (action.GetActive(hand.handType))
                {
                    ControllerButtonHints.ShowTextHint(hand, action, action.GetShortName());
                    yield return new WaitForSeconds(3.0f);
                    ControllerButtonHints.HideTextHint(hand, action);
                    yield return new WaitForSeconds(0.5f);
                }
                yield return null;
            }

            ControllerButtonHints.HideAllTextHints(hand);
            yield return new WaitForSeconds(3.0f);
        }
    }
}
