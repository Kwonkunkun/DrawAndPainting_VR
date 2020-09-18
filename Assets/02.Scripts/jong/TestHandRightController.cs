using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class TestHandRightController : MonoBehaviour
{
		public SteamVR_Input_Sources rightHand = SteamVR_Input_Sources.RightHand;

		public SteamVR_Action_Boolean trigger = SteamVR_Actions.default_InteractUI;

    
    bool buttonTouch = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(trigger.GetStateDown(rightHand)){
            Debug.Log("오른쪽 트리거 누름");
        }
    }
}
