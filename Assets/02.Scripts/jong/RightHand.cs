using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using PaintIn3D;

public class RightHand : MonoBehaviour
{
    public SteamVR_Action_Boolean SphereOnOff;

    public SteamVR_Input_Sources handType;

    void Start()
    {
        SphereOnOff.AddOnStateDownListener(TriggerDown, handType);
        SphereOnOff.AddOnStateUpListener(TriggerUp, handType);
    }

    public void TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        Transform allChildren = transform.GetChild(transform.childCount - 1);
        if ((allChildren.name).Substring(0, 8) == "SprayCan")
        {
            P3dToggleParticles sprayCan = allChildren.gameObject.GetComponent<P3dToggleParticles>();
            sprayCan.isOnClickMenuButton = true;
        }
    }
    public void TriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        Transform allChildren = transform.GetChild(transform.childCount - 1);
        if ((allChildren.name).Substring(0, 8) == "SprayCan")
        {
            P3dToggleParticles sprayCan = allChildren.gameObject.GetComponent<P3dToggleParticles>();
            Debug.Log("!!!!!!");
            sprayCan.isOnClickMenuButton = false;
        }
    }
    

}
