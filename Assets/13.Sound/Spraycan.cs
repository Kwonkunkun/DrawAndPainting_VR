using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintIn3D;
using Valve.VR.InteractionSystem;

public class Spraycan : MonoBehaviour
{
    P3dToggleParticles toggleEffect;
    Interactable myInteractive;

    private AudioSource theAudio;

    [SerializeField] private AudioClip clip;

    bool isPlay = false;

    // Use this for initialization
    void Start()
    {
        theAudio = GetComponent<AudioSource>();
        theAudio.loop = true;
        theAudio.clip = clip;

        toggleEffect = GetComponent<P3dToggleParticles>();
        myInteractive = GetComponent<Interactable>();

    }

    // Update is called once per frame
    void Update()
    {
        // 만일, 캔을 잡았다면 사운드를 출력한다.
        if (toggleEffect.Target != null)
        {
            if(toggleEffect.isCatch && !isPlay && toggleEffect.isOnClickMenuButton)
            {
                theAudio.Play();
                isPlay = true;
            }
            else if(!toggleEffect.isCatch && isPlay )
            {
                theAudio.Stop();
                isPlay = false;
            } else if (!toggleEffect.isOnClickMenuButton && isPlay)
            {
                theAudio.Stop();
                isPlay = false;
            }
        }
    }
}
