using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public RenderTexture rt;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SavePresentPaint();
    }
    void SavePresentPaint()
    {
        byte[] byteArray = toTexture2D(rt).EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/CameraScreenShot.png", byteArray);
        Debug.Log("Save present paint");
    }
    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(256, 256, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;

        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        Color32[] cols = tex.GetPixels32();

        // 캡처 이미지의 픽셀을 흑백으로 재계산한다.
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                Color32 pixel = cols[x + y * tex.width];
                int p = ((256 * 256 + pixel.r) * 256 + pixel.b) * 256 + pixel.g;
                int b = p % 256;
                p = Mathf.FloorToInt(p / 256);
                int g = p % 256;
                p = Mathf.FloorToInt(p / 256);
                int r = p % 256;
                float l = (0.2126f * r / 255f) + 0.7152f * (g / 255f) + 0.0722f * (b / 255f);
                Color c = new Color(l, l, l, 1);
                tex.SetPixel(x, y, c);
            }
        }

        tex.Apply();
        return tex;
    }
}
