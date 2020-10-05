using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class TestViveControllerInput : MonoBehaviour
{

    //입력 소스 정의
    public SteamVR_Input_Sources leftHand   = SteamVR_Input_Sources.LeftHand;
    //액션 - 트랙패드 클릭(Teleport)
    public SteamVR_Action_Boolean trackPad = SteamVR_Actions.default_Teleport;

    public SteamVR_Action_Boolean trackPadTouch = SteamVR_Actions.default_Teleport;
    //액션 - 트랙패드 터치 좌표(TrackpadPosition)
    public SteamVR_Action_Vector2 trackPadPosition = SteamVR_Actions.default_TrackpadPosition;

    void Update()
    {
        if (trackPadTouch.GetState(leftHand)){
        Vector2 pos = trackPadPosition.GetAxis(leftHand);
        Debug.LogFormat("Touch position = {0}", pos);
        }
    }
}
