using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShot : MonoBehaviour
{
    private static ScreenShot instance;

    private Camera screenShotCamera;
    private bool takeScreenShotOnNextFrame;

    private void Awake()
    {
        instance = this;
        screenShotCamera = gameObject.GetComponent<Camera>();
    }

    //프레임당 최대 하나만 찍을수 있게 하기 위해서 
    private void OnPostRender()
    {
        if (takeScreenShotOnNextFrame == true)
        {
            takeScreenShotOnNextFrame = false;
            RenderTexture renderTexture = screenShotCamera.targetTexture;

            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            byte[] byteArray = renderResult.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + "/CameraScreenShot.png", byteArray);
            Debug.Log("Save Camera screenshot");

            RenderTexture.ReleaseTemporary(renderTexture);
            screenShotCamera.targetTexture = null;


            Debug.Log(byteArray);
        }
    }

    private void TakeScreenShot(int width, int height)
    {
        screenShotCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        takeScreenShotOnNextFrame = true;
    }

    public static void TakeScreenShot_Static(int width, int height)
    {
        instance.TakeScreenShot(width, height);
    }
}
