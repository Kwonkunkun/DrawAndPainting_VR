using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreebieCameraHookupScript : MonoBehaviour
{
    private Camera securityCamera;

    public Camera SecurityCamera
    {
        get 
        { 
            if(securityCamera == null)
            {
                GetCameraReference();
            }

            return securityCamera; 
        }
    }

    private void Awake()
    {
        GetCameraReference();
    }

    private void GetCameraReference()
    {
        securityCamera = GetComponent<Camera>();
    }
}
