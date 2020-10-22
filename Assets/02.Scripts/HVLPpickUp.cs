using PaintIn3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HVLPpickUp : MonoBehaviour
{
    public bool isPickUp;
    public ParticleSystem particleSystem;
    public GameObject curvedCanvas;
    public P3dPaintSphere p3DPaintSphere;

    ParticleSystem.MainModule main;

    private void Start()
    {
        main = particleSystem.main;
    }
    void Update()
    {
        if (isPickUp == true)
            particleSystem.Play();
        else
            particleSystem.Pause();
    }

    public void pickUp()
    {
        isPickUp = true;
        curvedCanvas.SetActive(true);
    }

    public void putDown()
    {
        isPickUp = false;
        //curvedCanvas.SetActive(false);
    }

    public void ChangeColor(string colorName)
    {
        if(colorName == "RED")
        {   
            main.startColor = Color.red;
            p3DPaintSphere.Color = Color.red;
        }
        else if(colorName =="YELLOW")
        {
            main.startColor = Color.yellow;
            p3DPaintSphere.Color = Color.yellow;
        }
        else if(colorName == "GREEN")
        {
            main.startColor = Color.green;
            p3DPaintSphere.Color = Color.green;
        }
        else if(colorName == "BLUE")
        {
            main.startColor = Color.blue;
            p3DPaintSphere.Color = Color.blue;
        }
    }
}
