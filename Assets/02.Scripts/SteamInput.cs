using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class SteamInput : MonoBehaviour
{
    #region 싱글톤
    private static SteamInput m_instance;
    public static SteamInput instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 SteamInput 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<SteamInput>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    #endregion

    #region Input 관련
    public SteamVR_Input_Sources any;
    public SteamVR_Behaviour_Pose m_LeftHandPose = null;
    public SteamVR_Behaviour_Pose m_RightHandPose = null;
    public SteamVR_Action_Boolean m_UpButton = null;
    public SteamVR_Action_Boolean m_PullAction = null;
    //public SteamVR_Action_Vibration haptic = SteamVR_Actions.default_Haptic;
    #endregion
    private void Update()
    {
        Inputs();
    }

    private void Inputs()
    {
        if(m_UpButton.GetStateDown(m_RightHandPose.inputSource))
        {
            Debug.Log("m_UpButton.GetStateDown(m_RightHandPose.inputSource)");
        }
        else if(m_UpButton.GetStateDown(m_LeftHandPose.inputSource))
        {
            Debug.Log("m_UpButton.GetStateDown(m_LeftHandPose.inputSource)");
        }
        if (m_PullAction.GetStateDown(m_RightHandPose.inputSource))
        {
            Debug.Log("m_PullAction.GetStateDown(m_RightHandPose.inputSource)");
        }
        if (m_PullAction.GetStateDown(m_LeftHandPose.inputSource))
        {
            Debug.Log("m_PullAction.GetStateDown(m_LeftHandPose.inputSource)");
        }
    }
}
