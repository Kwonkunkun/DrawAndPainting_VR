using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class TestHandLeftController : MonoBehaviour
{
	public SteamVR_Input_Sources leftHand = SteamVR_Input_Sources.LeftHand;

	public SteamVR_Action_Boolean trigger = SteamVR_Actions.default_InteractUI;
    
    // public Button button;
    public GameObject gameObject;

    bool buttonTouch = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if(trigger.GetStateDown(leftHand)){
        if(trigger.GetStateDown(leftHand)&&buttonTouch){
            Debug.Log("왼쪽 트리거 누름");
            gameObject.GetComponent<TestButton>().BtnLog();
            // if(button!=null){
            //     //button.onClick.Invoke();

            // }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        if(other.gameObject.tag=="Button"){
            //Debug.Log("버튼 누름");
            buttonTouch = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag=="Button"){
            //Debug.Log("버튼 머무름");
            buttonTouch = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        
        if(other.gameObject.tag=="Button"){
            //Debug.Log("버튼 나감");
            buttonTouch = false;
        }
    }

}
