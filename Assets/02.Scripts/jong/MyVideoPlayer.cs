using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MyVideoPlayer : MonoBehaviour
{
    public VideoPlayer myVideo;

    void Start()
    {
        myVideo = GetComponent<VideoPlayer>();
    }

    void Update()
    {

    }
    public void VideoPlay()
    {
        myVideo.Play();
    }
    public void VideoPause()
    {
        myVideo.Pause();
    }
    public void VideoStop()
    {
        myVideo.Stop();
    }
}
