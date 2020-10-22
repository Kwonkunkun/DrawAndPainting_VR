using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnOffUI : MonoBehaviour
{
    public bool toggleBtn;
    public GameObject curvedCanvas;

    public void ToggleButton()
    {
        if (curvedCanvas != null)
        {
            toggleBtn ^= true;
            curvedCanvas.SetActive(toggleBtn);
        }
    }
}
