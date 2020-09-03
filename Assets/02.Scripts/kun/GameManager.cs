using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public RenderTexture rt;

    #region 싱글톤
    private static GameManager m_instance; // 싱글톤이 할당될 static 변수
    public static GameManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<GameManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }
    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
    }
    #endregion

    public void SavePresentPaint()
    {
        Debug.Log("Save present paint");
        StartCoroutine(Save(0.5f));
    }

    IEnumerator Save(float time)
    {
        yield return new WaitForSeconds(time);
        byte[] byteArray = toTexture2D(rt).EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/CameraScreenShot.png", byteArray);
    }

    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(256, 256, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;

        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        //Color32[] cols = tex.GetPixels32();

        //// 캡처 이미지의 픽셀을 흑백으로 재계산한다.
        //for (int x = 0; x < tex.width; x++)
        //{
        //    for (int y = 0; y < tex.height; y++)
        //    {
        //        Color32 pixel = cols[x + y * tex.width];
        //        int p = ((256 * 256 + pixel.r) * 256 + pixel.b) * 256 + pixel.g;
        //        int b = p % 256;
        //        p = Mathf.FloorToInt(p / 256);
        //        int g = p % 256;
        //        p = Mathf.FloorToInt(p / 256);
        //        int r = p % 256;
        //        float l = (0.2126f * r / 255f) + 0.7152f * (g / 255f) + 0.0722f * (b / 255f);
        //        Color c = new Color(l, l, l, 1);
        //        tex.SetPixel(x, y, c);
        //    }
        //}

        tex.Apply();
        return tex;
    }
}
