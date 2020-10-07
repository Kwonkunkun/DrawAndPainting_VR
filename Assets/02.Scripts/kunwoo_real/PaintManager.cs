using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintManager : MonoBehaviour
{
    //그림그릴 물체
    public GameObject paintObj;

    #region 싱글톤
    private static PaintManager m_instance; // 싱글톤이 할당될 static 변수
    public static PaintManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<PaintManager>();
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

    void Update()
    {
        if (paintObj == null)
            return;


        //원하시는대로
    }

    //초기화 (paintobj를 null, )
    public void Init()
    {

    }
}
